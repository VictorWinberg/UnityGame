using UnityEngine;
using System.Collections;

public class Scoreboard : MonoBehaviour {

	public static int score { get; private set; }
	float lastKillTime;
	int killStreak;
	float killStreakExpiry = 1f;

	// Use this for initialization
	public Scoreboard Create (Player player) {
		GameObject go = new GameObject ("Scoreboard");
		Scoreboard scoreboard = go.AddComponent<Scoreboard> ();

		Enemy.OnDeathStatic += scoreboard.OnEnemyKilled;
		player.OnDeath += OnPlayerDeath;
		return scoreboard;
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
