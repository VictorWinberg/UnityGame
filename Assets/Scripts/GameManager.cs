using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;

public class GameManager : NetworkBehaviour {

	public static int waves = 30;
	public bool devMode;

	private GameObject camera;
	private MapGenerator map;
	private Scoreboard scoreboard;
	private Crosshairs crosshairs;
	private Player player;
	private Enemy enemy;
	private Spawner spawner;
	private GameUI canvas;

	[SyncVar]
	public int seed;

	void Awake () {
		devMode = true;

		enemy = ((GameObject)Resources.Load ("Enemy")).GetComponent<Enemy> ();
		spawner = Spawner.Create ();
		spawner.devMode = devMode;
		map = MapGenerator.Create();
		map.GenerateMap ();
		GameObject audioManager = Instantiate (Resources.Load ("AudioManager"), Vector3.zero, Quaternion.identity) as GameObject;
	}

	void Start () {
		if (isServer) {
			System.Random random = new System.Random ();
			seed = random.Next ();
		}
		Debug.Log (seed);
	}
}
