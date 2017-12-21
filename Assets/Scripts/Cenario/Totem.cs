using UnityEngine;
using Com.LuisPedroFonseca.ProCamera2D;

public class Totem : Platform
{
	[SerializeField]
	private ParticleSystem m_explosion = null;

	private Sword m_sword = null;
	public Sword sword
	{
		set
		{
			m_sword = value;	
		}
	}

	private LifeManager m_lifeManager = null;
	public LifeManager lifeManager
	{
		get
		{
			return m_lifeManager;
		}
	}

	[SerializeField]
	private Transform m_spawn = null;
	public Transform spawn
	{
		get
		{
			return m_spawn;
		}
	}

	private void Start ()
	{
		m_lifeManager = new LifeManager(3);
		m_lifeManager.ResetLife();
		m_lifeManager.lifeUpate += LostLife;
		m_lifeManager.death += Death;
	}

	private void LostLife ()
	{
		ProCamera2DShake.Instance.Shake(ProCamera2DShake.Instance.ShakePresets[1]);

		Joystick.Instance.Vibrate (0.5f, m_sword.character.joystickId);

		if(lifeManager.IsAlive)
		{
			m_sword.character.SetState(new ExplosionThrow());
		}

		Invoke ("StopVibrate", 1f);
	}

	private void Death ()
	{
		m_explosion.Play (true);

		SoundManager.Instance.PlaySFX (4);

		Joystick.Instance.Vibrate (2.0f, m_sword.character.joystickId);

		ProCamera2DShake.Instance.Shake(ProCamera2DShake.Instance.ShakePresets[2]);

		VictoryManager.Instance.Active (m_sword.character);

		Invoke ("StopVibrate", 1f);
	}

	private void StopVibrate ()
	{
		Joystick.Instance.StopJoystickVibration (m_sword.character.joystickId);
	}
}
