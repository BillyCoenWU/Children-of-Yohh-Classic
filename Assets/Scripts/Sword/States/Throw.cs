using UnityEngine;
using Com.LuisPedroFonseca.ProCamera2D;

public class Throw : SFSM
{
	private Sword m_sword = null;

	public void OnTrigger (Collider2D other)
	{
		Character character = other.GetComponent<Character>();

		if(character != null)
		{
			if(character != m_sword.character)
			{
				m_sword.particleHit.Play(true);
				ProCamera2DShake.Instance.Shake(ProCamera2DShake.Instance.ShakePresets[3]);
				character.lifeManager.RemoveLife(1);
			}
		}

		Pillar pillar = other.GetComponent<Pillar>();

		if(pillar != null)
		{
			pillar.sword = m_sword;
			m_sword.pillar = pillar;
			m_sword.particleHit.Play(true);
			ProCamera2DShake.Instance.Shake(ProCamera2DShake.Instance.ShakePresets[1]);
			m_sword.SetState (new SIdle());
			m_sword.transform.SetParent(pillar.transform);
			if(m_sword.character != null)
			{
				Joystick.Instance.Vibrate (0.5f, m_sword.character.joystickId);
			}
			VictoryManager.Instance.InvokeStop();
			return;
		}

		Totem totem = other.GetComponent<Totem>();

		if(totem != null)
		{
			if(totem != m_sword.character.totem)
			{
				totem.sword = m_sword;
				m_sword.particleHit.Play(true);
				totem.lifeManager.RemoveLife(1);
				m_sword.transform.SetParent(totem.transform);
			}

			m_sword.SetState (new SIdle());
		}
	}

	public void EnterState (Sword sword)
	{
		m_sword = sword;

		SoundManager.Instance.PlaySFX (0);

		m_sword._rigidbody2D.bodyType = RigidbodyType2D.Dynamic;

		m_sword.SetThrowAngle();
	}

	public void FixedUpdate ()
	{
		
	}

	public void ExitState ()
	{
	}

	public void Update ()
	{
		
	}
}
