using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinishZoneScript : MonoBehaviour {

	public Slider slider;
	public PickUpScript pickUpScript;
	private float timer = 0;
	private bool completed = false;
	private bool insideZoneCurrently = false;

	
	void Start()
	{
		pickUpScript = GameObject.Find("MeshedObjectsPlaceable/WeightContainer/WeightPickUpRange").GetComponent<PickUpScript>();
	}

	void OnTriggerStay(Collider ball){
		if (!pickUpScript.isHooked) {
			if (ball.gameObject.tag == "Weight" && timer < 3) {
				insideZoneCurrently = true;
				timer += (Time.deltaTime * 2);
				slider.value = timer;
			}
			if (ball.gameObject.tag == "Weight" && slider.value == 3 && !completed) {
				completed = true;
				GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().CompleteTest();
			}
		}
	}
	
	void FixedUpdate()
	{
		if (slider.value > 0 && !completed) {
			slider.value -= Time.deltaTime;
			timer = slider.value;
		}
	}


}
