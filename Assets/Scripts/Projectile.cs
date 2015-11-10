using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	public LayerMask collisionMask;
	float speed = 10;
	float damage = 1;

	float lifetime = 3;
	float skinWidth = .1f;

	void Start() {
		Destroy (gameObject, lifetime);

		Collider[] initialCollision = Physics.OverlapSphere (transform.position, .1f, collisionMask);
		if (initialCollision.Length > 0) {
			OnHitObject(initialCollision[0]);
		}
	}

	public void SetSpeed(float speed) {
		this.speed = speed;
	}

	void Update () {
		// Movement
		float moveDistance = speed * Time.deltaTime;
		transform.Translate (Vector3.forward * moveDistance);

		// Collision Detection
		CheckCollisions (moveDistance);
	}

	void CheckCollisions (float moveDistance) {
		Ray ray = new Ray (transform.position, transform.forward);
		RaycastHit hit;

		if(Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide)) {
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

	void OnHitObject (Collider c){
		IDamageable damageableObject = c.GetComponent<IDamageable> ();
		
		if (damageableObject != null) {
			damageableObject.TakeDamage(damage);
		}
		GameObject.Destroy (gameObject);
	}
}
