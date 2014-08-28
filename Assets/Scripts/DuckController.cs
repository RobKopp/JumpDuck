using UnityEngine;
using System.Collections;

public class DuckController : MonoBehaviour {

	public enum Actions {
		Jump,
		Duck,
		Walking
	}

	Actions currentState;
	bool endingAction = false;
	public float Hangtime;
	float currentHangtime;


	void Start() {
		currentState = Actions.Walking;
		SetVisuals(currentState);
	}

	void StartAction (Actions startingAction) {
		currentState = startingAction;
		SetVisuals(currentState);
		endingAction = false;
	}

	void ActivateHangtime() {
		endingAction = true;
		currentHangtime = Hangtime;
	}

	void EndAction() {
		currentState = Actions.Walking;
		SetVisuals(currentState);
	}

	void SetVisuals(Actions state) {
		BroadcastMessage("SetState",state);
	}
	
	// Update is called once per frame
	void Update () {
#if UNITY_EDITOR
		KeyCode jumpKey = KeyCode.F;
		KeyCode duckKey = KeyCode.J;
		if(Input.GetKeyUp(jumpKey) && currentState == Actions.Jump) {
			ActivateHangtime();
		}
		if(Input.GetKeyUp(duckKey) && currentState == Actions.Duck) {
			ActivateHangtime();
		}

		if(Input.GetKeyDown(jumpKey) && currentState != Actions.Jump) {
			StartAction(Actions.Jump);
		} else if(Input.GetKeyDown(duckKey) && currentState != Actions.Duck) {
			StartAction(Actions.Duck);
		}
#endif
		if(endingAction) {
			currentHangtime -= Time.deltaTime;
			if(currentHangtime <= 0){
				endingAction = false;
				EndAction();
			}
		}
	}
}
