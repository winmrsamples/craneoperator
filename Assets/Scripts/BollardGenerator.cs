using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;
using UnityEngine.Networking;
using HoloToolkit.Unity.InputModule;
using  HoloToolkit.Unity;


public class BollardGenerator : NetworkBehaviour {

	public GameObject bollardPrefab;
	public GameObject weight;
	public GameObject finishZonePrefab;
	public Transform groundHeight;
	public Transform craneLocation;
	[Tooltip("must be no larger than 180")]
	public int rotationDeviation;
	public float corrodiorWidth;
	public float gapBetweenBollards;
	public GameObject meshedObjectsPlaceable;
	public GameObject positionMarker;

	//Add randomisation later
	public float maxSideLength;
	public float minSideLength;
	public float maxLength;
	public float minLength;
	public bool bollardsSpawned = false;
	

	private Transform startPosition;
	// private GameObject weightInstance;
	private Vector3 ballPosition;
	public GameObject obsticleCourseContainer;
	public GameObject groundLevel;
	private bool finishedInstantiating = false;
	private GameObject finishZoneInstance;
	private GameObject obsticleCourseContainerInstance;
	private GameObject positionMarkerInstance;
	private Vector3[] startLeft = new Vector3[2];
	private Vector3[] startRight = new Vector3[2];
	private Vector3[] endLeft = new Vector3[2];
	private Vector3[] endRight = new Vector3[2];
	private Vector3[] sideLine1 = new Vector3[2];
	private Vector3[] sideLine2 = new Vector3[2];
	private Vector3[] positionsToSend = new Vector3[100];
	private Vector3 startingWeightPos = new Vector3(0f, 0f, 0f);
	private Vector3 finishingZonePos = new Vector3(0f, 0f, 0f);
	private int rotationAmount;

	
	void Start()
	{
		obsticleCourseContainerInstance = Instantiate(obsticleCourseContainer, new Vector3(0f, 0f, 2f), Quaternion.identity);
		startPosition = GameObject.FindGameObjectWithTag("StartingPosition").transform;

		if (!HolographicSettings.IsDisplayOpaque) {
			bollardPrefab.GetComponent<Rigidbody>().isKinematic = true;
			bollardPrefab.GetComponent<CapsuleCollider>().isTrigger = true;
			// obsticleCourseContainerInstance.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
			// obsticleCourseContainerInstance.transform.rotation = startPosition.rotation;
			groundHeight.position = new Vector3 (groundHeight.position.x, groundHeight.position.y * -185, groundHeight.position.z); // 1.3 / 0.005 = 260
		} else {
			bollardPrefab.GetComponent<Rigidbody>().isKinematic = false;
		}
		finishedInstantiating = true;
	}
	
	public void updateStartingPosition()
	{
		if (GameObject.FindGameObjectWithTag("StartingPosition") != null) {
			startPosition = GameObject.FindGameObjectWithTag("StartingPosition").transform;
		}
	}

	[Command]
	public void CmdPlaceBollards(){
		CmdSetUpPoints();
		rotationAmount = Random.Range(-rotationDeviation, rotationDeviation);
		RpcChangeBollards(rotationAmount, false);
	}

	[Command]
	void CmdGiveAuthority(NetworkInstanceId bollardContainerId){
		NetworkServer.SpawnWithClientAuthority(GameObject.FindGameObjectWithTag("BollardContainer"), connectionToClient);
	}

	// Rotate the bollards to a random position, handle the scale offset between experiences.
	[ClientRpc]
	void RpcChangeBollards(int rotationAmount, bool hololensOnly){
		if ((hololensOnly && !HolographicSettings.IsDisplayOpaque) || !hololensOnly) {
			StartCoroutine(ChangeBollardsIfLoaded(rotationAmount));
		}
	}

	IEnumerator ChangeBollardsIfLoaded(int rotationAmount)
    {
        yield return new WaitUntil(() => finishedInstantiating == true);
		float heightOffset = 0f;
		
		if (!HolographicSettings.IsDisplayOpaque) {
			obsticleCourseContainerInstance.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
			obsticleCourseContainerInstance.transform.rotation = startPosition.rotation;
		}


		startPosition = GameObject.FindGameObjectWithTag("StartingPosition").transform;
		obsticleCourseContainerInstance.transform.position = startPosition.position + new Vector3(0f, heightOffset, 0f);
		obsticleCourseContainerInstance.transform.RotateAround(craneLocation.position, obsticleCourseContainerInstance.transform.up, rotationAmount);
		obsticleCourseContainerInstance.transform.RotateAround(obsticleCourseContainerInstance.transform.position, obsticleCourseContainerInstance.transform.up, 160f);

		weight.transform.position = positionMarkerInstance.transform.position;
		weight.transform.rotation = positionMarkerInstance.transform.rotation;


		weight.SetActive(true);
		if (!HolographicSettings.IsDisplayOpaque){
			obsticleCourseContainerInstance.transform.parent = meshedObjectsPlaceable.transform;
			GameObject.FindGameObjectWithTag("WeightPickUpIndicator").SetActive(false);
			if (WorldAnchorManager.Instance != null) {
				WorldAnchorManager.Instance.AttachAnchor(obsticleCourseContainerInstance, "obsticleContainer");
			}
		}
		if (isServer){
			weight.GetComponent<Rigidbody>().isKinematic = false;
		}

		bollardsSpawned = true;
	}



