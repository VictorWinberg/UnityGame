using UnityEngine;
using System.Collections;

public class LivingEntity : MonoBehaviour, IDamageable {

	public float startHealth = 3;
	protected float health;
	protected bool dead;

	public event System.Action OnDeath;

	protected virtual void Start() {
		health = startHealth;
	}

	public void TakeHit (float damage, RaycastHit hit){
		health -= damage;

		if (health <= 0 && !dead) {
			Die();
		}
	}

	protected void Die (){
		dead = true;
		if (OnDeath != null) {
			OnDeath();
		}
		GameObject.Destroy (gameObject);
	}
}
