using UnityEngine;
using System.Collections;

public class ItemController : MonoBehaviour {

	public float Speed;

	public GameObject player;

	public GameManager manager;

	bool scored = false;

	public void SetSpeed(float speed) {
		this.Speed = speed;
	}

	public void Initialize(float speed, GameObject player, GameManager manager) {
		SetSpeed (speed);
		this.player = player;
		this.manager = manager;
		scored = false;
	}

	void Move() {
		Vector3 pieceLoc = transform.position;
		pieceLoc.x -= Speed * Time.deltaTime;
		transform.position = pieceLoc;
		if(!scored && (player.transform.position.x > transform.position.x)) {

			manager.ScorePiece();
			scored = true;
		}
	}
}
