using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent (typeof (PlayerController))]
[RequireComponent (typeof (GunController))]
public class Player : LivingEntity {

	public float moveSpeed = 5;

	PlayerController controller;

	private bool firstSetup = true;
	public bool spawned { get; private set; }

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
			EnableComponents (false);
			firstSetup = false;
		} else {
			SetDefaults ();
		}
	}

	// TODO: Fix
	public override void OnStartLocalPlayer () {
		GetComponent<MeshRenderer>().material.color = Color.blue;
	}

	void Start() {
		controller = GetComponent<PlayerController> ();
		SyncManager.instance.OnNewWave += OnNewWave;
		Spawner spawner = FindObjectOfType<Spawner> ();
		spawner.setPlayer = this;
		FindObjectOfType<Scoreboard> ().setPlayer = this;
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
		startingHealth = (int)(startingHealth * 1.2f);
		Heal (startingHealth);

		if (!spawned) {
			SetupPlayer ();
			spawned = true;
		}
	}

	public override void Die () {
		AudioManager.instance.PlaySound ("Player Death", transform.position);
		base.Die ();

		EnableComponents (false);

		//Spawn a death effect
		GameObject _gfxIns = (GameObject)Instantiate(Resources.Load("Explosion"), transform.position, Quaternion.identity);
		Destroy(_gfxIns, 3f);

		//Switch cameras
		if (isLocalPlayer) {
			GameManager.instance.SetSceneCameraActive(true);
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

		if (isLocalPlayer) {
			//Switch cameras
			GameManager.instance.SetSceneCameraActive(false);
			GetComponent<PlayerSetup>().gameUI.SetActive(true);
		}

		spawned = false;

		Debug.Log(transform.name + " respawned.");
	}

	public void SetDefaults (){
		dead = false;

		startingHealth = 80;
		health = startingHealth;

		EnableComponents (true);

		//Create spawn effect
		GameObject _gfxIns = (GameObject)Instantiate(Resources.Load("SpawnEffect"), transform.position, Quaternion.identity);
		Destroy(_gfxIns, 3f);
	}

	private void EnableComponents(bool enabled) {
		
		//Enable the collider
		Collider _col = GetComponent<Collider>();
		if (_col != null)
			_col.enabled = enabled;

		//Enable the rigidbody
		Rigidbody _rig = GetComponent<Rigidbody>();
		if (_rig != null) {
			_rig.detectCollisions = enabled;
			_rig.isKinematic = !enabled;
		}

		//Enable the mesh renderer
		MeshRenderer renderer = GetComponent<MeshRenderer>();
		Color c = renderer.material.color;
		renderer.material.shader = Shader.Find( "Transparent/Diffuse" );
		renderer.material.color = enabled ? new Color (c.r, c.g, c.b, 1f) : new Color (c.r, c.g, c.b, 0.5f);
	}
}
