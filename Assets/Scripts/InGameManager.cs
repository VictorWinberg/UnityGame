using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class InGameManager : NetworkBehaviour {

	[SyncVar]
	public int seed;

	public event System.Action<int> OnNewWave;

	void Start () {
		GameManager gm = FindObjectOfType<GameManager> ();
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
		OnNewWave (wave);
	}
}
