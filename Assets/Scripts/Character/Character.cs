using UnityEngine;

public class Character : MonoBehaviour
{
	#region Variables

	private bool m_canCuspe = true;
	private bool m_canHold = false;
	
	private CFSM m_state = null;

	[SerializeField]
	private ParticleSystem m_deathParticle = null;
	[SerializeField]
	private ParticleSystem m_respawnParticle = null;

	[SerializeField]
	private Balao m_balao = null;

	[SerializeField]
	private Cuspe m_cuspe = null;

	[SerializeField]
	private Totem m_totem = null;
	public Totem totem
	{
		get { return m_totem; }
  	}

	[SerializeField]
	private AnimationController m_animationController = null;
	public AnimationController animationController
	{
		get { return m_animationController; }
	}

	[SerializeField]
	private Transform m_boca = null;
	public Transform boca
	{
		get { return m_boca; }
	}

	[SerializeField]
	private int m_joystickId = 0;
	public int joystickId
	{
		get { return m_joystickId; }
	}

	[SerializeField]
	private float m_speed = 2.0f;
	public float speed
	{
		get { return m_speed; }
  	}

	private bool m_isHolding = false;
	public bool isHolding
	{ 
		get { return m_isHolding; }
	}

	private Transform m_transform = null;
	public new Transform transform
	{
		get
		{
			if(m_transform == null)
			{
				m_transform = base.transform;
			}

			return m_transform;
		}
	}

	private LifeManager m_lifeManager = null;
	public LifeManager lifeManager
	{
		get { return m_lifeManager; }
	}

	private Rigidbody2D m_rigidbody2D = null;
	public Rigidbody2D _rigidbody2D
	{
		get
		{
			if(m_rigidbody2D == null)
			{
				m_rigidbody2D = GetComponent<Rigidbody2D>();
			}

			return m_rigidbody2D;
		}
	}

	#endregion

	#region Basic_Methods

	private void Awake ()
	{
		Blinding.Instance.CreateNewPlayerAction(m_joystickId);
	}

	private void Start ()
	{
		transform.position = totem.spawn.position;
		m_cuspe.character = this;

		SetState (new Idle());

		m_lifeManager = new LifeManager(1);
		m_lifeManager.ResetLife();
		m_lifeManager.death += Death;

		if(m_joystickId == 1)
		{
			animationController.spriteRenderer.flipX = true;
		}
	}
	
	private void Update ()
	{
		if(VictoryManager.Instance.hasEnded)
		{
			return;
		}

		if(PauseManager.Instance.isPaused)
		{
			if(PauseManager.Instance.character == this)
			{
				if(Blinding.Instance.StartWasPressed(m_joystickId))
				{
					PauseManager.Instance.Resume();
				}
			}
			return;
		}

		if(Blinding.Instance.StartWasPressed(m_joystickId))
		{
			if(!VictoryManager.Instance.hasEnded)
			{
				PauseManager.Instance.character = this;
				PauseManager.Instance.Pause();
				return;
			}
		}

		if(m_isHolding)
		{
			if (Blinding.Instance.HoldWasReleased (m_joystickId))
			{
				m_isHolding = false;
				Sword.Instance.SetState (new Fall());
			}
		}

		if (m_canHold && Sword.Instance.canBeHold)
		{
			if (Blinding.Instance.HoldWasPressed (m_joystickId) && !Sword.Instance.onHoldState)
			{
				m_isHolding = true;

				PlayBallon();

				Sword.Instance.character = this;
				Sword.Instance.canBeHold = false;
				Sword.Instance.SetState (new Hold());
			}
		}

		if(m_state != null)
		{
			m_state.Update ();
		}

		if(!m_canCuspe)
		{
			return;
		}

		if(Blinding.Instance.ActionWasPressed(m_joystickId))
		{
			if (!m_isHolding)
			{
				if (!m_cuspe.gameObject.activeSelf)
				{
					m_canCuspe = false;
					SoundManager.Instance.PlaySFX (2);
					m_cuspe.Active (!animationController.spriteRenderer.flipX, Blinding.Instance.Direction(m_joystickId).y);
				}
			}
			else
			{
				m_isHolding = false;
				Sword.Instance.SetState(new Throw());
			}
		}
	}

	private void FixedUpdate ()
	{
		if(PauseManager.Instance.isPaused || VictoryManager.Instance.hasEnded)
		{
			return;
		}

		if(m_state != null)
		{
			m_state.FixedUpdate ();
		}
	}

	#endregion

	#region Other_Methods

	private void Death ()
	{
		SoundManager.Instance.PlaySFX (3);

		m_deathParticle.transform.position = transform.position;
		m_deathParticle.Play(true);

		if(Joystick.Instance.JoysticksCount() > 1)
		{
			Joystick.Instance.Vibrate(1.0f, m_joystickId);

			VictoryManager.Instance.InvokeStop ();
		}

		transform.position = totem.spawn.position;

		m_respawnParticle.transform.position = transform.position;
		m_respawnParticle.Play(true);

		lifeManager.ResetLife();
	}

	public void SetFlip ()
	{
		if (animationController.spriteRenderer.flipX)
		{
			if (Blinding.Instance.Direction(m_joystickId).x > 0.0f)
			{
				animationController.spriteRenderer.flipX = false;

				if(m_isHolding)
				{
					Sword.Instance.Flip();
				}
			}
		}
		else
		{
			if (Blinding.Instance.Direction (m_joystickId).x < 0.0f)
			{
				animationController.spriteRenderer.flipX = true;

				if(m_isHolding)
				{
					Sword.Instance.Flip();
				}
			}
		}
	}

	public void PlayFall ()
	{
		animationController.PlayByType(ANIMATION_TYPE.FALL);
	}

	public void ActiveCuspe ()
	{
		m_canCuspe = true;
	}

	public void Die ()
	{
		lifeManager.RemoveLife (1);
	}

	public void PlayBallon (bool win = false)
	{
		SoundManager.Instance.PlaySFX (m_joystickId == 0 ? Random.Range(12, 14) : Random.Range(15, 17));

		m_balao.transform.position = new Vector3(animationController.spriteRenderer.flipX ? boca.position.x - 0.25f : boca.position.x + 0.25f, boca.position.y, 0.0f);

		m_balao.Active (win);
	}

	#endregion

	#region States

	public void SetState (CFSM newState)
	{
		if(m_state != null)
		{
			m_state.ExitState ();
		}

		m_state = newState;

		if(m_state != null)
		{
			m_state.EnterState (this);
		}
	}

	#endregion

	#region Collision

	private void OnCollisionEnter2D (Collision2D other)
	{
		if(m_state != null)
		{
			m_state.Collision(other);
		}
	}

	private void OnTriggerEnter2D (Collider2D other)
	{
		if(other.gameObject.layer == 14)
		{
			m_canHold = true;
		}
	}

	private void OnTriggerExit2D (Collider2D other)
	{
		if(other.gameObject.layer == 14)
		{
			m_canHold = false;
		}
	}

	#endregion
}
