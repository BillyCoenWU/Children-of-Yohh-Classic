using UnityEngine;

[System.Serializable]
public class SoundData
{
	public string name = string.Empty;

	public AudioClip sound = null;

	[Range(0.0f, 1.0f)]
	public float volume = 1.0f;
}

public class SoundManager : Singleton<SoundManager>
{
	[SerializeField]
	private SoundData[] m_sfxs = null;

	[SerializeField]
	private SoundData m_bmg = null;

	[SerializeField]
	private AudioSource[] m_sfxSources = null;

	[SerializeField]
	private AudioSource m_bmgSource = null;

	private void Awake ()
	{
		Instance = this;
	}

	public void PlayBMG ()
	{
		m_bmgSource.clip = m_bmg.sound;
		m_bmgSource.Play();
	}

	public void PlaySFX (int id)
	{
		AudioSource source = null;

		for(int i = 0; i < m_sfxSources.Length; i++)
		{
			if(!m_sfxSources[i].isPlaying)
			{
				source = m_sfxSources[i];
				break;
			}
		}

		if(source == null)
		{
			source = m_sfxSources[0];
		}

		source.volume = 1.0f;

		if(id == 6)
		{
			source.volume = 0.5f;
		}

		source.clip = m_sfxs[id].sound;
		source.Play();
	}

	public void PlaySFX (string soundName)
	{
		AudioSource source = null;

		for(int i = 0; i < m_sfxSources.Length; i++)
		{
			if(!m_sfxSources[i].isPlaying)
			{
				source = m_sfxSources[i];
				break;
			}
		}

		if(source == null)
		{
			source = m_sfxSources[0];
		}

		source.volume = 0.5f;

		for(int i = 0; i < m_sfxs.Length; i++)
		{
			if(m_sfxs[i].name == soundName)
			{
				source.clip = m_sfxs[i].sound;
				source.Play();
				return;
			}
		}
	}
}
