using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CraneDemoNetMigration : NetworkMigrationManager {

	void Start(){
		Debug.Log("start net migration manager");
	}

	public bool LostHostOnClient(NetworkConnection conn){
		Debug.Log("Disconnected with host on client LostHostOnClient");
		return true;
	}

	protected virtual void OnClientDisconnectedFromHost(NetworkConnection conn, out SceneChangeOption sceneChange){
		Debug.Log("Migration >> On Client Disconnected From Host");
		base.OnClientDisconnectedFromHost(conn, out sceneChange);
 
		// BecomeNewHost(7777);
	}
}