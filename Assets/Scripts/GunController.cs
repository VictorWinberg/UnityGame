﻿using UnityEngine;
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

	public void Shoot (){
		if (gun != null) {
			gun.Shoot ();
		}
	}
}