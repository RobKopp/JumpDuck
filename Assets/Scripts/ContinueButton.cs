using UnityEngine;
using System.Collections;

public class ContinueButton : MonoBehaviour {

	// Use this for initialization
	void OnMouseUpAsButton() {
		GameObject.FindGameObjectWithTag("GameController").SendMessage("Continue");
	}
}
