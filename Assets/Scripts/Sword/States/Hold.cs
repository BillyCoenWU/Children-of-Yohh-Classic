using UnityEngine;

public class Hold : SFSM
{
	private Sword m_sword = null;

	private float m_time = 0.0f;

	public void OnTrigger (Collider2D other) {}

	public void EnterState (Sword sword)
	{
		m_sword = sword;

		if(m_sword.pillar != null)
		{
			m_sword.pillar.sword = null;
			m_sword.pillar = null;
		}

		m_sword.onHoldState = true;
		m_sword._rigidbody2D.bodyType = RigidbodyType2D.Kinematic;

		m_sword.SetHold ();
	}

	public void FixedUpdate ()
	{
		
	}

	public void ExitState ()
	{
		Joystick.Instance.StopJoystickVibration (m_sword.character.joystickId);
		m_sword.onHoldState = false;
	}

	public void Update ()
	{
		m_time += Time.deltaTime;

		Joystick.Instance.Vibrate (m_time, m_sword.character.joystickId);

		if(m_time >= 3.0f)
		{
//			Joystick.Instance.StopJoystickVibration (m_sword.character.joystickId);
			m_sword.character.Invoke ("Die", 0.1f);
			m_sword.SetState(new Fall());
		}
	}
}
