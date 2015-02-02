using UnityEngine;
using System.Collections;

public class CollectionController : MonoBehaviour {

	public enum PieceType {
		GoodPiece,
		BadPiece
	}

	private int numCollected;

	// Use this for initialization
	void CollectPiece(PieceType pieceType) {
		switch(pieceType) {
			case PieceType.GoodPiece:
				numCollected += 1;
			break;

			case PieceType.BadPiece:

			break;

		}
	}
}
