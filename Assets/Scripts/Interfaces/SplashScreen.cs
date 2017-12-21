using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
	[SerializeField]
	private Image m_image = null;

	private Color m_color = Color.white;

	private void Start ()
	{
		StartCoroutine(Alpha());
	}

	private IEnumerator Alpha ()
	{
		float lerp = 0.0f;

		while(lerp < 1.0f)
		{
			lerp += Time.deltaTime;

			m_color = m_image.color;
			m_color.a = lerp;
			m_image.color = m_color;

			yield return null;
		}

		Invoke ("Load", 2.0f);
	}

	private void Load ()
	{
		SoundManager.Instance.PlayBMG ();
		PauseManager.Instance.Pause();
		SceneManager.LoadScene(1);
	}
}
