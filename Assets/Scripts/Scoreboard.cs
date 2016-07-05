using UnityEngine;
using System.Collections;

public class Scoreboard : MonoBehaviour {

	public static int score { get; private set; }
	float lastKillTime;
	int killStreak;
	float killStreakExpiry = 1f;

	public Player setPlayer {
		set {
			value.OnDeath += OnPlayerDeath;
		}
	}

	// Use this for initialization
	void Start() {
		Enemy.OnDeathStatic += OnEnemyKilled;
	}

	void OnEnemyKilled() {
		if (Time.time < lastKillTime + killStreakExpiry) {
			killStreak++;
		} else {
			killStreak = 0;
		}

		lastKillTime = Time.time;

		score += 5 + 2 * killStreak;
	}

	void OnPlayerDeath() {
		Enemy.OnDeathStatic -= OnEnemyKilled;
	}
}
