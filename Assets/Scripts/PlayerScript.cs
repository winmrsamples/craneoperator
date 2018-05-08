using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;
using UnityEngine.Networking;

public class PlayerScript : NetworkBehaviour {

	private GameObject gameManager;
	private GameObject CraneObject;
	private GameObject CraneDummy;

	void Start () {
		gameManager = GameObject.FindGameObjectWithTag("GameController");
		CraneObject = GameObject.FindGameObjectWithTag("PlaceableObject");
		CraneDummy = GameObject.FindGameObjectWithTag("PlaceableDummy");

		if (!isServer) {
			CmdGiveAuthority(GetComponent<NetworkIdentity>());
		} else {
			gameManager.GetComponent<GameManager>().CmdUpdateHololens();
		}

		if (!HolographicSettings.IsDisplayOpaque) {
			if (CraneObject.transform.position != CraneDummy.transform.position){
				CraneObject.transform.position = CraneDummy.transform.position;
				CraneObject.transform.rotation = CraneDummy.transform.rotation;
			}
			CraneObject.AddComponent<WorldAnchor>();
			CraneDummy.SetActive(false);
			GameObject.FindGameObjectWithTag("HololensLobbyScreen").SetActive(false);
		}
	}

	[Command]
	public void CmdGiveAuthority(NetworkIdentity name)
	{
		gameManager = GameObject.FindGameObjectWithTag("GameController");
		NetworkIdentity gameManagerID = gameManager.GetComponent<NetworkIdentity>();
		gameManagerID.AssignClientAuthority(connectionToClient);
	}
}
