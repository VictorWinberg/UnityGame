using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SyncManager : NetworkBehaviour {

	public static SyncManager instance;

	[SyncVar]
	public int seed;

	[SyncVar(hook="NewWave")]
	public int wave;

	public event System.Action<int> OnNewWave;

	void Awake () {
		if (instance != null) {
			Debug.LogError("More than one SyncManager in scene.");
		} else {
			instance = this;
		}
	}

	void Start () {
		if (isServer) {
			System.Random random = new System.Random ();
			seed = random.Next ();
		}
		if(isClient) {
			Spawner spawner = Spawner.Create ();
			MapGenerator map = MapGenerator.Create();
			map.GenerateMap ();
		}
	}

	public void NewWave(int wave) {
		this.wave = wave;
		OnNewWave (wave);
	}
}
