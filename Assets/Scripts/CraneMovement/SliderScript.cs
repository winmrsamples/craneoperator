using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderScript : MonoBehaviour {

	public Transform weightTransform;
	private Transform groundHeight;
	private Transform ball;
	// Update is called once per frame

	
	void Update () {
		//We really only need to update this if the hololens user moves the scene
		groundHeight = GameObject.FindGameObjectWithTag("GroundHeight").transform;
		ball = GameObject.FindGameObjectWithTag("Weight").transform;
		transform.position = new Vector3(weightTransform.position.x, (groundHeight.position.y - ball.lossyScale.y / 2) + 1f, weightTransform.position.z);
		transform.rotation = Quaternion.AngleAxis(90, Vector3.right);
	}
}
