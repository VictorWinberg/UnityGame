using UnityEngine;
using System.Collections;

[RequireComponent (typeof (PlayerController))]
[RequireComponent (typeof (GunController))]
public class Player : LivingEntity {

	public float moveSpeed = 5;

	Camera cam;
	PlayerController controller;
	GunController gunController;

	// Use this for initialization
	protected override void Start () {
		base.Start ();
		controller = GetComponent<PlayerController> ();
		gunController = GetComponent<GunController> ();
		cam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
		// Movement input
		Vector3 moveInput = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));
		Vector3 moveVelocity = moveInput.normalized * moveSpeed;
		controller.Move (moveVelocity);

		// Look input
		Ray ray = cam.ScreenPointToRay (Input.mousePosition);
		Plane ground = new Plane (Vector3.up, Vector3.zero);
		float raydistance;

		if(ground.Raycast(ray, out raydistance)) {
			Vector3 point = ray.GetPoint(raydistance);
			//Debug.DrawLine(cam.transform.position, point, Color.red);
			controller.LookAt(point);
		}

		if (Input.GetMouseButton (0)) {
			gunController.Shoot();
		}
	}
}
