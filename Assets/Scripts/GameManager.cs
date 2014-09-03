using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public enum GameState {
		NotPlaying,
		Playing,
		WaitingToRecord,
		Recording
	}

	public GameObject Player;
	TextAsset[] levels;
	Dictionary<string,TextAsset> levelDict = new Dictionary<string, TextAsset>();
	string[] currentLevel;
	int currentLevelNum = 1;
	public Transform UpSpawn, DownSpawn;
	public ItemQueue pool;

	public float PieceSpeed;
	public float TimeStep;

	float distance;

	List<GameObject> pieces = new List<GameObject>();
	
	
	float currentTime;
	int currentStep;

	GameState currentState;

	void Start() {
		levels = Resources.LoadAll<TextAsset>("Levels");
		if(levels.Length > 0) {
			foreach(TextAsset ta in levels) {
				levelDict.Add(ta.name, ta);
			}
		}
		ChangeGameState(GameState.NotPlaying);
	}

	void LoadLevel(int levelNum) {
		string levelText = levelDict["Level"+levelNum].text;
		string[] levelPieces = levelText.Split('\n');
		string[] configOptions = levelPieces[0].Split(' ');
		TimeStep = float.Parse(configOptions[0]);
		PieceSpeed = float.Parse(configOptions[1]);
		currentLevel = levelPieces[1].Split(' ');
	}
	
	void ChangeGameState(GameState newState) {
		currentState = newState;
	}

	void OnGUI() {
		if(currentState == GameState.NotPlaying) {
			if(GUI.Button(new Rect((Screen.width / 2) - 50, (Screen.height/2)- 25, 100, 50), "Start")) {
				ChangeGameState(GameState.Playing);
				currentTime = 0.0f;
				currentStep = 0;
				foreach(GameObject piece in pieces) {
					pool.DestroyItem(piece);
				}
				pieces = new List<GameObject>();
				LoadLevel (currentLevelNum);
			}
		}
	}

	void Update() {
		if(IsPlaying) {
			AttemptSpawn();
			MovePieces();
		}
	}

	bool IsPlaying {
		get {
			return currentState == GameState.Playing;
		}
	}

	void AttemptSpawn() {
		bool stepChange = false;
		currentTime += Time.deltaTime;
		if(currentStep >= currentLevel.Length) {
			currentStep = 0;
		}
		string currentItem = currentLevel[currentStep];

		float itemPos = float.Parse(currentItem);
		if(currentTime >=  Mathf.Abs(itemPos) * TimeStep) {
			stepChange = true;
			currentTime = 0;
		}
		if(stepChange) {
			GameObject item = null;
			Debug.Log (itemPos);
			Vector3 spawnLoc = itemPos > 0 ? UpSpawn.position : DownSpawn.position;
			item = pool.GetItem();
			spawnLoc.x -= (item.transform.localScale.x / 2);
			item.transform.position = spawnLoc;
			item.SetActive(true);
			item.SendMessage("SetSpeed", PieceSpeed);

			if(item != null) {
				pieces.Add(item);
			}
			currentStep += 1;
		}
	}

	void MovePieces() {
		List<GameObject> garbageList = null;
		foreach(GameObject piece in pieces) {
			piece.SendMessage("Move");
			if(piece.transform.position.x < 0) {
				pool.DestroyItem(piece);
				if(garbageList == null) {
					garbageList = new List<GameObject>();
				}
				garbageList.Add(piece);
			}
		}

		if(garbageList != null) {
			foreach(GameObject piece in garbageList) {
				pieces.Remove(piece);
			}
		}
	}
}
