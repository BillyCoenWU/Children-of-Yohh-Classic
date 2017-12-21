using System.Collections;
using UnityEngine;
using RGSMS.Math;

public class Sword : Singleton<Sword>
{
	#region Variables

	[SerializeField]
	private ParticleSystem m_particleHit = null;
	public ParticleSystem particleHit
	{
		get
		{
			return m_particleHit;
		}
	}

	[SerializeField, Header("TEMPORARIO")]
	private float m_swordForceX = 100.0f;
	[SerializeField, Header("TEMPORARIO")]
	private float m_swordForceY = 5.0f;

	private float m_lerp = 0.0f;

	private Pillar m_pillar = null;
	public Pillar pillar
	{
		get
		{
			return m_pillar;
		}

		set
		{
			m_pillar = value;
		}
	}

	private SFSM m_state = null;

	private Vector3 m_angle = Constantes.VECTOR_3_ZERO;
	private Vector3 m_finalAngle = Constantes.VECTOR_3_ZERO;

	private bool m_canBeHold = false;
	public bool canBeHold
	{
		get
		{
			return m_canBeHold;
		}

		set
		{
			m_canBeHold = value;
		}
	}

	private bool m_canRotate = false;
	private bool m_canImpulse = false;
	private bool m_canCollider = true;

	private bool m_onHoldState = false;
	public bool onHoldState
	{
		get { return m_onHoldState; }
		set { m_onHoldState = value; }
	}

	private Character m_character = null;
	public Character character
	{
		get { return m_character; }
		set { m_character = value; }
	}

	private Transform m_center = null;
	public Transform center
	{
		get
		{
			if(m_center == null)
			{
				m_center = transform.GetChild (1);
			}

			return m_center;
		}
	}

	private Transform m_cabo = null;
	public Transform cabo
	{
		get
		{
			if(m_cabo == null)
			{
				m_cabo = transform.GetChild (0);
			}

			return m_cabo;
		}
	}

	private Collider2D m_caboCollider = null;
	public Collider2D caboCollider
	{
		get
		{
			if(m_caboCollider == null)
			{
				m_caboCollider = cabo.GetComponent<Collider2D>();
			}

			return m_caboCollider;
		}
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
		Instance = this;
	}

	private void Start ()
	{
		SetState (new Fall());
	}

	private void Update ()
	{
		if(PauseManager.Instance.isPaused || VictoryManager.Instance.hasEnded)
		{
			return;
		}

		if(m_state != null)
		{
			m_state.Update();
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
			m_state.FixedUpdate();
		}
	}

	#endregion

	#region Other_Methods

	public void EndLoopRotate ()
	{
		m_canRotate = false;
		StopCoroutine (LoopRotate());
	}

	public void SetHold ()
	{
		transform.SetParent (null);
		transform.localPosition = character.boca.position;
		transform.SetParent (character.transform);

		Flip();
	}

	public void UnlockCollider ()
	{
		canBeHold = true;
		m_canCollider = true;
	}

	public void Impulse ()
	{
		if(!m_canImpulse)
		{
			m_canImpulse = true;
			return;
		}

		SoundManager.Instance.PlaySFX (9);

		m_canCollider = false;
		Invoke("UnlockCollider", 0.4f);
		StartCoroutine (LoopRotate());
		_rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
		_rigidbody2D.velocity = Constantes.VECTOR_2_ZERO;
		_rigidbody2D.AddForce (new Vector2(0.0f, 35.0f), ForceMode2D.Impulse);
	}

	public void Flip ()
	{
		if(character != null)
		{
			transform.localEulerAngles = new Vector3 (0.0f, 0.0f, character.animationController.spriteRenderer.flipX ? 120.0f : 240.0f);
		}
	}

	public void SetThrowAngle ()
	{
		if (character != null)
		{
			transform.localEulerAngles = new Vector3 (0.0f, 0.0f, character.animationController.spriteRenderer.flipX ? 270.0f : 90.0f);
		}

		transform.SetParent (null);

		Vector3 v3 = transform.localPosition;
		v3.x += (transform.localPosition.x - center.position.x);
		transform.localPosition = v3;

		if (character != null)
		{
			_rigidbody2D.AddForce (new Vector2 (character.animationController.spriteRenderer.flipX ? -m_swordForceX : m_swordForceX, m_swordForceY), ForceMode2D.Impulse);
		}
	}

	#endregion

	#region States

	public void SetState (SFSM newState)
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

	private void OnTriggerStay2D (Collider2D other)
	{
		if(!m_canCollider)
		{
			return;
		}

		if(m_state != null)
		{
			m_state.OnTrigger(other);
		}
	}

	#endregion

	#region Coroutines

	private IEnumerator LoopRotate ()
	{
		m_lerp = 0.0f;

		m_canRotate = true;
		m_angle = transform.localEulerAngles;
		m_finalAngle = m_angle;
		m_finalAngle.z += 360.0f;

		while(m_canRotate)
		{
			m_lerp += Time.deltaTime * 5.0f;

			if(m_lerp >= 1.0f)
			{
				m_lerp -= 1.0f;
			}

			transform.localEulerAngles = Vector3.Lerp (m_angle, m_finalAngle, m_lerp);

			yield return null;
		}
	}

	#endregion
}
