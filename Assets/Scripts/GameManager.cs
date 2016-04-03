﻿using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	private GameObject camera;
	private MapGenerator map;
	private Crosshairs crosshairs;
	private Player player;
	private Spawner spawner;
	private GameUI canvas;

	void Awake () {
		camera = Instantiate (Resources.Load ("GameCamera"), new Vector3 (0, 12, -6), Quaternion.AngleAxis (60, Vector3.right)) as GameObject;
		map = FindObjectOfType<MapGenerator> ();
		crosshairs = new Crosshairs ().Create();
		player = new Player().Create();
		player.crosshairs = crosshairs;
		spawner = FindObjectOfType<Spawner> ();
		spawner.setPlayer = player;
		canvas = new GameUI ().Create ();
	}

	void Start () {
		player.aimbot = true;
	}
	
	void Update () {

	}
}
