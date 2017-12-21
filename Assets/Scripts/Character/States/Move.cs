using UnityEngine;

public class Move : CFSM
{
	private Character m_character = null;

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
		m_character.animationController.PlayByType (ANIMATION_TYPE.WALK);
	}

	public void FixedUpdate ()
	{


		m_character.transform.position += Constantes.VECTOR_3_RIGHT * (Blinding.Instance.Direction(m_character.joystickId).x * realSpeed * Time.fixedDeltaTime);
	}

	public void ExitState ()
	{
		
	}

	public void Collision (Collision2D other) {}

	public void Update ()
	{
		m_character.SetFlip();

		if(Blinding.Instance.JumpWasPressed(m_character.joystickId))
		{
			m_character.SetState (new Jump());
			return;
		}

		if(Mathf.Approximately(Blinding.Instance.Direction(m_character.joystickId).x, 0.0f))
		{
			m_character.SetState (new Idle());
		}
	}
}
