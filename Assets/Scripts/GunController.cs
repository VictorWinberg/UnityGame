using UnityEngine;
using System.Collections;

public class GunController : MonoBehaviour {

	public Transform weaponHold;
	public Gun startingGun;
	Gun gun;

	void Start() {
		if (startingGun != null) {
			EquipGun(startingGun);
		}
	}

	public void EquipGun(Gun gunToEquip) {
		if (gun != null) {
			Destroy(gun.gameObject);
		}
		gun = Instantiate (gunToEquip, weaponHold.position,weaponHold.rotation) as Gun;
		gun.transform.parent = weaponHold;
	}

	public void OnTriggerHold (){
		if (gun != null) {
			gun.OnTriggerHold ();
		}
	}

	public void OnTriggerRelease() {
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

	public void Reload() {
		if (gun != null) {
			gun.Reload ();
		}
	}
}
