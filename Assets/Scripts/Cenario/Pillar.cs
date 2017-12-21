using Com.LuisPedroFonseca.ProCamera2D;
using System.Collections;
using UnityEngine;
using RGSMS.Math;

public class Pillar : Platform
{
	[SerializeField]
	private float m_upSpeed = 10.0f;
	[SerializeField]
	private float m_downSpeed = 10.0f;

	[SerializeField]
	private INTERPOLATION m_upInterpolation = INTERPOLATION.Linear;
	[SerializeField]
	private INTERPOLATION m_downInterpolation = INTERPOLATION.Linear;

	[SerializeField]
	private ParticleSystem m_playerOneParticle = null;
	[SerializeField]
	private ParticleSystem m_playerTwoParticle = null;

	private Sword m_sword = null;
	public Sword sword
	{
		get
		{
			return m_sword;
		}

		set
		{
			m_sword = value;
		}
	}

	private float m_lerp = 0.0f;

	private bool m_isActive = false;

	private bool m_isChanging = false;
	public bool isCharging
	{
		get
		{
			return m_isChanging;
		}
	}

	private Vector3 m_activePos = Constantes.VECTOR_3_ZERO;
	private Vector3 m_normalPos = Constantes.VECTOR_3_ZERO;

	private Character m_currentCharacter = null;
	public Character currentCharacter
	{
		set
		{
			if(m_currentCharacter != null)
			{
				if(value != m_currentCharacter)
				{
					m_currentCharacter.ActiveCuspe();
				}
			}

			m_currentCharacter = value;

			ActiveEffect();
		}
	}

	private void Start ()
	{
		m_normalPos = transform.localPosition;
		m_activePos = m_normalPos;
		m_activePos.y += 4.0f;
	}
		
	private void Update ()
	{
		if(PauseManager.Instance.isPaused || VictoryManager.Instance.hasEnded)
		{
			return;
		}

		if(m_currentCharacter != null && !m_isChanging)
		{
			if(m_currentCharacter.isHolding)
			{
				m_currentCharacter.Invoke ("ActiveCuspe", 0.1f);
				m_currentCharacter = null;
				return;
			}

			if(Blinding.Instance.ActionWasPressed(m_currentCharacter.joystickId))
			{
				m_isChanging = true;

				m_currentCharacter.PlayBallon();

				ProCamera2DShake.Instance.Shake(ProCamera2DShake.Instance.ShakePresets[1]);
				DeactiveParticles();

				SoundManager.Instance.PlaySFX (1);

				if (!m_isActive)
				{
					StartCoroutine (ActivePillar());
				}
				else StartCoroutine (DeactivePillar());

				return;
			}

			if(Blinding.Instance.CancelWasPressed(m_currentCharacter.joystickId))
			{
				m_currentCharacter.Invoke ("ActiveCuspe", 0.1f);
				m_currentCharacter = null;
			}
		}
	}

	private void DeactiveParticles ()
	{
		m_playerOneParticle.Stop (true);
		m_playerTwoParticle.Stop (true);
	}

	public void ActiveEffect ()
	{
		if (m_currentCharacter.joystickId == 0)
		{
			m_playerOneParticle.Play (true);
			m_playerTwoParticle.Stop (true);
		}
		else
		{
			m_playerTwoParticle.Play (true);
			m_playerOneParticle.Stop (true);
		}
	}

	private IEnumerator ActivePillar ()
	{
		m_lerp = 0.0f;
		m_isActive = true;
		m_currentCharacter.Invoke ("ActiveCuspe", 0.1f);

		while(m_lerp < 1.0f)
		{
			m_lerp += Time.deltaTime * m_upSpeed;

			transform.localPosition = MathR.Lerp (m_normalPos, m_activePos, m_lerp, m_upInterpolation);

			yield return null;
		}

		m_currentCharacter = null;
		m_isChanging = false;
	}

	private IEnumerator DeactivePillar ()
	{
		if(m_sword != null)
		{
			m_sword.SetState (new Fall());
			m_sword = null;
		}

		m_lerp = 0.0f;
		m_currentCharacter.Invoke ("ActiveCuspe", 0.1f);

		while(m_lerp < 1.0f)
		{
			m_lerp += Time.deltaTime * m_downSpeed;

			transform.localPosition = MathR.Lerp (m_activePos, m_normalPos, m_lerp, m_downInterpolation);

			yield return null;
		}

		m_isActive = false;
		m_isChanging = false;
		m_currentCharacter = null;
	}
}
