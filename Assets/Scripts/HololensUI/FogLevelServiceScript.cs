using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class FogLevelServiceScript : MonoBehaviour {
	// Interactive Toggle states don't work when calling network behaviours ([Command])
	// so we service them through seperate scripts on the client.

	public GameManager gameManager;
	public Renderer fogCubeRenderer;
	public GameObject radioButtons;
	public GameObject toggleButtonIcon;
	public Material iconHigh;
	public Material iconMed;
	public Material iconNone;
	
	public void SetFogLevel (int fogLevel) {
		if (fogLevel == 0){
			toggleButtonIcon.GetComponent<Renderer>().material = iconNone;
		} else if (fogLevel == 1) {
			toggleButtonIcon.GetComponent<Renderer>().material = iconMed;
		} else {
			toggleButtonIcon.GetComponent<Renderer>().material = iconHigh;
		}
		gameManager.CmdSetFogLevel(fogLevel);
		fogCubeRenderer.material.color = new Color(0.9f, 0.9f, 0.9f, 0.08f * fogLevel);
		StartCoroutine(WaitForSeconds(0.3f));
	}

	private IEnumerator WaitForSeconds(float Seconds) {
        yield return new WaitForSeconds(Seconds);
		radioButtons.SetActive(false);
	}
}