	[Command]
	public void CmdReplaceBollardsInHololens(){
		RpcIntantiateBollards(startLeft, true);
		RpcIntantiateBollards(startRight, true);
		RpcIntantiateBollards(endLeft, true);
		RpcIntantiateBollards(endRight, true);
		RpcIntantiateBollards(sideLine1, true);
		RpcIntantiateBollards(sideLine2, true);
		RpcIntantiateWeightAndFinishZone(finishingZonePos, startingWeightPos, true);
		RpcChangeBollards(rotationAmount, true);
		// And probably have to update the transform after this too.
	}

	[ClientRpc]
	public void RpcRemoveBollards(){
		GameObject[] currentBollards = GameObject.FindGameObjectsWithTag("Bollard");
		foreach (GameObject bollard in currentBollards) {
			Destroy(bollard);
		}

		if (GameObject.FindGameObjectWithTag("WeightPickUpZone") != null){
			GameObject.FindGameObjectWithTag("WeightPickUpZone").GetComponent<PickUpScript>().ResetWeight();
		}
		weight.SetActive(false);
		if (isServer){
			weight.GetComponent<Rigidbody>().isKinematic = true;
		}

		Destroy(GameObject.FindGameObjectWithTag("FinishZone"));
		Destroy(obsticleCourseContainerInstance);
		obsticleCourseContainerInstance = Instantiate(obsticleCourseContainer, new Vector3(0f, 0f, 2f), Quaternion.identity);
	}

	// Set up the points for the bollards to be placed inbetween.
	[Command]
	void CmdSetUpPoints(){
		float startLength = Random.Range(minLength, maxLength); // randomize
		float endLength = Random.Range(minLength, maxLength); // randomize
		float sideLength = Random.Range(minSideLength, maxSideLength); // randomize
		
		//STARTING LEFT LINE
		startLeft[0] = startPosition.position;
		Vector3 endPointOfLeftSide = startPosition.position + new Vector3(0f, 0f, startLength);
		startLeft[1] = endPointOfLeftSide;

		//STARTING RIGHT LINE
		startRight[0] = startPosition.position + new Vector3(corrodiorWidth, 0f, 0f);
		Vector3 endPointOfRightSide = startPosition.position + new Vector3(corrodiorWidth, 0f, startLength);
		startRight[1] = endPointOfRightSide;

		endLeft[0] = startPosition.position + new Vector3(sideLength, 0f, startLength + corrodiorWidth);
		endLeft[1] = endLeft[0] + new Vector3(0f, 0f, endLength);

		endRight[0] = endLeft[0] + new Vector3(corrodiorWidth, 0f, 0f);
		endRight[1] = endLeft[1] + new Vector3(corrodiorWidth, 0f, 0f);
		
		if (sideLength > 0) {
			startLeft[1].z += corrodiorWidth;
			endRight[0].z -= corrodiorWidth;
		} else {
			startRight[1].z += corrodiorWidth;
			endLeft[0].z -= corrodiorWidth;
		}

		sideLine1[0] = startLeft[1];
		sideLine1[1] = endLeft[0];
		sideLine2[0] = startRight[1];
		sideLine2[1] = endRight[0];

		startingWeightPos = new Vector3(startLeft[0].x + corrodiorWidth/2, 0f, startPosition.position.z);
		finishingZonePos = new Vector3(endLeft[1].x + corrodiorWidth/2, 0f, endLeft[1].z + 5);

		RpcIntantiateWeightAndFinishZone(finishingZonePos, startingWeightPos, false);

		RpcIntantiateBollards(startLeft, false);
		RpcIntantiateBollards(startRight, false);

		RpcIntantiateBollards(endLeft, false);
		RpcIntantiateBollards(endRight, false);

		RpcIntantiateBollards(sideLine1, false);
		RpcIntantiateBollards(sideLine2, false);
	}

