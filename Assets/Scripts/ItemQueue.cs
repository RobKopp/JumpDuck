using UnityEngine;
using System.Collections.Generic;

public class ItemQueue : MonoBehaviour {
	Dictionary<string,Queue<GameObject>> pool;

	public GameObject[] items;

	void Start() {
		pool = new Dictionary<string, Queue<GameObject>>();
	}

	public GameObject GetItem(string type){

		if(pool.ContainsKey(type) && pool[type].Count > 0) {
			return pool[type].Dequeue();
		} else {
			foreach(GameObject item in items) {
				if(item.tag == type) {
					return Instantiate(item) as GameObject;
				}
			}
			return null;
		}
	}

	public void DestroyItem(GameObject item) {
		item.SetActive(false);
		if(!pool.ContainsKey(item.tag)) {
			pool.Add(item.tag,new Queue<GameObject>());
		}
		pool[item.tag].Enqueue(item);
	}
	
}
