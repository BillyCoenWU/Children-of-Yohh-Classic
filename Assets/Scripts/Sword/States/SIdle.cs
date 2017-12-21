using UnityEngine;

public class SIdle : SFSM
{
	private Sword m_sword = null;

	public void OnTrigger (Collider2D other)
	{
		
	}

	public void EnterState (Sword sword)
	{
		m_sword = sword;

		m_sword.canBeHold = true;
		m_sword._rigidbody2D.bodyType = RigidbodyType2D.Static;
	}

	public void FixedUpdate ()
	{
		
	}

	public void ExitState ()
	{
		
	}

	public void Update ()
	{
		
	}
}
