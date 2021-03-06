﻿using UnityEngine;
using System.Collections.Generic;

public class DuckController : MonoBehaviour {

	public enum Actions {
		Jump,
		Duck,
		Walking,
		Waiting,
		Dead
	}

	public bool GodMode;

	public GameObject GameManager;

	public Actions currentState;
	
	public float JumpSpeed;
	public float Height;

	void OnTriggerEnter2D(Collider2D collider) {
		if(!GodMode) {
			GameObject item = collider.gameObject;
			if(item.tag == "Item") {
				GameManager.SendMessage("EndGame");
				StartAction(Actions.Dead);
				gameObject.particleSystem.Play();
			} else if(item.tag == "GoldPiece"){
				GameManager.SendMessage("CountScore",1);
				GameManager.SendMessage("DestroyPiece",item);
			}
		}
	}

	Dictionary<Actions,string> actionMap =  new Dictionary<Actions, string>()
	{
		{ Actions.Jump, "JumpState"},
		{ Actions.Duck, "DuckState"},
		{Actions.Walking, "WalkingState"},
		{Actions.Waiting, "WalkingState"},
		{Actions.Dead, "DeadState"}
	};

	void Wait() {
		currentState = Actions.Waiting;
		SetVisuals(currentState);
	}

	void Initialize() {
		currentState = Actions.Walking;
		SetVisuals(currentState);
	}

	void StartAction (Actions startingAction) {
		currentState = startingAction;
		SetVisuals(currentState);
	}

	void EndAction() {
		currentState = Actions.Walking;
		SetVisuals(currentState);
	}

	void SetVisuals(Actions state) {
		string actionName = actionMap[currentState];
		foreach(Transform child in transform) {
			child.gameObject.SetActive(child.name == actionName);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(currentState != Actions.Dead && currentState != Actions.Waiting) {
	#if UNITY_EDITOR
			KeyCode jumpKey = KeyCode.F;
			KeyCode duckKey = KeyCode.J;
			if(Input.GetKeyDown(jumpKey) && currentState != Actions.Jump) {
				StartAction(Actions.Jump);
			}
			if(Input.GetKeyDown(duckKey) && currentState != Actions.Duck) {
				StartAction(Actions.Duck);
			}

			if(Input.GetKeyUp(jumpKey) && currentState == Actions.Jump) {
				EndAction();
			} else if(Input.GetKeyUp(duckKey) && currentState == Actions.Duck) {
				EndAction();
			}

	#else
			if(Input.touchCount > 0) {
				foreach(Touch touch in Input.touches) {
					if(touch.phase == TouchPhase.Began) {
						if(touch.position.x > Screen.width /2){
							StartAction(Actions.Duck);
						} else {
							StartAction(Actions.Jump);
						}
						//If its ending and its the last touch, end the action
					} else if((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && Input.touchCount <= 1) {
						EndAction();
					}
				}
			}
	#endif
		}

	}
}
