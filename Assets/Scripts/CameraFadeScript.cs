using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFadeScript : MonoBehaviour {

	public GameObject cameraScreen;
	public GameObject outOfBoundsCanvas;

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "CraneCockpit"){
			cameraScreen.SetActive(true);
			outOfBoundsCanvas.SetActive(true);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "CraneCockpit"){
			cameraScreen.SetActive(false);
			outOfBoundsCanvas.SetActive(false);
		}
	}
}
