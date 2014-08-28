using UnityEngine;
using System.Collections;

public class VisualController : MonoBehaviour {

	public DuckController.Actions MyState;

	// Use this for initialization
	void SetState(DuckController.Actions currentState) {
		bool shouldSetActive = false;
		if(currentState == MyState) {
			shouldSetActive = true;
		} 

		renderer.enabled = shouldSetActive;
		collider2D.enabled = shouldSetActive;
	}
}
