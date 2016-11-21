using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GunController : NetworkBehaviour {

	public Transform weaponHold;
	public Gun[] guns;

	public Vector3 point { get; private set; }

	private Camera viewCamera;
	private Crosshairs crosshairs;
	private Gun gun;
	private int gunIndex;

	void Start() {
		viewCamera = Camera.main;

		SyncManager.instance.OnNewWave += OnNewWave;
		GetComponent<Player> ().OnDeath += OnPlayerDeath;

		if (isLocalPlayer) {
			// Create Crosshairs
			crosshairs = ((GameObject) Instantiate(Resources.Load("Crosshairs"))).GetComponent<Crosshairs>();
		}
	}

	void Update() {
		if(!isLocalPlayer || PauseMenu.IsOn)
			return;

		Ray ray = viewCamera.ScreenPointToRay (Input.mousePosition);
		Plane groundPlane = new Plane (Vector3.up, Vector3.up * GunHeight);
		float rayDistance;
		if (groundPlane.Raycast (ray, out rayDistance)) {
			point = ray.GetPoint (rayDistance);
			//Debug.DrawLine(ray.origin,point,Color.red);
			crosshairs.DetectTargets (ray);
			LookAtTarget (point);
		}

		// Weapon input
		if (Input.GetButtonDown("Fire1")) {
			if (GameManager.instance.aimbot) {
				// Look input
				int enemyLayer = 1 << LayerMask.NameToLayer ("Enemy");
				float minDist = Mathf.Infinity;
				Vector3 closest = point;
				foreach (Collider hit in Physics.OverlapSphere (transform.position, 10f, enemyLayer)) {
					float dist = Vector3.Distance (hit.transform.position, transform.position);
					if (dist < minDist) {
						minDist = dist;
						closest = hit.transform.position;
					}
				}
				crosshairs.DetectTargets (!point.Equals (closest));
				LookAtTarget (closest);
			}
			CmdOnTriggerHold();
		}
		if (Input.GetButtonUp("Fire1")) {
			CmdOnTriggerRelease();
		}
		if (Input.GetButtonDown("Reload")) {
			CmdReload();
		}
		if (GameManager.instance.devMode) {
			if (Input.GetKeyDown (KeyCode.F))
				GameManager.instance.aimbot = !GameManager.instance.aimbot;
		}
	}

	void OnNewWave(int waveNumber) {
		CmdEquipGun (waveNumber - 1);
	}

	void OnPlayerDeath() {
		CmdEquipGun (-1);
	}

	void EquipGun(Gun gunToEquip) {
		if (gun != null)
			Destroy(gun.gameObject);
		
		if (gunToEquip != null) {
			gun = (Gun)Instantiate (gunToEquip, weaponHold.position, weaponHold.rotation);
			gun.transform.SetParent (weaponHold);
		}
	}

	[Command]
	void CmdEquipGun(int gunIndex) {
		RpcEquipGun (gunIndex);
	}

	[ClientRpc]
	void RpcEquipGun(int gunIndex) {
		this.gunIndex = gunIndex;
		if (gunIndex == -1)
			EquipGun (null);
		else {
			EquipGun (guns [gunIndex % guns.Length]);
		}
	}

	[Command]
	void CmdOnTriggerHold (){
		RpcDoTriggerHold ();
	}

	[ClientRpc]
	void RpcDoTriggerHold() {
		if (gun != null) {
			gun.OnTriggerHold ();
		}
	}

	[Command]
	void CmdOnTriggerRelease() {
		RpcDoTriggerRelease ();
	}

	[ClientRpc]
	void RpcDoTriggerRelease () {
		if (gun != null) {
			gun.OnTriggerRelease ();
		}
	}

	float GunHeight {
		get {
			return weaponHold.position.y;
		}
	}

	[Command]
	public void CmdReload() {
		RpcReload ();
	}

	[ClientRpc]
	void RpcReload() {
		if (gun != null) {
			gun.Reload ();
		}
	}
	
	public Gun getGun() {
		return guns[gunIndex % guns.Length];
	}

	public Gun getGunWithIndex(int gunIndex) {
		return guns[gunIndex % guns.Length];
	}

	void LookAtTarget (Vector3 point) {
		crosshairs.transform.position = point;
	}
}
