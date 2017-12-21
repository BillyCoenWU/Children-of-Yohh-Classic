using UnityEngine;

public interface SFSM
{
	void OnTrigger (Collider2D other);
	void EnterState (Sword sword);
	void FixedUpdate ();
	void ExitState ();
	void Update ();
}
