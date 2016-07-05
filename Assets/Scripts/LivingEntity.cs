﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class LivingEntity : NetworkBehaviour, IDamageable {

	public float startingHealth = 2;
	public float health { get; protected set; }
	protected bool dead;

	public event System.Action OnDeath;

	protected virtual void Start() {
		health = startingHealth;
	}

	public virtual void TakeHit (float damage, Vector3 hitPoint, Vector3 hitDirection){
		// Do some stuff here with hit var
		TakeDamage (damage);
	}

	public virtual void TakeDamage (float damage){
		health -= damage;
		
		if (health <= 0 && !dead) {
			Die();
		}
	}

	[ContextMenu("Self Destruct")]
	public virtual void Die (){
		dead = true;
		if (OnDeath != null) {
			OnDeath();
		}
		GameObject.Destroy (gameObject);
	}
}
