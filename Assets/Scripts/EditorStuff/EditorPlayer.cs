using UnityEngine;
using System.Collections;
using System.Linq;

public class EditorPlayer : MonoBehaviour {

	public EditorControls editorController;
	GameManager.GameState currentState = GameManager.GameState.NotPlaying;
	GameObject recordingHudHolder;
	string recordingLevelName = "TempRecording";
	float currentRecordingTime = 0;
	// Use this for initialization
	void Start () {
		//Need to homogonize distance based off of speed
		GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
		foreach(GameObject item in items) 
		{
			item.GetComponent<ItemController>().Speed = editorController.PieceSpeed;	
		}
	
	}

	void ChangeGameState(GameManager.GameState newState) {
		if(currentState != GameManager.GameState.Recording) {
			currentState = newState;
		}
	}

	void OnGUI() {
		if(currentState == GameManager.GameState.NotPlaying) {
			if(GUI.Button(new Rect((Screen.width / 2) - 100, (Screen.height/2)- 150, 100, 50), "Test")) {
				if(this.audio.clip != null) {
					this.audio.Play();
				}
				ChangeGameState(GameManager.GameState.Playing);
			}

			if(GUI.Button(new Rect((Screen.width / 2) + 100, (Screen.height/2)- 150, 100, 50), "Record")) {
				BeginRecording();
				ChangeGameState(GameManager.GameState.WaitingToRecord);
			}
		} else if(currentState == GameManager.GameState.WaitingToRecord) {
			GUI.Label(new Rect((Screen.width / 2), (Screen.height/2) - 100, 100, 50), "Press Space To Start...");
		}
	}

	void BeginRecording() {
		GameObject[] oldItems = GameObject.FindGameObjectsWithTag("Item");
		foreach(GameObject item in oldItems) {
			Destroy(item);	
		}
		recordingHudHolder = new GameObject();
		recordingHudHolder.transform.position = Vector3.zero;
		for(int i = 0; i < 100; ++i) {
			Vector3 tickPos = new Vector3(i * (editorController.PieceSpeed * editorController.TimeStep), 5, 0);
			GameObject tickMark = Instantiate(editorController.item, tickPos, Quaternion.identity) as GameObject;
			tickMark.GetComponent<ItemController>().Speed = editorController.PieceSpeed;
			tickMark.transform.parent = recordingHudHolder.transform;
		}
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		player.transform.position = Vector3.zero;
	}

	void EndRecording() {
		//We need to save out the level so we can load it again when we leave recording mode
		DestroyImmediate(recordingHudHolder);

		correctPlacement();
		editorController.ExportLevel(recordingLevelName);
		currentState = GameManager.GameState.NotPlaying;
	}

	void correctPlacement() {
		GameObject[] children = GameObject.FindGameObjectsWithTag("Item");
		foreach(GameObject item in children.OrderBy(child => child.transform.position.x)) {
			Vector3 itemPos = item.transform.position;
			itemPos.x += currentRecordingTime * (editorController.PieceSpeed * editorController.TimeStep);
			item.transform.position = itemPos;
		}
	}

	void Update() {
		if(currentState == GameManager.GameState.Playing) {
			GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
			foreach(GameObject item in items) 
			{
				item.SendMessage("Move");	
			}
		}else if(currentState == GameManager.GameState.WaitingToRecord) {
			if(Input.GetKeyDown(KeyCode.Space)) {
				ChangeGameState(GameManager.GameState.Recording);
				if(this.audio.clip != null) {
					this.audio.Play();
				}
			}

		} else if(currentState == GameManager.GameState.Recording) {
			currentRecordingTime += Time.deltaTime;
			GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
			foreach(GameObject item in items) 
			{
				item.SendMessage("Move");	
			}


			GameObject newItem = null;
			if(Input.GetKeyDown(KeyCode.F)) {
				newItem = Instantiate(editorController.item, Vector3.up, Quaternion.identity) as GameObject;

			} else if(Input.GetKeyDown(KeyCode.J)) {
				newItem = Instantiate(editorController.item, Vector3.down, Quaternion.identity) as GameObject;
			}

			if(newItem != null) {
				newItem.GetComponent<ItemController>().Speed = editorController.PieceSpeed;
			}

			if(Input.GetKeyDown(KeyCode.Space)) {
				EndRecording();
			}
		}

	}
}
