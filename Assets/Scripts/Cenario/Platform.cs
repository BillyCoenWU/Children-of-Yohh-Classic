using UnityEngine;

public class Platform : MonoBehaviour
{
	protected Transform m_transform = null;
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

	protected Transform m_upPoint = null;
	public Transform upPoint
	{
		get
		{
			if(m_upPoint == null)
			{
				m_upPoint = transform.GetChild(0);
			}

			return m_upPoint;
		}
	}
}
