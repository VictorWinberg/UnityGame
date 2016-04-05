using UnityEngine;
using System.Collections;

[RequireComponent (typeof (PlayerController))]
[RequireComponent (typeof (GunController))]
public class Player : LivingEntity {

	public float moveSpeed = 5;

	public Crosshairs  crosshairs;

	protected Camera viewCamera;
	PlayerController controller;
	GunController gunController;

	public bool aimbot = false;

	public static Player Create() {
		Player player = ((GameObject)Instantiate (Resources.Load ("Player"), Vector3.up, Quaternion.identity)).GetComponent<Player> ();
		player.controller = player.gameObject.GetComponent<PlayerController> ();
		player.gunController = player.gameObject.GetComponent<GunController> ();
		player.viewCamera = Camera.main;
		return player;
	}

	protected override void Start () {
		base.Start ();
		FindObjectOfType<Spawner> ().OnNewWave += OnNewWave;
	}

	void OnNewWave(int waveNumber) {
		health = startingHealth;
		gunController.EquipGun (waveNumber - 1);
	}

	void LookAtTarget (Vector3 point) {
		controller.LookAt (point);
		crosshairs.transform.position = point;
		if((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1)
			gunController.Aim (point);
	}

	void Update () {
		// Movement input
		Vector3 moveInput = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));
		Vector3 moveVelocity = moveInput.normalized * moveSpeed;
		controller.Move (moveVelocity);

		Ray ray = viewCamera.ScreenPointToRay (Input.mousePosition);
		Plane groundPlane = new Plane (Vector3.up, Vector3.up * gunController.GunHeight);
		float rayDistance;
		Vector3 point = Vector3.zero;
		if (groundPlane.Raycast (ray, out rayDistance)) {
			point = ray.GetPoint (rayDistance);
			//Debug.DrawLine(ray.origin,point,Color.red);
			crosshairs.DetectTargets (ray);
			LookAtTarget (point);
		}

		// Weapon input
		if (Input.GetMouseButton(0)) {
			if (aimbot) {
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
			gunController.OnTriggerHold();
		}
		if (Input.GetMouseButtonUp(0)) {
			gunController.OnTriggerRelease();
		}
		if (Input.GetKeyDown (KeyCode.R)) {
			gunController.Reload();
		}
	}
}
