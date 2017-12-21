using UnityEngine;

public class PauseManager
{
	private bool m_pause = false;
	public bool isPaused
	{
		get { return m_pause; }
	}

	private Character m_character = null;
	public Character character
	{
		get { return m_character; }
		set { m_character = value; }
	}

	private static PauseManager s_instance = null;
	public static PauseManager Instance
	{
		get
		{
			if(s_instance == null)
			{
				s_instance = new PauseManager();
			}

			return s_instance;
		}
	}

	public void Pause ()
	{
		m_pause = true;
		Time.timeScale = 0.0f;

		if(PauseMenu.Instance != null)
		{
			if(!VictoryManager.Instance.hasEnded)
			{
				PauseMenu.Instance.gameObject.SetActive (true);
			}
		}
	}

	public void Resume ()
	{
		m_pause = false;
		Time.timeScale = 1.0f;

		if(PauseMenu.Instance != null)
		{
			PauseMenu.Instance.gameObject.SetActive (false);
		}
	}
}
