using UnityEngine;

public class InitGame : MonoBehaviour
{
	private void LateUpdate ()
	{
		if(Blinding.Instance.StartWasPressed(0))
		{
			PauseManager.Instance.Resume();
			this.gameObject.SetActive(false);
		}
	}
}
