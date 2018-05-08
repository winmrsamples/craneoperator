using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;

public class WindManager : MonoBehaviour {

	public Vector3 direction;
	public float strength;

	Rigidbody rigidBody;
	
	void Start () {
		rigidBody = this.GetComponent<Rigidbody>();
		strength = 0;
		direction = new Vector3(0f, 0f, 0f);
	}
	
	void FixedUpdate () {
		if (strength > 0) {  // Wind on
			if (HolographicSettings.IsDisplayOpaque) {
				rigidBody.AddForce(direction * strength);
			} else {
				rigidBody.AddForce(direction * strength * 2000);
			}
		}
	}

	public void SetWindLevel(float strengthLevel, Vector3 directionVector){
		direction = directionVector;
		strength = strengthLevel;
	}
}
