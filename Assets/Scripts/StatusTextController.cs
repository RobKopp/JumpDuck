using UnityEngine;
using System.Collections;

public class StatusTextController : MonoBehaviour {

	

	// Use this for initialization
	void ShowStatus(string status) {
		gameObject.GetComponent<TextMesh>().text = status;
		animation.Rewind();
		animation.Play();
	}
}
