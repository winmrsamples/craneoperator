using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.XR.WSA;

public class CraneDemoNetDiscovery : NetworkDiscovery
{
    public CraneDemoNetManager networkManager;
    const string CrossDeviceDemoAppDataHeader = "CrossDeviceDemo"; 

    void Start(){
        if (!HolographicSettings.IsDisplayOpaque) {
            StartAsClient();
            StartCoroutine(Hack());

        } else {
            broadcastData = CrossDeviceDemoAppDataHeader; 
            StartAsServer();
        }
    }
    
    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        if (data == CrossDeviceDemoAppDataHeader)
        {
            networkManager.networkAddress = fromAddress;
            networkManager.StartClientWithAddress();
        } 
    }

    bool isConnected = false;
    int retries = 30;

    IEnumerator Hack()
    {
       
        do
        {
            yield return new WaitForSeconds(5);
            if (!isConnected)
            {
                Debug.Log("Using Manual Connection");
                networkManager.networkAddress = "::ffff:192.168.124.123";

                networkManager.StartClientWithAddress();
                yield return new WaitForSeconds(2);
                isConnected = networkManager.client.isConnected;
            }

        } while (--retries > 0 && isConnected == false);


    }
}
