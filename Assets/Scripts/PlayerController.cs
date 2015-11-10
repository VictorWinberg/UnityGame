using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
public class PlayerController : MonoBehaviour {

	Vector3 velocity;
	Rigidbody body;

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody> ();
	}

	void FixedUpdate () {
		body.MovePosition (body.position + velocity * Time.fixedDeltaTime);
	}

	public void Move(Vector3 velocity) {
		this.velocity = velocity;
	}
	
	public void LookAt (Vector3 point){
		Vector3 lookPoint = new Vector3 (point.x, body.position.y, point.z);
		body.transform.LookAt (lookPoint);
	}
}
