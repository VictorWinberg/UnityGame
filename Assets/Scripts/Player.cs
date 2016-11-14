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

	private bool aimbot = false;

	protected override void Start () {
		if (!isLocalPlayer) {
			Destroy(transform.FindChild ("View Visualisation").gameObject);
			return;
		}

		startingHealth = 80;

		base.Start ();

		controller = GetComponent<PlayerController> ();
		gunController = GetComponent<GunController> ();
		crosshairs = FindObjectOfType<Crosshairs> ();
		viewCamera = Camera.main;
		InGameManager igm = FindObjectOfType<InGameManager> ();
		igm.OnNewWave += OnNewWave;
		Spawner spawner = FindObjectOfType<Spawner> ();
		spawner.setPlayer = this;
		FindObjectOfType<GameUI> ().setPlayer = this;
		FindObjectOfType<Scoreboard> ().setPlayer = this;
	}

	public override void OnStartLocalPlayer () {
		GetComponent<MeshRenderer>().material.color = Color.blue;
	}

	void Update () {
		if (!isLocalPlayer) {
			return;
		}

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
			gunController.CmdOnTriggerHold();
		}
		if (Input.GetMouseButtonUp(0)) {
			gunController.CmdOnTriggerRelease();
		}
		if (Input.GetKeyDown (KeyCode.R)) {
			gunController.CmdReload();
		}
		if (devMode) {
			if(Input.GetKeyDown (KeyCode.G)) aimbot = !aimbot;
			if (Input.GetKeyDown (KeyCode.H)) Heal (startingHealth);
		}
	}

	void OnNewWave(int waveNumber) {
		if (waveNumber != 1) startingHealth = (int)(startingHealth * 1.2f);
		health = startingHealth;
		gunController.CmdEquipGun (waveNumber - 1);
	}

	void LookAtTarget (Vector3 point) {
		controller.LookAt (point);
		crosshairs.transform.position = point;
		if((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1)
			gunController.Aim (point);
	}

	public Gun getGun() {
		return gunController.getGun ();
	}

	public Gun getGunWithIndex(int gunIndex) {
		return gunController.getGunWithIndex (gunIndex);
	}

	public override void Die () {
		AudioManager.instance.PlaySound ("Player Death", transform.position);
		base.Die ();
	}
}
