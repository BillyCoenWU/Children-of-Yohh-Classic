using UnityEngine;

public class ExplosionThrow : CFSM
{
	private Character m_character = null;

	public void EnterState (Character character)
	{
		m_character = character;

		m_character._rigidbody2D.AddForce(new Vector2(m_character.joystickId == 1 ? 15.0f : -15.0f, 5.0f), ForceMode2D.Impulse);
	}

	public void Collision (Collision2D other)
	{
		Platform platform = other.gameObject.GetComponent<Platform>();

		if(platform != null)
		{
			m_character.SetState (new Idle());
		}
	}

	public void FixedUpdate () {}
	public void ExitState () {}
	public void Update () {}
}
