using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent (typeof (PlayerController))]
[RequireComponent (typeof (GunController))]
public class Player : LivingEntity {

	public float moveSpeed = 5;

	PlayerController controller;
	GunController gunController;

	[SerializeField]
	private Behaviour[] disableOnDeath;
	private bool[] wasEnabled;

	[SerializeField]
	private GameObject[] disableGameObjectsOnDeath;

	private bool firstSetup = true;
	private bool aimbot = false;

	public void SetupPlayer () {
		if (isLocalPlayer) {
			//Switch cameras
			GameManager.instance.SetSceneCameraActive(false);
			GetComponent<PlayerSetup>().gameUI.SetActive(true);
		}

		CmdBroadCastNewPlayerSetup();
	}

	[Command]
	private void CmdBroadCastNewPlayerSetup () {
		RpcSetupPlayerOnAllClients();
	}

	[ClientRpc]
	private void RpcSetupPlayerOnAllClients () {
		if (firstSetup) {
			wasEnabled = new bool[disableOnDeath.Length];
			for (int i = 0; i < wasEnabled.Length; i++) {
				wasEnabled[i] = disableOnDeath[i].enabled;
			}

			firstSetup = false;
		}

		SetDefaults();
	}

	// TODO: Fix
	public override void OnStartLocalPlayer () {
		GetComponent<MeshRenderer>().material.color = Color.blue;
	}

	void Start() {
		controller = GetComponent<PlayerController> ();
	}

	void Update () {
		if (!isLocalPlayer || PauseMenu.IsOn) {
			return;
		}

		// Movement input
		Vector3 moveInput = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));
		Vector3 moveVelocity = moveInput.normalized * moveSpeed;
		controller.Move (moveVelocity);

		if (GameManager.instance.devMode) {
			if (Input.GetKeyDown (KeyCode.H)) 
				Heal (startingHealth);
		}
	}

	void OnNewWave(int waveNumber) {
		if (waveNumber != 1) startingHealth = (int)(startingHealth * 1.2f);
		Heal (startingHealth);
	}

	public Gun getGun() {
		return gunController.getGun ();
	}

	public Gun getGunWithIndex(int gunIndex) {
		return gunController.getGunWithIndex (gunIndex);
	}

	public override void Die () {
		AudioManager.instance.PlaySound ("Player Death", transform.position);
		base.Die ();

		//Disable components
		for (int i = 0; i < disableOnDeath.Length; i++) {
			disableOnDeath[i].enabled = false;
		}

		//Disable GameObjects
		for (int i = 0; i < disableGameObjectsOnDeath.Length; i++) {
			disableGameObjectsOnDeath[i].SetActive(false);
		}

		//Disable the collider
		Collider _col = GetComponent<Collider>();
		if (_col != null)
			_col.enabled = false;

		//Spawn a death effect
		GameObject _gfxIns = (GameObject)Instantiate(Resources.Load("Explosion"), transform.position, Quaternion.identity);
		Destroy(_gfxIns, 3f);

		//Switch cameras
		if (isLocalPlayer) {
			GameManager.instance.SetSceneCameraActive(true);
			//GetComponent<PlayerSetup>().playerUI.SetActive(false);
		}

		Debug.Log(transform.name + " is DEAD!");
	}

	public void Respawn() {
		StartCoroutine (_Respawn());
	}

	private IEnumerator _Respawn () {
		yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

		Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
		transform.position = _spawnPoint.position;
		transform.rotation = _spawnPoint.rotation;

		yield return new WaitForSeconds(0.1f);

		SetupPlayer();

		Debug.Log(transform.name + " respawned.");
	}

	public void SetDefaults (){
		dead = false;

		startingHealth = 80;
		health = startingHealth;

		gunController = GetComponent<GunController> ();
		SyncManager.instance.OnNewWave += OnNewWave;
		Spawner spawner = FindObjectOfType<Spawner> ();
		spawner.setPlayer = this;
		FindObjectOfType<Scoreboard> ().setPlayer = this;

		//Enable the components
		for (int i = 0; i < disableOnDeath.Length; i++) {
			disableOnDeath[i].enabled = wasEnabled[i];
		}

		//Enable the gameobjects
		for (int i = 0; i < disableGameObjectsOnDeath.Length; i++) {
			disableGameObjectsOnDeath[i].SetActive(true);
		}

		//Enable the collider
		Collider _col = GetComponent<Collider>();
		if (_col != null)
			_col.enabled = true;

		//Create spawn effect
		GameObject _gfxIns = (GameObject)Instantiate(Resources.Load("SpawnEffect"), transform.position, Quaternion.identity);
		Destroy(_gfxIns, 3f);
	}
}
