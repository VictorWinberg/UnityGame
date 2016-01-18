using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	public Wave[] waves;
	public Enemy enemy;

	LivingEntity player;

	Wave currentWave;
	int currentWaveNumber;

	int enemiesRemainingToSpawn, enemiesRemainingAlive;
	float nextSpawnTime;

	MapGenerator map;

	float idleTimeCheck = 2;
	float idleThresholdDistance = 1.5f;
	float nextIdleTimeCheck;
	Vector3 idlePositionPrevious;
	bool isIdle;

	bool isDisabled;

	public event System.Action<int> OnNewWave;

	void Start () {
		player = FindObjectOfType<Player> ();

		nextIdleTimeCheck = idleTimeCheck + Time.time;
		idlePositionPrevious = player.transform.position;
		player.OnDeath += OnPlayerDeath;

		map = FindObjectOfType<MapGenerator> ();
		NextWave ();
	}

	void Update () {
		if (isDisabled)
			return;

		if (Time.time > nextIdleTimeCheck) {
			nextIdleTimeCheck = idleTimeCheck + Time.time;

			isIdle = (Vector3.Distance(player.transform.position, idlePositionPrevious) < idleThresholdDistance);
			idlePositionPrevious = player.transform.position;
		}

		if (enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime) {
			enemiesRemainingToSpawn--;
			nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

			StartCoroutine(SpawnEnemy());
		}
	}

	IEnumerator SpawnEnemy() {
		float spawnDelay = 1;
		float tileFlashSpeed = 4;

		Transform spawnTile = isIdle ? map.getTileFromPosition(player.transform.position) : map.getRandomOpenTile ();
		Material tileMaterial = spawnTile.GetComponent<Renderer> ().material;
		Color initialColor = tileMaterial.color;
		Color flashColor = Color.red;
		float spawnTimer = 0;

		while (spawnTimer < spawnDelay) {

			tileMaterial.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));

			spawnTimer += Time.deltaTime;
			yield return null;
		}

		Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
		spawnedEnemy.OnDeath += OnEnemyDeath;
	}

	void OnPlayerDeath() {
		isDisabled = true;
	}

	void OnEnemyDeath (){
		enemiesRemainingAlive--;

		if (enemiesRemainingAlive == 0) {
			NextWave();
		}
	}

	void ResetPlayerPosition() {
		player.transform.position = map.getTileFromPosition(Vector3.zero).position + Vector3.up;
	}

	void NextWave() {
		currentWaveNumber++;
		print ("Wave: " + currentWaveNumber);
		if (currentWaveNumber - 1 < waves.Length) {
			currentWave = waves [currentWaveNumber - 1];
			
			enemiesRemainingToSpawn = currentWave.enemyCount;
			enemiesRemainingAlive = enemiesRemainingToSpawn;

			if(OnNewWave != null) OnNewWave(currentWaveNumber);
			ResetPlayerPosition();
		}
	}

	[System.Serializable]
	public class Wave {
		public int enemyCount;
		public float timeBetweenSpawns;
	}
}
