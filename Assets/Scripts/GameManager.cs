using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static int waves = 30;

	private GameObject camera;
	private MapGenerator map;
	private Scoreboard scoreboard;
	private Crosshairs crosshairs;
	private Player player;
	private Enemy enemy;
	private Spawner spawner;
	private GameUI canvas;

	void Awake () {
		camera = Instantiate (Resources.Load ("GameCamera"), new Vector3 (0, 12, -6), Quaternion.AngleAxis (60, Vector3.right)) as GameObject;
		GameObject cache = new GameObject("Cache");
		crosshairs = cache.AddComponent<Crosshairs> ().Create ();
		player = Player.Create();
		player.crosshairs = crosshairs;
		enemy = ((GameObject)Resources.Load ("Enemy")).GetComponent<Enemy> ();
		spawner = cache.AddComponent<Spawner>().Create (enemy);
		spawner.setPlayer = player;
		map = MapGenerator.Create(spawner);
		map.GenerateMap ();
		scoreboard = cache.AddComponent<Scoreboard> ().Create (player);
		GameObject audioManager = Instantiate (Resources.Load ("AudioManager"), Vector3.zero, Quaternion.identity) as GameObject;
		audioManager.GetComponent<AudioManager> ().SetPlayer (player.gameObject);
		Destroy (cache);
	}

	void Start () {
		//player.aimbot = false;
		//player.startingHealth = 100;
		GameObject cache = new GameObject ("Cache");
		canvas = cache.AddComponent<GameUI>().Create (player, spawner, this);
		Destroy (cache);
	}
	
	void Update () {

	}

	public Player getPlayer() {
		return player;
	}
}
