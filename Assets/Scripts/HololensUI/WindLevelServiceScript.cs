using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindLevelServiceScript : MonoBehaviour {
	// Interactive Toggle states don't work when calling network behaviours ([Command])
	// so we service them through seperate scripts on the client.

	public GameManager gameManager;
	public GameObject radioButtons;
	public GameObject toggleButtonIcon;
	public Material iconHigh;
	public Material iconMed;
	public Material iconNone;
	
	public void SetWindLevel (float windLevel) {
		if (windLevel == 0f){
			toggleButtonIcon.GetComponent<Renderer>().material = iconNone;
		} else if (windLevel == 0.5f) {
			toggleButtonIcon.GetComponent<Renderer>().material = iconMed;
		} else {
			toggleButtonIcon.GetComponent<Renderer>().material = iconHigh;
		}
		gameManager.CmdSetWindLevel(windLevel);
		StartCoroutine(WaitForSeconds(0.3f));
	}

	private IEnumerator WaitForSeconds(float Seconds) {
        yield return new WaitForSeconds(Seconds);
		radioButtons.SetActive(false);
	}
}
