using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.WSA;
using UnityEngine.Networking;

public class CraneMovementScript : NetworkBehaviour {
    public GameObject orbitCenter; // GameObject as origin;
    public GameObject trolley; // GameObject as origin;
    public GameObject rope;
    public GameObject hololensRope;
    public GameObject hook;
    public Vector3 rotationDirection;  
    public float smoothTime;
	public GameObject leftJoystick;
	public GameObject rightJoystick;
	public GameObject HMDUCockpitObjects;
	public bool gamePaused = false;
	public float ropeSpeed = 1f;
	public float trolleySpeed = 1f;
	public float rotationSpeed = 1f;

    private Vector3 orbitPosition;
	private Vector3 initRopePosition;
	private Vector3 ropeAnchor;
    private float convertedTime = 200;
    private float smooth;
    private float ropeLength = 1.9f;

	void Update () {
		// Calculate the in game (crane) joysticks. 
		// We must only do this for the HMDU user for now as the rope and hook are instantiated prefabs and will thrown null ref errors
		// on the hololens.
		if (HolographicSettings.IsDisplayOpaque && !gamePaused) {
			float inputX = leftJoystick.transform.eulerAngles.x;
			float inputZ = leftJoystick.transform.eulerAngles.z;
			float rightInputX = rightJoystick.transform.eulerAngles.x;
			

			
			if (inputX > 180) {
				inputX -= 360;	
			}
			if (inputZ > 180) {
				inputZ -= 360;
			}
			if (rightInputX > 180) {
				rightInputX -= 360;
			}

			float inputPercentageX = inputX / 30;
			float inputPercentageZ = inputZ / 30;
			float rightInputPercentageX = rightInputX / 30;

			if (inputPercentageX < 0.1 && inputPercentageX > -0.1){
				inputPercentageX = 0;
			}
			if (rightInputPercentageX < 0.1 && rightInputPercentageX > -0.1){
				rightInputPercentageX = 0;
			}


			//controlling crane movement
			orbitPosition = orbitCenter.transform.position;
			smooth = Time.deltaTime * rotationSpeed * smoothTime * convertedTime;

			if (Input.GetAxis("CONTROLLER_LEFT_STICK_HORIZONTAL") != 0 || Input.GetAxis("Horizontal") != 0)
            {
				transform.RotateAround(orbitPosition, rotationDirection * 0.005f * (Input.GetAxis("Horizontal") + Input.GetAxis("CONTROLLER_LEFT_STICK_HORIZONTAL")), smooth); 
				HMDUCockpitObjects.transform.RotateAround(orbitPosition, rotationDirection * 0.005f * (Input.GetAxis("Horizontal") + Input.GetAxis("CONTROLLER_LEFT_STICK_HORIZONTAL")), smooth);
            }
#if DEBUG
            //Lazy way to use it all with one controller 
            else if (Input.GetAxis("CONTROLLER_RIGHT_TOUCHPAD_X") != 0)
            {
                transform.RotateAround(orbitPosition,  rotationDirection * 0.005f * (Input.GetAxis("CONTROLLER_RIGHT_TOUCHPAD_X")), smooth);
                HMDUCockpitObjects.transform.RotateAround(orbitPosition,  rotationDirection * 0.005f * (Input.GetAxis("CONTROLLER_RIGHT_TOUCHPAD_X")), smooth);
            }
#endif

            else if (inputPercentageZ  != 0) {
				transform.RotateAround(orbitPosition,  rotationDirection * 0.005f * inputPercentageZ * -1, smooth * Mathf.Abs(inputPercentageZ)); 
				HMDUCockpitObjects.transform.RotateAround(orbitPosition,  rotationDirection * 0.005f * inputPercentageZ * -1, smooth * Mathf.Abs(inputPercentageZ)); 
			}

			if (Input.GetAxis("CONTROLLER_LEFT_STICK_VERTICAL") != 0 || Input.GetAxis("Vertical") != 0) {
				trolley.transform.position += trolley.transform.forward * trolleySpeed * 0.01f * (Input.GetAxis("Vertical") + Input.GetAxis("CONTROLLER_LEFT_STICK_VERTICAL")); 
			}
#if DEBUG
            //Lazy way to use it all with one controller 
            else if (Input.GetAxis("CONTROLLER_RIGHT_TOUCHPAD_Y") != 0)
            {
                trolley.transform.position += trolley.transform.forward * trolleySpeed * 0.01f * (Input.GetAxis("CONTROLLER_RIGHT_TOUCHPAD_Y"));
            }
#endif
            else if (inputPercentageX  != 0) {
				trolley.transform.position += trolley.transform.forward * trolleySpeed * 0.01f * inputPercentageX; 
			}



            if ((Input.GetAxis("CONTROLLER_RIGHT_STICK_VERTICAL") < 0 && rope.transform.localScale.y > 0.5f) || (Input.GetAxis("CONTROLLER_RIGHT_STICK_VERTICAL") > 0 && rope.transform.localScale.y < 5f)){
				rope.transform.localScale += new Vector3(0f, 0.15f * Time.deltaTime * Input.GetAxis("CONTROLLER_RIGHT_STICK_VERTICAL"), 0f);
			}
			
			if ((rope.transform.localScale.y > 0.5f && rightInputPercentageX < 0.001) || (rope.transform.localScale.y < 5f && rightInputPercentageX > 0.001)) {
				 if (rightInputPercentageX  != 0 && rope.transform.localScale.y > 0.5f || rope.transform.localScale.y < 5f && rightInputPercentageX > 0) {
					rope.transform.localScale += new Vector3(0f, 0.3f * ropeSpeed * Time.deltaTime * rightInputPercentageX, 0f);

					float differenceInScale = ropeLength - rope.transform.localScale.y;
					if (differenceInScale > 0.1f || differenceInScale < -0.1f) {
						ropeLength = rope.transform.localScale.y;
						RpcUpdateRopeLength(rope.transform.localScale);
					}
				}
			}

			//For testing with keyboard
			if (Input.GetKey("o") && rope.transform.localScale.y < 5f){
				Vector3 scaleUp = new Vector3(0f, 0.3f * ropeSpeed * Time.deltaTime * 1, 0f);
				rope.transform.localScale += scaleUp;
				
				float differenceInScale = ropeLength - rope.transform.localScale.y;
				if (differenceInScale > 0.1f || differenceInScale < -0.1f) {
					ropeLength = rope.transform.localScale.y;
					RpcUpdateRopeLength(rope.transform.localScale);
				}
				
			} else if (Input.GetKey("p") && rope.transform.localScale.y > 0.5f) {
				rope.transform.localScale -= new Vector3(0f, 0.3f * ropeSpeed * Time.deltaTime * 1, 0f);

				float differenceInScale = ropeLength - rope.transform.localScale.y;
				if (differenceInScale > 0.1f || differenceInScale < -0.1f) {
					ropeLength = rope.transform.localScale.y;
					RpcUpdateRopeLength(rope.transform.localScale);
				}
			}
		}
	}

	//Update rope length on hololens
	[ClientRpc]
	void RpcUpdateRopeLength(Vector3 scaleAmount){
		if (!HolographicSettings.IsDisplayOpaque) {
			rope.transform.localScale = new Vector3(rope.transform.localScale.x, scaleAmount.y, rope.transform.localScale.z);
		}
	}
}


