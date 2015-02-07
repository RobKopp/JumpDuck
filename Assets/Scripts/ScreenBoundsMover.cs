using UnityEngine;
using System.Collections;

public class ScreenBoundsMover : MonoBehaviour {

	public enum Side {
		Left,
		Right,
		Top,
		Bottom
	}

	public Side WhichSide;

	public Vector3 Offset;

	// Use this for initialization
	void Start () {
		Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

		switch(WhichSide) {
			case Side.Left:
			screenPos.x = 0;
				break;

			case Side.Right:
			screenPos.x = Screen.width;
				break;

			case Side.Top:
			screenPos.y = 0;
				break;

			case Side.Bottom:
			screenPos.y = Screen.height;
				break;
		}

		screenPos = Camera.main.ScreenToWorldPoint(screenPos);

		transform.position = transform.position + (screenPos - transform.position) + Offset;
	}
}
