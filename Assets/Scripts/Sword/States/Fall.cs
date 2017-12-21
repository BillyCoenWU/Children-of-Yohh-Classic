using UnityEngine;
using Com.LuisPedroFonseca.ProCamera2D;

public class Fall : SFSM
{
	private Sword m_sword = null;

	public void OnTrigger (Collider2D other)
	{
		Platform platform = other.GetComponent<Platform>();

		if(platform != null)
		{
			Pillar pillar = platform.GetComponent<Pillar> ();

			if(pillar != null)
			{
				pillar.sword = m_sword;
				pillar.sword.pillar = pillar;
			}

			m_sword.EndLoopRotate();
			m_sword.particleHit.Play(true);

			if(other.bounds.Intersects(m_sword.caboCollider.bounds))
			{
				m_sword.transform.localEulerAngles = new Vector3 (0.0f, 0.0f, m_sword.transform.localEulerAngles.z + 180.0f);
			}

			m_sword.transform.SetParent (other.transform);

			if(Joystick.Instance.JoysticksCount() > 1)
			{
				Joystick.Instance.Vibrate (0.5f, 0);
				Joystick.Instance.Vibrate (0.5f, 1);

				VictoryManager.Instance.InvokeStop();
			}

			m_sword.SetState (new SIdle());
		}
	}

	public void EnterState (Sword sword)
	{
		m_sword = sword;
		m_sword.canBeHold = false;
		m_sword.character = null;
		m_sword.transform.SetParent(null);
		m_sword.Impulse();
	}

	public void ExitState ()
	{
		ProCamera2DShake.Instance.Shake(ProCamera2DShake.Instance.ShakePresets[0]);
	}

	public void FixedUpdate () {}
	public void Update () {}
}
