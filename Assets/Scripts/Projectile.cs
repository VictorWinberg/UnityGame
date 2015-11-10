using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	public LayerMask collisionMask;
	float speed = 10;
	float damage = 1;

	public void SetSpeed(float speed) {
		this.speed = speed;
	}

	void Update () {
		// Movement
		transform.Translate (Vector3.forward * Time.deltaTime * speed);

		// Collision Detection
		float moveDistance = speed * Time.deltaTime;
		CheckCollision (moveDistance);
	}

	void CheckCollision (float moveDistance){
		Ray ray = new Ray (transform.position, transform.forward);
		RaycastHit hit;

		if(Physics.Raycast(ray, out hit, moveDistance, collisionMask, QueryTriggerInteraction.Collide)) {
			OnHitObject(hit);
		}
	}

	void OnHitObject (RaycastHit hit){
		IDamageable damageableObject = hit.collider.GetComponent<IDamageable> ();

		if (damageableObject != null) {
			damageableObject.TakeHit(damage, hit);
		}
		GameObject.Destroy (gameObject);
	}
}
