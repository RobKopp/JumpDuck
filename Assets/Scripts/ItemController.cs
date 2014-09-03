using UnityEngine;
using System.Collections;

public class ItemController : MonoBehaviour {

	public float Speed;

	// Use this for initialization
	void OnTriggerEnter2D(Collider2D collider) {
		GameObject.FindGameObjectWithTag("GameController").SendMessage("ChangeGameState", GameManager.GameState.NotPlaying);
	}

	void SetSpeed(float speed) {
		this.Speed = speed;
	}

	void Move() {
		Vector3 pieceLoc = transform.position;
		pieceLoc.x -= Speed * Time.deltaTime;
		transform.position = pieceLoc;
	}
}
