using UnityEngine;
using System.Collections;

public class DuckController : MonoBehaviour {

	public enum Actions {
		Jump,
		Duck,
		Walking
	}

	Actions currentState;


	void Start() {
		currentState = Actions.Walking;
		SetVisuals(currentState);
	}

	void StartAction (Actions startingAction) {
		currentState = startingAction;
		SetVisuals(currentState);
	}

	void EndAction(Actions endingAction) {
		currentState = Actions.Walking;
		SetVisuals(currentState);
	}

	void SetVisuals(Actions state) {
		BroadcastMessage("SetState",state);
	}
	
	// Update is called once per frame
	void Update () {
		KeyCode jumpKey = KeyCode.F;
		KeyCode duckKey = KeyCode.J;
#if UNITY_EDITOR
		if(Input.GetKeyUp(jumpKey) && currentState == Actions.Jump) {
		
			EndAction(Actions.Jump);
		}
		if(Input.GetKeyUp(duckKey) && currentState == Actions.Duck) {
			EndAction(Actions.Duck);
		}

		if(Input.GetKeyDown(jumpKey)) {
			StartAction(Actions.Jump);
		} else if(Input.GetKeyDown(duckKey)) {
			StartAction(Actions.Duck);
		}
#endif
	}
}
