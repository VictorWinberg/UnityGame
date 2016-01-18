using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

	public enum FireMode {Auto, Burst, Single};
	public FireMode fireMode;

	public Transform[] projectileSpawns;
	public Projectile projectile;
	public float msBetweenShots = 100;
	public float muzzleVelocity = 35;
	public int burstCount;

	public Transform shell, shellEjection;
	MuzzleFlash muzzleflash;

	float nextShotTime;

	bool triggerReleased;
	int burstShotsRemaining;

	void Start() {
		muzzleflash = GetComponent<MuzzleFlash> ();
		burstShotsRemaining = burstCount;
	}

	void Shoot() {
		if(Time.time > nextShotTime) {
			if(fireMode == FireMode.Burst) {
				if(burstShotsRemaining == 0)
					return;

				burstShotsRemaining--;
			} else if(fireMode == FireMode.Single) {
				if(!triggerReleased)
					return;
			}

			for (int i = 0; i < projectileSpawns.Length; i++) {
				nextShotTime = Time.time + msBetweenShots / 1000;
				Projectile newProjectile = Instantiate (projectile, projectileSpawns[i].position, projectileSpawns[i].rotation) as Projectile;
				newProjectile.SetSpeed (muzzleVelocity);
			}

			Instantiate(shell, shellEjection.position, shellEjection.rotation);
			muzzleflash.Activate();
		}
	}

	public void OnTriggerHold() {
		Shoot ();
		triggerReleased = false;
	}

	public void OnTriggerRelease() {
		triggerReleased = true;
		burstShotsRemaining = burstCount;
	}
}
