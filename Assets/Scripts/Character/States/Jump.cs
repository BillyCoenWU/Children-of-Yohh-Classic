using UnityEngine;

public class Jump : CFSM
{
	private Character m_character = null;

	private bool m_canJump = true;

	private float realSpeed
	{
		get
		{
			return m_character.isHolding ? 1.0f : m_character.speed;
		}
	}

	public void EnterState (Character character)
	{
		m_character = character;
		m_character.animationController.PlayByType (ANIMATION_TYPE.JUMP);

		Impulse();
	}

	public void FixedUpdate ()
	{
		m_character.transform.position += Constantes.VECTOR_3_RIGHT * (Blinding.Instance.Direction(m_character.joystickId).x * realSpeed * Time.fixedDeltaTime);
	}

	public void ExitState ()
	{
		
	}

	public void Update ()
	{
		m_character.SetFlip();

		if (m_canJump)
		{
			if(Blinding.Instance.JumpWasPressed(m_character.joystickId))
			{
				Impulse ();
				m_character.animationController.PlayByType (ANIMATION_TYPE.JUMP);
				m_canJump = false;
			}
		}
	}

	public void Collision (Collision2D other)
	{
		Platform platform = other.gameObject.GetComponent<Platform> ();

		if(platform != null)
		{
			SoundManager.Instance.PlaySFX (5);

			if(m_character.transform.localPosition.y >= platform.upPoint.position.y)
			{
				if(!Mathf.Approximately(Blinding.Instance.Direction(m_character.joystickId).x, 0.0f))
				{
					m_character.SetState (new Move());
				}
				else m_character.SetState (new Idle());
			}
		}
	}

	public void Impulse ()
	{
//		SoundManager.Instance.PlaySFX (6);
		m_character._rigidbody2D.velocity = Constantes.VECTOR_2_ZERO;
		m_character._rigidbody2D.AddForce (Constantes.IMPULSE, ForceMode2D.Impulse);
	}
}
