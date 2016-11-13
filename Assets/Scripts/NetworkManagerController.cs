using UnityEngine;
using UnityEngine.Networking;

public class NetworkManagerController : NetworkManager {

	bool preview = true;

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

	MapGenerator map;
	float starttime = 3f;
	float timer = 0;

	void Awake() {
		setPreview (preview);
	}

	void Update() {
		if (preview) {
			map.transform.RotateAround (Vector3.zero, Vector3.up, .3f);

			timer += Time.deltaTime;
			if (timer > starttime) {
				timer = 0;
				map.mapIndex++;
				map.transform.rotation = Quaternion.identity;
				map.GenerateMap ();
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
