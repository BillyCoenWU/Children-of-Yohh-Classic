using System.Collections;
using UnityEngine;
using RGSMS.Math;

public class Balao : MonoBehaviour
{
	private float m_lerp = 0.0f;

	private Vector3 m_scale = Constantes.VECTOR_3_ZERO;

	[SerializeField]
	private Character m_character = null;

	private Transform m_text = null;
	public Transform text
	{
		get
		{
			if(m_text == null)
			{
				m_text = transform.GetChild(0);
			}

			return m_text;
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

	private IEnumerator Lerp (bool win)
	{
		m_lerp = 0.0f;

		while(m_lerp < 1.0f)
		{
			m_lerp += Time.deltaTime * 15.0f;

			transform.localScale = MathR.Lerp (Constantes.VECTOR_3_ZERO, m_scale, m_lerp, INTERPOLATION.ExponentialEaseIn);

			yield return null;
		}

		if(!win)
		{
			Invoke ("Deactive", 0.25f);
		}
	}

	public void Active (bool win)
	{
		CancelInvoke();

		m_scale = new Vector3(m_character.animationController.spriteRenderer.flipX ? -1.0f : 1.0f, 1.0f, 1.0f);
		transform.localScale = Constantes.VECTOR_3_ZERO;
		text.localScale = new Vector3(m_scale.x < 0.0f ? -0.75f : 0.75f, 1.0f, 1.0f);

		gameObject.SetActive (true);

		StartCoroutine (Lerp(win));
	}

	private void Deactive ()
	{
		gameObject.SetActive (false);
	}
}
