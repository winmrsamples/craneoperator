using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;
public class HololensOnlyScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (HolographicSettings.IsDisplayOpaque) {
			Destroy(gameObject);
		}
	}
}
