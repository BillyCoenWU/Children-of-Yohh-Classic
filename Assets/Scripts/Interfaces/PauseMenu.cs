using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : Singleton<PauseMenu>
{
	private void Awake ()
	{
		Instance = this;
		gameObject.SetActive(false);
	}

	private void Update ()
	{
		if(Blinding.Instance.StartWasPressed(PauseManager.Instance.character.joystickId))
		{
			PauseManager.Instance.Resume ();
			return;
		}
		
		if(Blinding.Instance.SelectWasPressed(PauseManager.Instance.character.joystickId))
		{
			SceneManager.LoadScene (1);
		}
	}
}
