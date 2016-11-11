﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public abstract class LivingEntity : NetworkBehaviour, IDamageable {

	public float startingHealth { get; protected set; }
	[SyncVar(hook = "ChangeHealth")]
	protected float health;
	protected bool dead, devMode;

	public event System.Action OnDeath, OnChangeHealth;

	protected virtual void Start() {
		health = startingHealth;
		devMode = FindObjectOfType<GameManager> ().devMode;
	}

	public virtual void TakeHit (float damage, Vector3 hitPoint, Vector3 hitDirection) {
		// Do some stuff here with hit var
		TakeDamage (damage);
	}

	public virtual void TakeDamage (float damage) {
		health -= damage;
		
		if (health <= 0 && !dead) {
			Die();
		}
	}

	public virtual void Heal (float amount) {
		health += Mathf.Clamp (amount, 0, startingHealth - health);
	}

	public float _health() {
		return health;
	}

	protected void ChangeHealth (float health) {
		this.health = health;
		if (OnChangeHealth != null)
			OnChangeHealth ();
	}

	[ContextMenu("Self Destruct")]
	public virtual void Die () {
		dead = true;
		if (OnDeath != null) {
			OnDeath ();
		}
		GameObject.Destroy (gameObject);
	}
}
