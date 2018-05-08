using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;

public class HookScript : MonoBehaviour {
	public bool connected = false;

	void FixedUpdate () {
		if (!connected && HolographicSettings.IsDisplayOpaque) {
			if (GameObject.FindGameObjectWithTag("Hook") != null) {
				transform.position = new Vector3 (40f, 110f, 20f);
				transform.rotation = Quaternion.identity;
				GetComponent<ConfigurableJoint>().connectedBody = GameObject.FindGameObjectWithTag("Hook").GetComponent<Rigidbody>();
				connected = true;
			}
		} else if (HolographicSettings.IsDisplayOpaque){
			GetComponent<Rigidbody>().isKinematic = false;
		} 
	}
}
