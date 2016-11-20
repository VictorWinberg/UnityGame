using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;

public class NetworkManagerExt : NetworkManager {

	public static int waves = 30;

	private MapGenerator map;
	private Spawner spawner;
	private GameObject seed;
	private bool preview = true;

	void Awake() {
		setPreview (preview);
	}

	public override void OnStartClient(NetworkClient client) {
		base.OnStartClient (client);
		Debug.Log ("OnStartClient");
		setPreview(false);
	}

	public override void OnStopClient () {
		base.OnStopClient ();
		Debug.Log ("OnStopClient");
		setPreview(true);
	}

	public override void OnMatchCreate (bool success, string extendedInfo, UnityEngine.Networking.Match.MatchInfo matchInfo) {
		base.OnMatchCreate (success, extendedInfo, matchInfo);
		Debug.Log ("OnMatchCreate");
		setPreview(false);
	}

	public override void OnMatchJoined (bool success, string extendedInfo, UnityEngine.Networking.Match.MatchInfo matchInfo) {
		base.OnMatchJoined (success, extendedInfo, matchInfo);
		Debug.Log ("OnMatchJoined");
		setPreview(false);
	}

	public override void OnDestroyMatch (bool success, string extendedInfo) {
		base.OnDestroyMatch (success, extendedInfo);
		Debug.Log ("OnDestroyMatch");
		setPreview(false);
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
