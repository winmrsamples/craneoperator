using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;

// This is used to detect if the bollard is intersecting any others whilst instantiating.
// If it is it will destroy itself.
public class BollardScript : MonoBehaviour {
    public bool collided = false;
    public bool hit = false;
    public Material glowMaterial;
    public float[] Id = new float[2]; // Used to identify bollards between devices

    private GameManager gameManager;
    private Renderer meshRenderer;

    void Start() {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        meshRenderer = GetComponent<Renderer>();
        StartCoroutine(CheckForCollision());
    }

    void Update() {
        if (!hit && HolographicSettings.IsDisplayOpaque){
            if (transform.eulerAngles.x > 90 || transform.eulerAngles.z > 90 || transform.eulerAngles.x < -90 || transform.eulerAngles.z < -90) {
                hit = true;
                gameManager.CmdRegisterHitBollard(Id);
            }
        }
    }
    
    public void SetToHit(){
        meshRenderer.material = glowMaterial;
        StartCoroutine(DestroySelfAfterSeconds(4f));
    }

    private IEnumerator DestroySelfAfterSeconds(float Seconds) {
        yield return new WaitForSeconds(Seconds);
        gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider collidingObject) {
        if (collidingObject.gameObject.tag == "Bollard"){
        collided = true;
        }
        // if ((collidingObject.gameObject.tag == "Weight" || collidingObject.gameObject.tag == "Hook" || collidingObject.gameObject.tag == "Bollard" || collidingObject.gameObject.tag == "HookMeshes") && GetComponent<Rigidbody>().isKinematic) {
        //     GetComponent<CapsuleCollider>().isTrigger = false;
        //     GetComponent<Rigidbody>().isKinematic = false;
        // }
    }

    IEnumerator CheckForCollision() {
        yield return null;
        if (collided) {
            Destroy(this.gameObject);
        } else if (HolographicSettings.IsDisplayOpaque) {
            GetComponent<CapsuleCollider>().radius = 0.5f;
            GetComponent<CapsuleCollider>().isTrigger = false;
        }
    }
}
