using UnityEngine;

public interface CFSM
{
	void EnterState(Character character);
	void Collision(Collision2D other);
	void FixedUpdate();
	void ExitState();
	void Update();
}
