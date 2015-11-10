using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	public Wave[] waves;
	public Enemy enemy;

	Wave currentWave;
	int currentWaveNumber;

	int enemiesRemainingToSpawn;
	int enemiesRemainingAlive;
	float nextSpawnTime; 

	// Use this for initialization
	void Start () {
		NextWave ();
	}
	
	// Update is called once per frame
	void Update () {
		if (enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime) {
			enemiesRemainingToSpawn--;
			nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

			Enemy spawnedEnemy = Instantiate(enemy, Vector3.zero, Quaternion.identity) as Enemy;
			spawnedEnemy.OnDeath += OnEnemyDeath;
		}
	}

	void OnEnemyDeath (){
		enemiesRemainingAlive--;
		if (currentWaveNumber - 1 < waves.Length) {
			currentWave = waves [currentWaveNumber - 1];

			enemiesRemainingToSpawn = currentWave.enemyCount;
			enemiesRemainingAlive = enemiesRemainingToSpawn;
		}
	}

	void NextWave() {
		currentWaveNumber++;
		currentWave = waves [currentWaveNumber - 1];

		enemiesRemainingToSpawn = currentWave.enemyCount;
	}
	
	[System.Serializable]
	public class Wave {
		public int enemyCount;
		public float timeBetweenSpawns;
	}
}
