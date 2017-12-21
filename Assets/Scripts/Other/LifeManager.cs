using UnityEngine;

[System.Serializable]
public class LifeManager
{
	#region Variables & Properties & Delegates
	
	public int life = 0;
	public int maxLife = 0;

	private bool m_takeDamage = true;
	public bool canTakeDamage
	{
		get
		{
			return m_takeDamage;
		}
	}

	public bool IsAlive
	{
		get
		{
			return life > 0;
		}
	}

	public bool IsDead
	{
		get
		{
			return life == 0;
		}
	}

	public bool IsDamaged
	{
		get
		{
			return life < maxLife;
		}
	}

	public float DecimalPercent
	{
		get
		{
			return Mathf.InverseLerp(0, maxLife, life);
		}
	}

	public int percentLife
	{
		get
		{
			return (life * 100) / maxLife;
		}
	}

	public delegate void Death();
	public event Death death;

	public delegate void LifeUpdate();
	public event LifeUpdate lifeUpate;

	#endregion

	#region Instance

	public LifeManager (int maxLife, bool canTakeDamage = true)
	{
		this.maxLife = maxLife;
		this.m_takeDamage = canTakeDamage;
	}

	#endregion

	#region Life Methods

	public void TakeDamage (bool take)
	{
		m_takeDamage = take;
	}

	public void AddLife (int plusLife)
	{
		life = Mathf.Clamp(life + plusLife, life, maxLife);

		if(lifeUpate != null)
		{
			lifeUpate();
		}
	}

	public void RemoveLife (int damage)
	{
		if(!m_takeDamage)
		{
			return;
		}

		damage = Mathf.Clamp(damage, 0, damage);

		life = Mathf.Clamp(life - damage, 0, maxLife);

		if(lifeUpate != null)
		{
			lifeUpate();
		}

		if(!IsAlive)
		{
			if(death != null)
			{
				death();
			}
		}
	}

	public void ResetLife ()
	{
		life = maxLife;

		if(lifeUpate != null)
		{
			lifeUpate();
		}
	}

	#endregion
}
