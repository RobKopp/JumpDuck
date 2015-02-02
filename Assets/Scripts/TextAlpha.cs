// TextAlpha.cs 
// Copyright (c) 2014 Orbital Games, LLC

using UnityEngine;

[RequireComponent(typeof(TextMesh))]
[ExecuteInEditMode]
public class TextAlpha : MonoBehaviour
{
	public float alpha = 1;

	TextMesh text;
	Color _cachedColor;

	void Awake()
	{
		text = GetComponent<TextMesh>();
	}

	void Update()
	{
		if (text != null)
		{
			_cachedColor = text.color;
			_cachedColor.a = alpha;
			text.color = _cachedColor;
		}
	}
}
