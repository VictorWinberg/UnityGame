using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	private GameObject camera;
	private MapGenerator map;
	private Crosshairs crosshairs;
	private Player player;
	private Enemy enemy;
	private Spawner spawner;
	private GameUI canvas;

	void Awake () {
		camera = Instantiate (Resources.Load ("GameCamera"), new Vector3 (0, 12, -6), Quaternion.AngleAxis (60, Vector3.right)) as GameObject;
		crosshairs = new Crosshairs ().Create();
		player = Player.Create();
		player.crosshairs = crosshairs;
		enemy = ((GameObject)Resources.Load ("Enemy")).GetComponent<Enemy> ();
		spawner = new Spawner ().Create (enemy);
		spawner.setPlayer = player;
		map = MapGenerator.Create(spawner);
		map.GenerateMap ();
		canvas = new GameUI ().Create ();
	}

	void Start () {
		player.aimbot = false;
	}
	
	void Update () {

	}
}