	[ClientRpc]
	void RpcIntantiateWeightAndFinishZone(Vector3 finishingZonePosition, Vector3 weightPosition, bool hololensOnly){
		StartCoroutine(InstantiateWeightAndFinishZoneIfLoaded(finishingZonePosition, weightPosition, hololensOnly));
	}

	IEnumerator InstantiateWeightAndFinishZoneIfLoaded(Vector3 finishingZonePosition, Vector3 weightPosition, bool hololensOnly)
    {
		yield return new WaitUntil(() => finishedInstantiating == true);
		if ((hololensOnly && !HolographicSettings.IsDisplayOpaque) || !hololensOnly) {
			weightPosition += new Vector3(0f, groundHeight.transform.position.y, 0f);
			finishingZonePosition += new Vector3(0f, groundHeight.transform.position.y + finishZonePrefab.transform.localScale.y * 1.7f, 0f);
			
			finishZoneInstance = Instantiate(finishZonePrefab, finishingZonePosition, Quaternion.identity); 
			finishZoneInstance.transform.parent = obsticleCourseContainerInstance.transform;

			if (!HolographicSettings.IsDisplayOpaque){
				finishZoneInstance.transform.position += new Vector3(0f, .9f, 0f);
			}

			positionMarkerInstance = Instantiate(positionMarker, weightPosition, Quaternion.Euler(Quaternion.identity.x, 80f, Quaternion.identity.z));
			positionMarkerInstance.transform.parent = obsticleCourseContainerInstance.transform;
		}
	}

	// Place bollards between the two given vectors.
	[ClientRpc]
	void RpcIntantiateBollards(Vector3[] line, bool hololensOnly){
		float bollardInstantiateHeight = groundHeight.transform.position.y;
		StartCoroutine(InstantiateBollardsIfLoaded(line, hololensOnly, bollardInstantiateHeight));
	}

	[ClientRpc]
	public void RpcRemoveHitBollard(float[] id){
		if (!HolographicSettings.IsDisplayOpaque){
			StartCoroutine(RemoveHitBollardAfterTestSetup(id));
		}
	}

	IEnumerator RemoveHitBollardAfterTestSetup(float[] id)
    {
        yield return new WaitUntil(() => bollardsSpawned == true);
		GameObject[] AllBollards = GameObject.FindGameObjectsWithTag("Bollard");
		for (int i = 0; i < AllBollards.Length; i++) {
			if (AllBollards[i].GetComponent<BollardScript>().Id[0] == id[0] && AllBollards[i].GetComponent<BollardScript>().Id[1] == id[1]){
				Destroy(AllBollards[i]);
			}
		}
	}

	//The RPC may be called on the client before the start method has run.
	IEnumerator InstantiateBollardsIfLoaded(Vector3[] line, bool hololensOnly, float bollardY)
    {
        yield return new WaitUntil(() => finishedInstantiating == true);
		if (hololensOnly) {
			bollardY += bollardPrefab.transform.localScale.y;
		}

        if ((hololensOnly && !HolographicSettings.IsDisplayOpaque) || !hololensOnly) {
			// If x values are the same we are working with a vertical line
			line[0].y = bollardY;

			if (line[0].x == line[1].x) {
				for (float i = line[0].z; i < line[1].z; i += gapBetweenBollards) {
					GameObject bollardInstance = Instantiate(bollardPrefab, new Vector3(line[0].x, line[0].y, i) , startPosition.rotation, obsticleCourseContainerInstance.transform);
					bollardInstance.GetComponent<BollardScript>().Id[0] = line[0].x; // Used to identify bollards between devices
					bollardInstance.GetComponent<BollardScript>().Id[1] = i;
				}
			} else if (line[0].x > line[1].x) {
				for (float i = line[0].x; i > line[1].x; i -= gapBetweenBollards) {
					GameObject bollardInstance = Instantiate(bollardPrefab, new Vector3(i, line[0].y, line[0].z) , startPosition.rotation, obsticleCourseContainerInstance.transform);
					bollardInstance.GetComponent<BollardScript>().Id[0] = i; // Used to identify bollards between devices
					bollardInstance.GetComponent<BollardScript>().Id[1] = line[0].z;
				}
			} else if (line[0].x < line[1].x) {
				for (float i = line[0].x; i < line[1].x; i += gapBetweenBollards) {
					GameObject bollardInstance = Instantiate(bollardPrefab, new Vector3(i, line[0].y, line[0].z) , startPosition.rotation, obsticleCourseContainerInstance.transform);
					bollardInstance.GetComponent<BollardScript>().Id[0] = i; // Used to identify bollards between devices
					bollardInstance.GetComponent<BollardScript>().Id[1] = line[0].z;
				}
			}
		}
    }
}
