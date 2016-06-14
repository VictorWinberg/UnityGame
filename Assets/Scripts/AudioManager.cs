using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

	public enum AudioChannel {Master, SFX, Music};

	float masterVolume = .3f;
	float sfxVolume = 1;
	float musicVolume = 1;

	AudioSource sfxSound2D;
	AudioSource[] musicSources;
	int musicSourceIndex;

	GameObject listener, player;

	SoundLibrary soundLibrary;

	public static AudioManager instance;

	public static void Create(Player player, SoundLibrary soundLibrary) {
		GameObject go = new GameObject("AudioManager");
		AudioManager audioManager = go.AddComponent<AudioManager> ();
		audioManager.player = player.gameObject;

		audioManager.soundLibrary = soundLibrary;

		GameObject child = new GameObject ("AudioListener");
		child.AddComponent<AudioListener> ();
		child.transform.parent = go.transform;

		audioManager.listener = child;
	}

	void Awake() {
		if (instance != null) {
			Destroy (gameObject);
			return;
		}

		instance = this;
		DontDestroyOnLoad (gameObject);

		musicSources = new AudioSource[2];
		for (int i = 0; i < musicSources.Length; i++) {
			GameObject newMusicSource = new GameObject ("Music source " + (i + 1));
			musicSources [i] = newMusicSource.AddComponent<AudioSource> ();
			musicSources [i].loop = true;
			newMusicSource.transform.parent = transform;
		}

		GameObject newSource2D = new GameObject ("SFX source 2D");
		sfxSound2D = newSource2D.AddComponent<AudioSource> ();
		newSource2D.transform.parent = transform;

		masterVolume = PlayerPrefs.GetFloat ("MasterVolume", masterVolume);
		musicVolume = PlayerPrefs.GetFloat ("MusicVolume", musicVolume);
		sfxVolume = PlayerPrefs.GetFloat ("SFXVolume", sfxVolume);
	}

	void Update() {
		if (player != null) {
			listener.transform.position = player.transform.position;
		}
	}

	public void SetVolume(float volume, AudioChannel channel) {
		switch (channel) {
		case AudioChannel.Master:
			masterVolume = volume;
			break;
		case AudioChannel.Music:
			musicVolume = volume;
			break;
		case AudioChannel.SFX:
			sfxVolume = volume;
			break;
		}

		musicSources [0].volume = musicVolume * masterVolume;
		musicSources [1].volume = musicVolume * masterVolume;

		PlayerPrefs.SetFloat ("MasterVolume", masterVolume);
		PlayerPrefs.SetFloat ("MusicVolume", musicVolume);
		PlayerPrefs.SetFloat ("SFXVolume", sfxVolume);
	}

	public void PlayMusic(AudioClip clip, float fadeDuration = 1) {
		musicSourceIndex = 1 - musicSourceIndex;
		musicSources [musicSourceIndex].clip = clip;
		musicSources [musicSourceIndex].Play ();

		StartCoroutine (AnimateMusicCrossfade(fadeDuration));
	}

	public void PlaySound(AudioClip clip, Vector3 pos) {
		if(clip != null)
			AudioSource.PlayClipAtPoint (clip, pos, sfxVolume * masterVolume);
	}

	public void PlaySound(string title, Vector3 pos) {
		PlaySound (soundLibrary.getClipFromTitle (title), pos);
	}

	public void PlaySound(string title) {
		sfxSound2D.PlayOneShot (soundLibrary.getClipFromTitle (title), sfxVolume * masterVolume);
	}

	IEnumerator AnimateMusicCrossfade(float duration) {
		float percent = 0;

		while (percent < 1) {
			percent += Time.deltaTime * 1 / duration;
			musicSources [musicSourceIndex].volume = Mathf.Lerp (0, musicVolume * masterVolume, percent);
			musicSources [1 - musicSourceIndex].volume = Mathf.Lerp (musicVolume * masterVolume, 0, percent);
			yield return null;
		}
	}
}
