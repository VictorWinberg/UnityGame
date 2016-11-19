using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GameManager : NetworkBehaviour {

	[SyncVar]
	public int seed;

	[SyncVar(hook="NewWave")]
	public int wave;

	public event System.Action<int> OnNewWave;

	void Start () {
		NetworkManagerExt gm = FindObjectOfType<NetworkManagerExt> ();
		if (isServer) {
			System.Random random = new System.Random ();
			seed = random.Next ();
		}
		if(isClient) {
			Spawner spawner = Spawner.Create ();
			spawner.devMode = gm.devMode;
			MapGenerator map = MapGenerator.Create();
			map.GenerateMap ();
			FindObjectOfType<GameUI> ().setIGM = this;
		}
	}

	public void NewWave(int wave) {
		this.wave = wave;
		OnNewWave (wave);
	}
}
