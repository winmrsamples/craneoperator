using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class ToggleShowHideScript : MonoBehaviour, IInputClickHandler, IInputHandler {
	
	public GameObject objectToToggle;
	public GameObject altObjectToToggle;
	private bool hidden = false; //leaving here for handling fade in/out later

	// Use this for initialization
	void Start () {
		if (!objectToToggle.activeSelf) {
			hidden = true;
		}
	}

	public void OnInputClicked(InputClickedEventData eventData)
	{
		altObjectToToggle.SetActive(false);
		objectToToggle.SetActive(!objectToToggle.activeSelf);
	}
	
	public void OnInputDown(InputEventData eventData)
  	{ }

	public void OnInputUp(InputEventData eventData)
	{ }
}
