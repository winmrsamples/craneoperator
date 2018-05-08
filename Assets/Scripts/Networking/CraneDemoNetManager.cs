using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.XR.WSA;
using HoloToolkit.Unity.InputModule;

public class CraneDemoNetManager : NetworkManager
{

	public NetworkDiscovery discovery;
	public NetworkMigrationManager migration;
	public GameManager gameManager;
    public GameObject meshedObjectsPlaceable;
    public GameObject craneSceneDummyPrefab;
    private GameObject meshedObjectsPlaceableHololens;

    private int noOfPlayersConnected = 0;

	void Start () {
        if (UnityEngine.XR.XRSettings.enabled)
        {
            // if HMD start Host, else start client
            if (HolographicSettings.IsDisplayOpaque)
            {
                StartHost();
            }
            else
            {
                meshedObjectsPlaceableHololens = Instantiate(craneSceneDummyPrefab, craneSceneDummyPrefab.transform.position, craneSceneDummyPrefab.transform.rotation);
                discovery.Initialize();
            }
        }
    }

	public override void OnStartHost()
	{
		discovery.Initialize();
	}


	public override void OnStartClient(NetworkClient client)
	{
		discovery.showGUI = false;
        discovery.StopBroadcast();
	}

    public override void OnServerConnect(NetworkConnection conn)
    {
        // if (conn.connectionId != 0)
        noOfPlayersConnected ++;
        if (noOfPlayersConnected == 2)
        {
            gameManager.CmdCompleteLoading();
        }
    }

	public override void OnStopClient()
	{
		discovery.StopBroadcast();
		discovery.showGUI = true;
	}

    public void StartClientWithAddress(){
		StartClient();
	}

    public override void OnServerDisconnect(NetworkConnection connection)
    {
        discovery.StartAsServer();
    }

    public override void OnClientDisconnect(NetworkConnection connection)
    {
        if (!HolographicSettings.IsDisplayOpaque){
            if (meshedObjectsPlaceableHololens != null) {
                meshedObjectsPlaceableHololens.SetActive(true);
            }
            if (meshedObjectsPlaceable != null) {
                meshedObjectsPlaceable.GetComponent<TapToPlace>().enabled = true;
                Destroy(meshedObjectsPlaceable.GetComponent<WorldAnchor>());
            }

            GameObject[] bollards = GameObject.FindGameObjectsWithTag("Bollard");
            for (int i = 0; i < bollards.Length; i++){
				Destroy(bollards[i]);
			}
        }

        StopClient();
        networkAddress = "localhost";
        discovery.Initialize();
        discovery.StartAsClient();
    }
}
