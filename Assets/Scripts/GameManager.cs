using UnityEngine;

using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	[System.Serializable]
	public class Level
	{
		public int MaxTempo;
		public int MinTempo;
		public float TimeStep;
		public float TimeDeviance;
		public AnimationCurve levelCurve;
	}

	public enum GameState {
		NotPlaying,
		Playing,
		TitleScreen,
		EndScreen,
		WaitingToRecord,
		Recording
	}

	public GameObject Player;
	public TextMesh StatusText;
	public TextMesh StartText;
	public TextMesh ScoreText;
	public GameObject StartButton;
	public TextMesh EndText;
	AudioClip[] songs;

	public Level[] LevelCurves;
	

	Transform UpSpawn, DownSpawn;

	public Transform Spawn;
	public int currentLevelNum = 0;
	int nextLevelNum = 0;

	public ItemQueue pool;

	public float PieceSpeed;
	public float TimeStep;

	public float Tempo;

	public int MaxTempo;
	public int MinTempo;

	public int TempoThreshold;

	public float TempoMultiplierDeviance;

	int currentScore;

	float distance;

	List<GameObject> pieces = new List<GameObject>();
	
	
	float currentTime;
	public int currentStep;
	int nextTempoChangeStep;
	public int nextWarningStep;
	public int WarningTime;
	int currentCurvePoint;

	GameState currentState;

	Dictionary<GameState,GameState> stateChanges =  new Dictionary<GameState, GameState>()
	{
		{ GameState.EndScreen, GameState.TitleScreen},
		{ GameState.TitleScreen, GameState.Playing}
	};

	void Start() {
		Player.SendMessage("Wait");
		ChangeGameState(GameState.TitleScreen);
		currentScore = 0;
		ScoreText.text = currentScore.ToString();;
	}

	void StartGame() {
		currentScore = 0;
		ScoreText.text = currentScore.ToString();;
		ProceedToNextLevel();
		distance = (Player.transform.position - Spawn.position).magnitude;
		CheckTempoChange();
		EndText.gameObject.SetActive(false);
		Player.SendMessage("Initialize");
		foreach(GameObject piece in pieces) {
			pool.DestroyItem(piece);
		}
		pieces = new List<GameObject>();
		StartText.gameObject.SetActive(false);
		if(this.audio.clip != null) {
			this.audio.Play();
		}
		
		ChangeGameState(GameState.Playing);
	}
	
	void EndGame() {
		ChangeGameState(GameState.EndScreen);
		EndText.gameObject.SetActive(true);
		EndText.text = "Score: " + currentScore;
		StartButton.gameObject.SetActive(true);
		
	}
	
	void Continue() {
		ChangeGameState(stateChanges[currentState]);
	}

	void ChangeGameState(GameState newState) {

		switch(currentState) {
			case GameState.EndScreen:
				EndText.gameObject.SetActive(false);
				StartButton.SetActive(false);
				ScoreText.gameObject.SetActive(false);
				break;
		}

		switch(newState) {
			case GameState.TitleScreen:
				StartText.gameObject.SetActive(true);
				break;
			
			case GameState.Playing:
				ScoreText.gameObject.SetActive(true);
				break;
		}
		currentState = newState;

	}

	void CalculatePieceSpeed() {
		float timesPerSecond = Tempo / 60.0f;
		TimeStep = 1/timesPerSecond;
		PieceSpeed = distance * timesPerSecond;
	}



	void Update() {

		if(IsPlaying) {
			AttemptSpawn();
			CheckTempoChange();
			MovePieces();

		} else if(currentState == GameState.TitleScreen) {
			bool beginGame = false;
			if(Input.touchCount > 0) {
				if(Input.touches[0].phase == TouchPhase.Began) {
					beginGame = true;
				}
			}

			if(Input.GetKeyUp(KeyCode.Space)) {
				beginGame = true;
			}

			if(beginGame) {
				StartGame();
			}
		}

	}

	bool IsPlaying {
		get {
			return currentState == GameState.Playing;
		}
	}

	void AttemptSpawn() {

		currentTime += Time.deltaTime;

		if(currentTime >=  TimeStep) {
			currentTime = 0;
			currentStep += 1;
			GameObject item = null;

			item = GetSpawnedItem();
			if(item != null) {

				ItemController controller = item.GetComponent<ItemController>();
				controller.Initialize(PieceSpeed, Player, this);

				item.SetActive(true);
				item.SendMessage("SetSpeed", PieceSpeed);

				if(item != null) {
					pieces.Add(item);
				}
			}

		}
	}

	GameObject GetSpawnedItem() {
		GameObject item = null;

		int pieceWeight = 350;
		if(Tempo < TempoThreshold) {
			pieceWeight = 450;
		}

		int blankWeight = 1000 - (2*pieceWeight);

		int[] weights = new int[3] {pieceWeight, blankWeight, pieceWeight};
		int roll = Random.Range (0,(pieceWeight * 2) + blankWeight);
		int counter = 0;
		int index;

		for(index = 0; index < weights.Length; ++index) {
			counter += weights[index];
			if(counter > roll) {
				break;
			}
		}

		if(index - 1 != 0) {
			Vector3 spawnLoc = Spawn.position;
			item = pool.GetItem();
			spawnLoc.y = index - 1;
			spawnLoc.x -= (item.transform.localScale.x / 2);
			item.transform.position = spawnLoc;
		}

		return item;

	}

	void CheckTempoChange() {

		//Use the animation Curve to figure out what to use for the next tempo
		if(currentStep >= nextTempoChangeStep) {

			Level level = LevelCurves[currentLevelNum];
			AnimationCurve curve = level.levelCurve;

			Keyframe curveKeyFrame = curve.keys[currentCurvePoint];
			//Change the tempo and figure out when to change next

			int newTempo = (int)Mathf.Lerp(level.MinTempo,level.MaxTempo,curveKeyFrame.value);

			Tempo = newTempo;


			int nextKey = currentCurvePoint + 1;
			float diff = 0.1f;
			if(nextKey < curve.keys.Length) {
				diff = (curve.keys[nextKey].time - curveKeyFrame.time);
			}
			float newStep = (diff/0.1f)*level.TimeStep ;
			nextTempoChangeStep += (int)Random.Range (newStep - level.TimeDeviance,newStep + level.TimeDeviance);
			nextWarningStep = nextTempoChangeStep - WarningTime;
			CalculatePieceSpeed();
			foreach(GameObject piece in pieces) {
				piece.SendMessage("SetSpeed", PieceSpeed);
			}
			currentCurvePoint += 1;
			if(currentCurvePoint >= curve.keys.Length) {
				ProceedToNextLevel();
			}
		} else if(currentStep >= nextWarningStep) {
			Level level = LevelCurves[currentLevelNum];
			int lastPoint = currentCurvePoint - 1 < 0 ? level.levelCurve.keys.Length - 1 : currentCurvePoint - 1;

			int inTime = nextTempoChangeStep - currentStep;
			string status = level.levelCurve.keys[currentCurvePoint].value - level.levelCurve.keys[lastPoint].value > 0 ? "Speeding Up! " +  inTime : "Slow It Down " + inTime;
			StatusText.SendMessage("ShowStatus", status);
		}

	}

	void ProceedToNextLevel() {
		currentLevelNum = nextLevelNum;
		nextLevelNum = Random.Range (0,LevelCurves.Length);
		currentTime = 0.0f;
		currentStep = 0;
		nextTempoChangeStep = 0;
		currentCurvePoint = 0;
	}

	void DestroyPiece(GameObject piece) {
		pieces.Remove(piece);
		pool.DestroyItem(piece);
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

	public void ScorePiece() {
		currentScore += 1;
		ScoreText.text = currentScore.ToString();
	}
}
