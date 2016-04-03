using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameUI : MonoBehaviour {

	public Image fadeCanvas;
	public GameObject gameOverUI;

	public RectTransform waveBanner;
	public Text waveTitle, waveEnemyCount;

	Spawner spawner;

	void Awake() {
		spawner = FindObjectOfType<Spawner> ();
		spawner.OnNewWave += OnNewWave;
	}

	void Start () {
		FindObjectOfType<Player> ().OnDeath += OnGameOver;
	}

	void OnNewWave(int waveNumber) {
		waveTitle.text = "- Wave " + HumanFriendlyInteger.IntegerToWritten (waveNumber) + " -";
		string enemyCount = (spawner.waves [waveNumber - 1].infinite) ? "Infinite" : spawner.waves [waveNumber - 1].enemyCount + "";
		waveEnemyCount.text = "Enemies: " + enemyCount;
		StopCoroutine ("AnimateWaveBanner");
		StartCoroutine ("AnimateWaveBanner");
	}

	IEnumerator AnimateWaveBanner () {
		float delayTime = 1.5f;
		float speed = 3f;
		float animatePercent = 0f;
		int dir = 1;

		float endDelayTime = Time.time + 1 / speed + delayTime;

		while (animatePercent >= 0) {
			animatePercent += Time.deltaTime * speed * dir;

			if(animatePercent >= 1) {
				animatePercent = 1;
				if(Time.time > endDelayTime) {
					dir = -1;
				}
			}

			waveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-150, 150, animatePercent);
			yield return null;
		}
	}
	
	void OnGameOver () {
		StartCoroutine(Fade(Color.clear, Color.black, 1));
		gameOverUI.SetActive (true);
	}

	IEnumerator Fade(Color from, Color to, float time) {
		float speed = 1 / time;
		float percent = 0;

		while (percent < 1) {
			percent += Time.deltaTime * speed;
			fadeCanvas.color = Color.Lerp(from, to, percent);
			yield return null;
		}
	}

	// UI Input
	public void StartNewGame() {
		Application.LoadLevel ("Scene1");
	}
}
