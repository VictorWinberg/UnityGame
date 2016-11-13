using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GunController : NetworkBehaviour {

	public Transform weaponHold;
	public Gun[] guns;
	private Gun gun;
	private int gunIndex;

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
}
