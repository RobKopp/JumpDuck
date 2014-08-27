using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	TextAsset[] levels;
	string[] currentLevel;
	public Transform UpSpawn, DownSpawn;
	public ItemQueue pool;

	public float Tempo;
	public float PieceSpeed;

	List<GameObject> pieces = new List<GameObject>();
	

	bool isPlaying = false;
	float currentTime;
	int currentStep;
	int currentSpawnPeriod;

	void Start() {
		levels = Resources.LoadAll<TextAsset>("Levels");
		if(levels.Length > 0) {
			currentLevel = levels[0].text.Split(' ');
		}
		isPlaying = true;
		currentTime = 0.0f;
		currentStep = 0;
		currentSpawnPeriod = 0;
		
	}

	void Update() {
		if(isPlaying) {
			AttemptSpawn();
			MovePieces();
		}
	}

	void AttemptSpawn() {
		bool stepChange = false;
		currentTime += Time.deltaTime;
		//If we did a beat then update the step
		if(currentTime > Tempo) {
			stepChange = true;
			currentTime = 0;
		}
		if(stepChange) {
			if(currentSpawnPeriod == 0) {

				GameObject item = null;
				if(currentStep >= currentLevel.Length) {
					currentStep = 0;
				}
				string[] items = currentLevel[currentStep].Split(':');

				//If there is this character then its a rest
				int spawnTime = 0;
				string text = "Wait: ";
				if(items.Length == 1)
				{
					Vector3 spawnLoc = int.Parse(items[0]) > 0 ? UpSpawn.position : DownSpawn.position;
					item = pool.GetItem();
					item.transform.position = spawnLoc;
					item.SetActive(true);
					spawnTime = Mathf.Abs(int.Parse(items[0]));
					text = "Spawn: "; 
				} else {
					spawnTime = Mathf.Abs(int.Parse(items[1]));
					text = "Rest: ";
				}

				currentSpawnPeriod = spawnTime;
				if(item != null) {
					pieces.Add(item);
				}
				currentStep += 1;
			}
			if(currentSpawnPeriod > 0) {
				currentSpawnPeriod -= 1;
			}
		}
	}

	void MovePieces() {
		List<GameObject> garbageList = null;
		foreach(GameObject piece in pieces) {
			Vector3 pieceLoc = piece.transform.position;
			pieceLoc.x -= PieceSpeed * Time.deltaTime;
			piece.transform.position = pieceLoc;
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
