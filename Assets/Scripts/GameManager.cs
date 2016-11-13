using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;

public class GameManager : NetworkManager {

	public static int waves = 30;
	public bool devMode;

	private MapGenerator map;
	private Spawner spawner;
	private GameObject seed;
	private bool preview = true;

	void Awake() {
		devMode = true;
		setPreview (preview);
	}

	public override void OnStartClient(NetworkClient client) {
		base.OnStartClient (client);
		Debug.Log ("OnStartClient");
		setPreview(false);
	}

	public override void OnStartHost (){
		base.OnStartHost ();
		Debug.Log ("OnStartHost");
		setPreview(false);
	}

	public override void OnStopClient () {
		base.OnStopClient ();
		Debug.Log ("OnStopClient");
		setPreview(true);
	}

	public override void OnStopHost () {
		base.OnStopHost ();
		Debug.Log ("OnStopHost");
		setPreview(true);
	}

	float starttime = 3f;
	float timer = 0;

	void Update() {
		if (preview) {
			map.transform.RotateAround (Vector3.zero, Vector3.up, .3f);

			timer += Time.deltaTime;
			if (timer > starttime) {
				timer = 0;
				map.mapIndex++;
				Quaternion rotation = map.transform.rotation;
				map.transform.rotation = Quaternion.identity;
				map.GenerateMap ();
				map.transform.rotation = rotation;
			}
		}
	}

	void setPreview(bool value) {
		if (value) {
			map = MapGenerator.Create ();
			map.GenerateMap ();
		} else {
			Destroy (map.gameObject);
		}

		preview = value;
	}
}
