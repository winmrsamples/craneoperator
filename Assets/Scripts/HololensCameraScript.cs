using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;

public class HololensCameraScript : MonoBehaviour {

	void Start () {
		GetComponent<AudioListener>().enabled = true;
		if (!HolographicSettings.IsDisplayOpaque){
			GameObject[] HMDUOnlyObjects = GameObject.FindGameObjectsWithTag("HMDUOnly");
			for (int i = 0; i < HMDUOnlyObjects.Length; i++){
				HMDUOnlyObjects[i].SetActive(false);
			}
		}
	}
}
