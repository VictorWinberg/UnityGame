using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent (typeof (NavMeshAgent))]
public class Enemy : LivingEntity {

	public enum State {Idle, Chasing, Attacking};
	State currentState;

	public ParticleSystem deathEffect;
	public static event System.Action OnDeathStatic;
	
	NavMeshAgent pathfinder;
	Transform target;
	LivingEntity targetEntity;

	Material skinMaterial;

	Color originalColour;

	float attackDistanceThreshold = .5f;
	float timeBetweenAttacks = 1;
	float damage = 1;
	
	float nextAttackTime, myCollisionRadius, targetCollisionRadius;

	[SyncVar]
	float moveSpeed, angularSpeed;

	bool hasTarget;
	[SyncVar(hook="Freeze")]
	bool isFreeze;

	void Awake() {
		pathfinder = GetComponent<NavMeshAgent> ();
		angularSpeed = pathfinder.angularSpeed;
		
		if (GameObject.FindGameObjectWithTag ("Player") != null) {
			hasTarget = true;

			target = GameObject.FindGameObjectWithTag ("Player").transform;
			targetEntity = target.GetComponent<LivingEntity> ();
			
			myCollisionRadius = GetComponent<CapsuleCollider> ().radius;
			targetCollisionRadius = target.GetComponent<CapsuleCollider> ().radius;
		}
	}

	protected void Start () {
		Heal (startingHealth);
		Freeze (isFreeze);

		if (hasTarget) {
			currentState = State.Chasing;

			targetEntity.OnDeath += OnTargetDeath;
			
			StartCoroutine (UpdatePath ());
		}
	}

	public void SetCharacteristics (float moveSpeed, int damage, float health, Color skinColor){
		this.moveSpeed = moveSpeed;
		pathfinder.speed = moveSpeed;

		if (hasTarget) this.damage = damage;
		startingHealth = health;

		deathEffect.startColor = new Color (skinColor.r, skinColor.g, skinColor.b, 1);
		skinMaterial = GetComponent<Renderer> ().material;
		skinMaterial.color = skinColor;
		originalColour = skinMaterial.color;
	}

	public override void TakeHit (float damage, Vector3 hitPoint, Vector3 hitDirection) {
		AudioManager.instance.PlaySound ("Impact", transform.position);
		if (damage <= health) {
			if (OnDeathStatic != null) {
				OnDeathStatic ();
			}
			AudioManager.instance.PlaySound ("Enemy Death", transform.position);
			Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, deathEffect.startLifetime);
		}
		base.TakeHit (damage, hitPoint, hitDirection);
	}

	void OnTargetDeath() {
		hasTarget = false;
		currentState = State.Idle;
	}

	void Update () {
		if (!isServer)
			return;
		
		if (hasTarget) {
			if (Time.time > nextAttackTime) {
				float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
				if (sqrDstToTarget < Mathf.Pow (attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2)) {
					nextAttackTime = Time.time + timeBetweenAttacks;
					AudioManager.instance.PlaySound ("Enemy Attack", transform.position);
					StartCoroutine (Attack ());
				}
			}
		}

		if (transform.position.y < -10) {
			RpcTakeDamage (health);
		}
	}

	IEnumerator Attack() {
		currentState = State.Attacking;
		pathfinder.enabled = false;
		
		Vector3 originalPosition = transform.position;
		Vector3 dirToTarget = (target.position - transform.position).normalized;
		Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius);
		
		float attackSpeed = 3;
		float percent = 0;

		skinMaterial.color = Color.red;
		bool hasAppliedDamage = false;
		
		while (percent <= 1) {

			if(percent >= 0.5f && !hasAppliedDamage) {
				hasAppliedDamage = true;
				targetEntity.RpcTakeDamage(damage);
			}
			percent += Time.deltaTime * attackSpeed;
			float interpolation = (-Mathf.Pow(percent,2) + percent) * 4;
			transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);
			
			yield return null;
		}
		
		skinMaterial.color = originalColour;
		currentState = State.Chasing;
		pathfinder.enabled = true;
	}

	// UpdatePath is called once per refreshRate
	IEnumerator UpdatePath() {
		float refreshRate = .25f;

		while (hasTarget) {
			if (currentState == State.Chasing) {
				Vector3 dirToTarget = (target.position - transform.position).normalized;
				Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold/2);
				if (!dead) {
					pathfinder.SetDestination (targetPosition);
				}
			}
			yield return new WaitForSeconds(refreshRate);
		}
	}

	public override void Die () {
		base.Die ();
		GameObject.Destroy (gameObject);
	}

	public void Freeze(bool freeze) {
		this.isFreeze = freeze;
		if (freeze) {
			pathfinder.speed = 0;
			pathfinder.angularSpeed = 0;
		} else {
			pathfinder.speed = moveSpeed;
			pathfinder.angularSpeed = angularSpeed;
		}
	}
}
