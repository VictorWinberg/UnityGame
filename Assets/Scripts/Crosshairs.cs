﻿using UnityEngine;
using System.Collections;

public class Crosshairs : MonoBehaviour {

	private Crosshairs instance;

	public SpriteRenderer dot;
	public Color dotHighlightColor;
	Color dotDefaultColor;

	public Crosshairs Create() {
		return ((GameObject)Instantiate (Resources.Load ("Crosshairs"), Vector3.right, Quaternion.AngleAxis(90, Vector3.right))).GetComponent<Crosshairs>();
	}

	void Start() {
		Cursor.visible = false;
		dotDefaultColor = dot.color;
	}

	void Update() {
		transform.Rotate(Vector3.forward * -40 * Time.deltaTime);
	}

	public void DetectTargets(Ray ray) {
		int enemyLayer = 1 << LayerMask.NameToLayer ("Enemy");
		DetectTargets (Physics.Raycast (ray, 100, enemyLayer));
	}

	public void DetectTargets(bool detected) {
		if (detected) {
			dot.color = dotHighlightColor;
		} else {
			dot.color = dotDefaultColor;
		}
	}
}
