using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
public class PlayerController : MonoBehaviour {

	Vector3 velocity;
	Rigidbody body;
	GunController gun;

	void Start () {
		body = GetComponent<Rigidbody> ();
		gun = GetComponent<GunController> ();
	}

	void FixedUpdate () {
		body.MovePosition (body.position + velocity * Time.fixedDeltaTime);
		LookAt (gun.point);
	}

	public void Move(Vector3 velocity) {
		this.velocity = velocity;
	}

	public void LookAt (Vector3 lookPoint) {
		Vector3 heightCorrectedPoint = new Vector3 (lookPoint.x, transform.position.y, lookPoint.z);
		transform.LookAt (heightCorrectedPoint);
	}
}
