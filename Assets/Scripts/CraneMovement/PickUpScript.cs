using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PickUpScript : MonoBehaviour {
	public GameObject parentPickUp;
	public ConfigurableJoint ropeJoint;
	public Slider slider;
	public bool isHooked = false;

	private float timer = 0;
	private float pickUpTimer = 5;
	private float releaseTimer = 3;
	private bool hasJustBeenHooked = false;
	private bool hasJustBeenReleased = false;
	private Vector3 previousPosition;
	private Vector3 positionDifference;
	private JointDrive connectedDrive;
	private JointDrive disconnectedDrive;
	private SoftJointLimit connectedLimit;

	void Start(){
		previousPosition = transform.position;
		if (!HolographicSettings.IsDisplayOpaque){
			// parentPickUp.GetComponent<Rigidbody>().isKinematic = true;
		}

		connectedDrive = new JointDrive();
        connectedDrive.positionSpring = 35f;
        connectedDrive.positionDamper = 1000f;
        connectedDrive.maximumForce = 3.40282300000000000000000000f;
		
		disconnectedDrive = new JointDrive();
        disconnectedDrive.positionSpring = 35f;
        disconnectedDrive.positionDamper = 35f;
        disconnectedDrive.maximumForce = 3.40282300000000000000000000f;
	}

	void Update()
	{
		if (HolographicSettings.IsDisplayOpaque){
			if (hasJustBeenHooked && pickUpTimer > 0) {
				pickUpTimer -= Time.deltaTime;
			} else if (hasJustBeenReleased && releaseTimer > 0) {
				releaseTimer -= Time.deltaTime;
				return;
			} else if (hasJustBeenReleased && releaseTimer < 0) {
				hasJustBeenReleased = false;
				releaseTimer = 3;
			} else if (hasJustBeenHooked && pickUpTimer <= 0){
				hasJustBeenHooked = false;
				pickUpTimer = 5;
			} else if (Input.GetAxis("PICK_UP_BUTTON") != 0 && isHooked && !hasJustBeenReleased){
				timer -= Time.deltaTime;
				slider.value = timer;
				if (timer <= 0){
					Destroy(parentPickUp.GetComponent<FixedJoint>());
					timer = 0;
					isHooked = false;
					hasJustBeenReleased = true;
					ropeJoint.angularXMotion = ConfigurableJointMotion.Free;
					ropeJoint.angularZMotion = ConfigurableJointMotion.Free;
				}
			} else if (isHooked && slider.value < 3) {
				slider.value += Time.deltaTime;
				timer = slider.value;
			}
		}
	}

	public void ResetWeight(){
		slider.value = 0;
		timer = 0;
		pickUpTimer = 5;
		isHooked = false;
		hasJustBeenReleased = true;
		ropeJoint.angularXMotion = ConfigurableJointMotion.Free;
		ropeJoint.angularZMotion = ConfigurableJointMotion.Free;
		Destroy(parentPickUp.GetComponent<FixedJoint>());
	}

	void OnTriggerStay(Collider hook){
		if (!isHooked) {
			if (hook.gameObject.tag == "Hook" && timer < 3f && Input.GetAxis("PICK_UP_BUTTON") != 0 && !hasJustBeenReleased){
				timer += Time.deltaTime;
				Destroy(parentPickUp.GetComponent<FixedJoint>());
				slider.value = timer;
			} else if (hook.gameObject.tag == "Hook" && timer >= 3f && Input.GetAxis("PICK_UP_BUTTON") != 0){
				isHooked = true;
				parentPickUp.AddComponent<FixedJoint>();
				parentPickUp.GetComponent<FixedJoint>().connectedBody = hook.GetComponent<Rigidbody>();
				slider.value = 3;
				timer = 3;
				hasJustBeenHooked = true;

				ropeJoint.angularXMotion = ConfigurableJointMotion.Limited;
				ropeJoint.angularZMotion = ConfigurableJointMotion.Limited;
				ropeJoint.angularXDrive = connectedDrive;
				ropeJoint.angularYZDrive = connectedDrive;
			}
		}
	}
}
