using UnityEngine;
using System.Collections.Generic;

public class ItemQueue : MonoBehaviour {
	Queue<GameObject> pool;

	public GameObject item;

	public void Start() {
		pool = new Queue<GameObject>();
	}

	public GameObject GetItem(){
		if(pool.Count > 0) {
			return pool.Dequeue();
		} else {
			return Instantiate(item) as GameObject;
		}
	}

	public void DestroyItem(GameObject item) {
		item.SetActive(false);
		pool.Enqueue(item);
	}
	
}
