using UnityEngine;
using System.Collections;

public class ItemController : MonoBehaviour {

	// Use this for initialization
	void OnTriggerEnter2D(Collider2D collider) {
		GameObject.FindGameObjectWithTag("GameController").SendMessage("ChangeGameState", GameManager.GameState.NotPlaying);
	}
}
