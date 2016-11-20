using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent (typeof(Player))]
[RequireComponent (typeof (PlayerController))]
public class PlayerSetup : NetworkBehaviour {

	[SerializeField]
	Behaviour[] componentsToDisable;

	[SerializeField]
	string remoteLayerName = "RemotePlayer";

	[HideInInspector]
	public GameObject gameUI;

	// Use this for initialization
	void Start () {
		// Disable components that should only be
		// active on the player that we control
		if (!isLocalPlayer) {
			DisableComponents();
			AssignRemoteLayer();
			Destroy(transform.FindChild ("View Visualisation").gameObject);
		}
		else {
			// Create GameUI
			gameUI = Instantiate(Resources.Load("GameUI")) as GameObject;
			gameUI.GetComponent<GameUI> ().setIGM = SyncManager.instance;
			gameUI.GetComponent<GameUI> ().setPlayer = GetComponent<Player> ();

			GetComponent<Player>().SetupPlayer();
		}
	}
	
	void SetLayerRecursively (GameObject obj, int newLayer) {
		obj.layer = newLayer;

		foreach (Transform child in obj.transform) {
			SetLayerRecursively(child.gameObject, newLayer);
		}
	}

	public override void OnStartClient() {
		base.OnStartClient();

		string _netID = GetComponent<NetworkIdentity>().netId.ToString();
		Player _player = GetComponent<Player>();

		GameManager.RegisterPlayer(_netID, _player);
	}

	void AssignRemoteLayer () {
		gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
	}

	void DisableComponents () {
		for (int i = 0; i < componentsToDisable.Length; i++) {
			componentsToDisable[i].enabled = false;
		}
	}

	// When we are destroyed
	void OnDisable () {
		if (isLocalPlayer)
			GameManager.instance.SetSceneCameraActive(true);

		GameManager.UnRegisterPlayer(transform.name);
	}
}
