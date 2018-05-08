using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.WSA;
using UnityEngine.XR;
using HoloToolkit.Examples.InteractiveElements;

public class GameManager : NetworkBehaviour {

	public CraneMovementScript noPhysicsScript;
	public BollardGenerator bollardGenerator;
	public HookScript hookScript;
	public GameObject fogManager;
	public GameObject fakeRopeCylinder;
	public GameObject hololensRopeCylinder;
	public GameObject fakeHookCylinder;
	public GameObject meshedScene;
	public GameObject cityScape;
	public GameObject testCompleteCanvas;
	public GameObject cameraScreenCanvas;
	public GameObject HMDUCockpitObjects;
	public GameObject HololensUI;
	public GameObject HololensWindRadio;
	public GameObject HololensFogRadio;
	public GameObject craneArm;
	public GameObject bollardsHitText;
	public GameObject weightContainer;
	public Rigidbody sled;
	public Transform sledWorldPosition;
	public ResetSceneButton resetSceneButton;
	public GameObject particleFog;

#if DEBUG
    public bool runStandAloneOnOccluded = false; 
    public bool runStandAloneOnHololens = false ; 
#endif


    // private GameObject fogManagerInstance;
	private GameObject fakeRopeCylinderInstance;
	private GameObject fakeHookCylinderInstance;
	private bool bollardsPlaced = false;
	private bool windIsBlowing = false;
	private float windStrength = 0;
	private int fogLevel = 0;
	private Vector3 windDirection = new Vector3(0f, 0f, 0f);
	private Vector3 craneArmStartingPos;
	private Vector3 cockpitArmStartingPos;
	private int numberOfBollardsHit = 0;
	private BollardScript[] hitBollards = new BollardScript[30];

	
	Color HeavyCloudColor = new Color(0.68f, 0.68f, 0.68f, 0.9f);
	Color MediumCloudColor = new Color(0.68f, 0.68f, 0.68f, 0.9f);
	// Color HeavyMainColor = new Color(0.62f, 0.62f, 0.62f, 0.093f);
	// Color HeavySecondColor = new Color(0.31f, 0.31f, 0.31f, 0.31f);

	// Color MediumMainColor = new Color(0.62f, 0.62f, 0.62f, 0.063f);
	// Color MediumSecondColor = new Color(0.31f, 0.31f, 0.31f, 0.2f);



    // Use this for initialization
    void Start () {
		// IF NOT HOLOLENS
		if (HolographicSettings.IsDisplayOpaque){
			bool testTrackingType = XRDevice.SetTrackingSpaceType(TrackingSpaceType.Stationary);
			craneArmStartingPos = craneArm.transform.localPosition;
			cockpitArmStartingPos = HMDUCockpitObjects.transform.localPosition;
			
			meshedScene.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
		
			GameObject[] hololensOnlyObjects = GameObject.FindGameObjectsWithTag("HololensOnly");
			for (int i = 0; i < hololensOnlyObjects.Length; i++){
				hololensOnlyObjects[i].SetActive(false);
			}

			fakeHookCylinderInstance = Instantiate(fakeHookCylinder, sledWorldPosition.position, Quaternion.identity);

			fakeRopeCylinder.GetComponent<ConfigurableJoint>().connectedBody = sled;
			fakeRopeCylinder.GetComponent<Rigidbody>().isKinematic = false;
			noPhysicsScript.rope = fakeRopeCylinder;
			noPhysicsScript.hook = fakeHookCylinderInstance;
			fakeHookCylinderInstance.transform.parent = fakeRopeCylinder.transform;
			fakeHookCylinderInstance.transform.position -= new Vector3(0f, 19.5f, 0f);

#if DEBUG
            if (runStandAloneOnOccluded)
            {
                StartCoroutine(StartWithDelay(10));  
            }
#endif 
			StartCoroutine(LoadBundle());
		} else {
			GameObject[] HMDUOnlyObjects = GameObject.FindGameObjectsWithTag("HMDUOnly");
			for (int i = 0; i < HMDUOnlyObjects.Length; i++){
				HMDUOnlyObjects[i].SetActive(false);
			}
		}
	}

