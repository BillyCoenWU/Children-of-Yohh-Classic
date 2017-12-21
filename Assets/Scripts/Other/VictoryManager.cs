using RGSMS.Math;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Com.LuisPedroFonseca.ProCamera2D;

public class VictoryManager : Singleton<VictoryManager>
{
	[SerializeField]
	private ProCamera2DZoomToFitTargets m_zoomToFit = null;

	[SerializeField]
	private ProCamera2D m_proCamera = null;
	[SerializeField]
	private ProCamera2DShake m_cameraShake = null;

	private float m_lerp = 0.0f;

	private Character m_winner = null;

	private bool m_hasEnded = false;
	public bool hasEnded 
	{
		get
		{
			return m_hasEnded;
		}
	}

	private Camera m_camera = null;
	public Camera mainCamera
	{
		get
		{
			if(m_camera == null)
			{
				m_camera = Camera.main;
			}

			return m_camera;
		}
	}

	private Transform m_transform = null;
	public new Transform transform
	{
		get
		{
			if(m_transform == null)
			{
				m_transform = base.transform;
			}

			return m_transform;
		}
	}

	private void Awake ()
	{
		Instance = this;
	}

	public void Active (Character character)
	{
		m_hasEnded = true;

		m_proCamera.enabled = false;
		m_zoomToFit.enabled = false;
		m_cameraShake.enabled = false;

		transform.parent = null;
		m_winner = character;

		m_winner.animationController.PlayByType(ANIMATION_TYPE.IDLE);

		StartCoroutine (MoveTo());
	}

	private Vector3 m_position = Constantes.VECTOR_3_ZERO;
	private Vector3 m_endPosition = Constantes.VECTOR_3_ZERO;

	private IEnumerator MoveTo ()
	{
		while(m_lerp < 1.0f)
		{
			m_lerp += Time.deltaTime;

			m_position = transform.position;
			m_endPosition = m_winner.transform.position;
			m_endPosition.z = m_position.z;

			transform.position = MathR.Lerp (m_position, m_endPosition, m_lerp, INTERPOLATION.Linear);
			mainCamera.orthographicSize = Mathf.Lerp (5.0f, 2.0f, m_lerp);

			yield return null;
		}

		m_winner.animationController.PlayByType(ANIMATION_TYPE.IDLE);
		m_winner.PlayBallon (true);

		Invoke("Load", 2.0f);
	}

	public void InvokeStop ()
	{
		Invoke ("Stop", 1.0f);
	}

	private void Stop ()
	{
		Joystick.Instance.StopJoystickVibration (0);
		Joystick.Instance.StopJoystickVibration (1);
	}

	private void Load ()
	{
		Stop();
		PauseManager.Instance.Pause ();
		SceneManager.LoadScene(1);
	}
}
