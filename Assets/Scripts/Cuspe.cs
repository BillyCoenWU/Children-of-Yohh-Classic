using UnityEngine;

public class Cuspe : MonoBehaviour
{
	[SerializeField]
	private Vector2 m_impulse = Constantes.VECTOR_2_ZERO;

	[SerializeField]
	private ParticleSystem m_particle = null;

	[SerializeField]
	private Rigidbody2D m_rigidbody2D = null;

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

	private Character m_character = null;
	public Character character
	{
		set
		{
			m_character = value;
		}
	}

	public void Active (bool facingRight, float axisY)
	{
		m_impulse.x = facingRight ? Mathf.Abs(m_impulse.x) : (Mathf.Abs(m_impulse.x) * -1.0f);

		transform.localPosition = m_character.boca.position;
	
		gameObject.SetActive(true);

		m_rigidbody2D.AddForce (axisY < -0.5f ? new Vector2(0.0f, -5.0f) : m_impulse, ForceMode2D.Impulse);
	}

	private void OnTriggerEnter2D (Collider2D other)
	{
		Pillar pillar = other.GetComponent<Pillar>();

		SoundManager.Instance.PlaySFX (11);

		if (pillar != null)
		{
			if (!pillar.isCharging)
			{
				pillar.currentCharacter = m_character;
				pillar.ActiveEffect();
			}
			else m_character.ActiveCuspe();
		}
		else m_character.ActiveCuspe();

		m_particle.transform.position = transform.position;
		m_particle.Play (true);

		this.gameObject.SetActive(false);
	}
}