	IEnumerator LoadBundle() {
		//This tries to load the asset bundle that is provided as a package.
		var cityAssetBundle = AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, "cityscapesmall"));
		yield return cityAssetBundle;
		
		var loadedAssetBundle = cityAssetBundle.assetBundle;
		if (loadedAssetBundle == null)
		{
			Debug.Log("Failed to load AssetBundle for city");
			yield break;
		}

		var cityPrefab = loadedAssetBundle.LoadAssetAsync<GameObject>("CityScapeSmall");
		yield return cityPrefab;

		GameObject prefab = cityPrefab.asset as GameObject;
		Instantiate(prefab);

		loadedAssetBundle.Unload(false);
	}

#if DEBUG
    IEnumerator StartWithDelay ( float delay ) 
    { 
        yield return new WaitForSeconds (delay); 
        CmdCompleteLoading(); 
        yield return new WaitForSeconds(2); 
        CmdPlaceBollards(); 
    } 
#endif 

    public void CompleteTest(){
		noPhysicsScript.gamePaused = true;
		testCompleteCanvas.SetActive(true);
		cameraScreenCanvas.SetActive(true);
		numberOfBollardsHit = 0;
		bollardsHitText.GetComponent<TextMesh>().text = "Bollards Hit: " + numberOfBollardsHit.ToString();
		GameObject.FindGameObjectWithTag("Weight").GetComponent<Rigidbody>().isKinematic = true;
		StartCoroutine(ResetGameAfterSeconds(5f));
	}

	private IEnumerator ResetGameAfterSeconds(float Seconds) {
        yield return new WaitForSeconds(Seconds);
		
		bollardsPlaced = false;
		bollardGenerator.RpcRemoveBollards();

		if (HolographicSettings.IsDisplayOpaque) {
			testCompleteCanvas.SetActive(false);
			cameraScreenCanvas.SetActive(false);
			noPhysicsScript.gamePaused = false;
		}

		CmdSetFogLevel(0);
		CmdSetWindLevel(0);
	}

	[Command]
	public void CmdCompleteLoading(){
		GameObject[] HMDULoadingObjects = GameObject.FindGameObjectsWithTag("HMDULobbyScreen");
		for (int i = 0; i < HMDULoadingObjects.Length; i++){
			HMDULoadingObjects[i].SetActive(false);
		}

        var indicator = GameObject.FindGameObjectWithTag("WeightPickUpIndicator"); 
        if ( indicator != null )
        {
            indicator.SetActive(false); 
        } 
	}


	[Command]
	public void CmdSetWindLevel(float strength){
		if (windDirection == new Vector3(0f, 0f, 0f) || strength == 0){
			while (windDirection.x < 0.5f && windDirection.x > -0.5f) {
				windDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
			}
		}

		windStrength = strength;
		if (HolographicSettings.IsDisplayOpaque) {
			fakeRopeCylinder.GetComponent<WindManager>().SetWindLevel(windStrength, windDirection);
		}
	}

	// 0 = no fog, 1 = medium, 2 = heavy
	[Command]
	public void CmdSetFogLevel(int levelOfFogEnum){
		fogLevel = levelOfFogEnum;

		if (levelOfFogEnum == 0 && particleFog.activeSelf) {
			particleFog.SetActive(false);
		} else if (levelOfFogEnum != 0 && !particleFog.activeSelf) {
			particleFog.SetActive(true);
		}

		ParticleSystem ps = particleFog.GetComponent<ParticleSystem>();
		if (particleFog.activeSelf) {
			if (levelOfFogEnum == 1){
				 var mainModule = ps.main;
				  mainModule.startColor = MediumCloudColor;
			}
			if (levelOfFogEnum == 2){
				 var mainModule = ps.main;
				mainModule.startColor = HeavyCloudColor;
			}
		}
	}

	[Command]
	public void CmdRemoveBollards(){
		bollardsPlaced = false;
		bollardGenerator.RpcRemoveBollards();
		RpcResetHitCount();
	}

	[ClientRpc]
	void RpcResetHitCount(){
		numberOfBollardsHit = 0;
		bollardsHitText.GetComponent<TextMesh>().text = "Bollards Hit: " + numberOfBollardsHit.ToString();
	}

	[Command]
	public void CmdPlaceBollards(){
		bollardsPlaced = true;
		RpcTryPlaceThroughClient();
	}

	[ClientRpc]
	void RpcTryPlaceThroughClient(){
		if (HolographicSettings.IsDisplayOpaque) {
			bollardGenerator.RpcRemoveBollards();
			bollardGenerator.CmdPlaceBollards();
		} else {
			hololensRopeCylinder.SetActive(true);
		}
	}
	
	[Command]
	public void CmdRegisterHitBollard(float[] id){
		RpcRegisterHitBollard(id);
	}

	//FIND THE KNOCKED OVER BOLLARD ON HOLOLENS
	[ClientRpc]
	public void RpcRegisterHitBollard(float[] id){
		if (!HolographicSettings.IsDisplayOpaque) { 
			GameObject[] bollards = GameObject.FindGameObjectsWithTag("Bollard");
			for (int i = 0; i < bollards.Length; i++){
				BollardScript bollardInArray = bollards[i].GetComponent<BollardScript>();
				if (bollardInArray.Id[0] == id[0] && bollardInArray.Id[1] == id[1]){
					bollardInArray.SetToHit();
				}
			}
		}
		numberOfBollardsHit++;
		bollardsHitText.GetComponent<TextMesh>().text = "Bollards Hit: " + numberOfBollardsHit.ToString();
	}
	public void CmdUpdateHololens(){
		RpcUpdateHololens(fogLevel, windStrength, bollardsPlaced);
		if (bollardsPlaced) {
			bollardGenerator.CmdReplaceBollardsInHololens();
		}
		//Unet doesn't support components or multidimensional arrays in RPC calls so pass the ids individualy.
		for (int i = 0; i < hitBollards.Length; i++){
			if (hitBollards[i] != null){
				bollardGenerator.RpcRemoveHitBollard(hitBollards[i].Id);
			}
		}
	}

	[ClientRpc]
	public void	RpcUpdateHololens(int fogLevel, float windLevel, bool areBollardsPlaced){
		if (!HolographicSettings.IsDisplayOpaque) {
			HololensUI.GetComponent<FogLevelServiceScript>().SetFogLevel(fogLevel);
			HololensFogRadio.GetComponent<InteractiveSet>().HandleOnSelection(fogLevel);
			HololensWindRadio.GetComponent<InteractiveSet>().HandleOnSelection((int)windLevel);
			
			if (areBollardsPlaced) {
				weightContainer.SetActive(true);
				resetSceneButton.SetButtonToClear();
			}
		}
	}


	public void RegisterHitBollard(BollardScript bollardScript){
		for (int i = 0; i < hitBollards.Length; i++){
			if (hitBollards[i] == null) {
				hitBollards[i] = bollardScript;
				return;
			}
		}
	}

	void Update()
	{
		if (Input.GetButton("CONTROLLER_RIGHT_STICK_PRESS")){
			InputTracking.Recenter();
		}


		
		// FOR TESTING
		if (Input.GetKeyDown("6")){
			CmdSetFogLevel(0);
		}
		if (Input.GetKeyDown("7")){
			CmdSetFogLevel(1);
		}
		if (Input.GetKeyDown("8")){
			CmdSetFogLevel(2);
		}
		if (Input.GetKeyDown("r")){
			CmdPlaceBollards();
		}
		if (Input.GetKeyDown("1")){
			CompleteTest();
		}
	}
}
