using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class togglePickUpInstructions : MonoBehaviour {

	public GameObject pickUpInstructions;
	public bool sceneActive = false;

	void Update()
	{
		if (sceneActive && GameObject.FindGameObjectWithTag("WeightPickUpZone") != null){
			if (GameObject.FindGameObjectWithTag("WeightPickUpZone").GetComponent<PickUpScript>().isHooked){
				pickUpInstructions.SetActive(false);
			}
		} else if (GameObject.FindGameObjectWithTag("WeightPickUpZone") != null){
			 sceneActive = true;
		} else {
			 sceneActive = false;
		}
	}

	void OnTriggerEnter(Collider weight){
		if (weight.gameObject.tag == "WeightPickUpZone"){
			if (GameObject.FindGameObjectWithTag("WeightPickUpZone").GetComponent<PickUpScript>().isHooked == false) {
				pickUpInstructions.SetActive(true);
			}
		}
	}

	void OnTriggerExit(Collider weight){
		if (weight.gameObject.tag == "WeightPickUpZone"){
			if (GameObject.FindGameObjectWithTag("WeightPickUpZone").GetComponent<PickUpScript>().isHooked == false) {
				pickUpInstructions.SetActive(false);
			}
		}
	}
}
