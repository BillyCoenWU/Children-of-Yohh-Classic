using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T s_instance = null;
	public static T Instance
	{
		get { return s_instance; }
		set
		{
			if(s_instance != null)
			{
				Destroy(s_instance);
			}

			s_instance = value;
		}
	}
}
