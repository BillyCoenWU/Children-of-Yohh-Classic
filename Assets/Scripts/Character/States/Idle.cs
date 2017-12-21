using UnityEngine;

public class Idle : CFSM
{
	private Character m_character = null;

	public void EnterState (Character character)
	{
		m_character = character;
		m_character.animationController.PlayByType (ANIMATION_TYPE.IDLE);
	}

	public void Update ()
	{
		if(Blinding.Instance.JumpWasPressed(m_character.joystickId))
		{
			m_character.SetState (new Jump());
			return;
		}

		if(!Mathf.Approximately(Blinding.Instance.Direction(m_character.joystickId).x, 0.0f))
		{
			m_character.SetState (new Move());
		}
	}

	public void FixedUpdate () {}
	public void Collision (Collision2D other) {}
	public void ExitState () {}
}
