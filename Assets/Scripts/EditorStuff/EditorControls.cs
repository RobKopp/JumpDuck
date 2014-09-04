using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor;

[ExecuteInEditMode]
public class EditorControls : MonoBehaviour {


	public static string FILE_PATH = "Assets/Resources/Levels/";
	public static string FILE_SUFFIX = ".txt";


	public GameObject item;
	public float PieceSpeed;
	public float TimeStep;

	float oldPieceSpeed = 0;
	float oldTimeStep = 0;

	public string LevelName;

	private Dictionary<string, AudioClip> loadedSongs = new Dictionary<string, AudioClip>();

	public void Update() {
		//We changed one
		if(PieceSpeed != oldPieceSpeed || TimeStep != oldTimeStep) {
			if(PieceSpeed != 0 && TimeStep != 0 && oldPieceSpeed != 0 && oldTimeStep != 0) {
				Debug.Log ("Changing Piece Distances...");
				modifyItems();
				oldPieceSpeed = PieceSpeed;
				oldTimeStep = TimeStep;
			}
		}
	}

	void modifyItems() {
		GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
		foreach(GameObject item in items.OrderBy(child => child.transform.position.x)) {
			Vector3 itemLoc = item.transform.position;
			itemLoc.x /= (oldPieceSpeed * oldTimeStep);
			itemLoc.x *= (PieceSpeed * TimeStep);
			item.transform.position = itemLoc;
		}

	}

	public void ExportLevel(string levelName) {
		GameObject[] children = GameObject.FindGameObjectsWithTag("Item");
		float lastX = 0;
		StreamWriter sw = new StreamWriter(FILE_PATH+levelName+FILE_SUFFIX, false);
		string[] items = new string[children.Count()];
		int currentItem = 0;
		foreach(GameObject item in children.OrderBy(child => child.transform.position.x)) {
			float currentX = item.transform.position.x / (PieceSpeed * TimeStep);
			float diff = Mathf.Abs(currentX - lastX);
			items[currentItem] = item.transform.position.y > 0 ? diff.ToString() : (-diff).ToString();
			currentItem += 1;
			lastX = currentX;
		}

		string serializedLevel = string.Join(" ",items);
		Debug.Log ("Writing Level..."+levelName);
		string audioName = this.audio.clip != null ? " " + this.audio.clip.name : "";
		string configOptions = TimeStep + " " + PieceSpeed + audioName;
		sw.WriteLine(configOptions);
		sw.Write (serializedLevel);
		sw.Flush();
		sw.Close();
		Debug.Log ("Writing Complete.");
	}

	public void LoadLevel(string levelName) {
		GameObject[] oldItems = GameObject.FindGameObjectsWithTag("Item");
		foreach(GameObject item in oldItems) {
			DestroyImmediate(item);	
		}
		StreamReader sr = new StreamReader(FILE_PATH+levelName+FILE_SUFFIX);
		string[] configOptions = sr.ReadLine().Split(' ');
		string level = sr.ReadToEnd();
		sr.Close();
		this.TimeStep = float.Parse(configOptions[0]);
		this.PieceSpeed = float.Parse(configOptions[1]);
		AudioClip song = null;
		if(loadedSongs.ContainsKey(configOptions[2])) {
			song = loadedSongs[configOptions[2]];
		} else {
			song = Resources.Load<AudioClip>("Songs/"+configOptions[2]);
			loadedSongs.Add(configOptions[2], song);
		}

		this.audio.clip = song;
		oldTimeStep = TimeStep;
		oldPieceSpeed = PieceSpeed;
		string[] levelItems = level.Split(' ');
		float lastX = 0;
		GameObject newItem = null;
		int count = 1;
		foreach(string itemDef in levelItems) {
			float itemNum = float.Parse(itemDef);
			float newX = lastX + (Mathf.Abs(itemNum) * (PieceSpeed * TimeStep));
			Vector3 itemPos = new Vector3(newX, itemNum > 0 ? 1 : -1, 0);
			newItem = PrefabUtility.InstantiatePrefab(item) as GameObject;
			newItem.transform.position = itemPos;
			newItem.transform.rotation = Quaternion.identity;
			newItem.name = "Item"+count.ToString();
			//Set it to the normalized version, we don't want the mutation here
			lastX = itemPos.x;

			++count;
		}
	}
}
