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

		// Create Crosshairs
		crosshairs = ((GameObject) Instantiate(Resources.Load("Crosshairs"))).GetComponent<Crosshairs>();
	}

	void Update() {
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
		if (Input.GetMouseButton(0)) {
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
		if (Input.GetMouseButtonUp(0)) {
			CmdOnTriggerRelease();
		}
		if (Input.GetKeyDown (KeyCode.R)) {
			CmdReload();
		}
	}

	public void EquipGun(Gun gunToEquip) {
		if (gun != null) {
			Destroy(gun.gameObject);
		}
		gun = (Gun)Instantiate (gunToEquip, weaponHold.position, weaponHold.rotation);
		gun.parentNetId = weaponHold.parent.GetComponent<NetworkIdentity> ().netId;
		gun.transform.parent = weaponHold;
		NetworkServer.Spawn (gun.gameObject);
	}

	[Command]
	public void CmdEquipGun(int gunIndex) {
		this.gunIndex = gunIndex;
		EquipGun (guns [gunIndex % guns.Length]);
	}

	[Command]
	public void CmdOnTriggerHold (){
		if (gun != null) {
			gun.OnTriggerHold ();
		}
	}

	[Command]
	public void CmdOnTriggerRelease() {
		if (gun != null) {
			gun.OnTriggerRelease ();
		}
	}

	public float GunHeight {
		get {
			return weaponHold.position.y;
		}
	}

	public void Aim(Vector3 aimPoint) {
		if (gun != null) {
			gun.Aim (aimPoint);
		}
	}

	[Command]
	public void CmdReload() {
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
		if((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1)
			Aim (point);
	}
}
