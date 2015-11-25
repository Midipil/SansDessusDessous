using UnityEngine;
using UnityEngine.VR;
using System.Collections;

public class PlayerController : MonoBehaviour {

    private float boostFactor = 3.0f;
	private float moveSpeed = 1.5f;
    private float rotationSpeed = 5.0f;
	private float sidewaysSpeed = 1.0f;

	private float boostDuration = 1;
	private float boost = 3.0f;
	private float lerpTime = 0;

	// Update is called once per frame
	void FixedUpdate () {

		float currentBoost = 1;
		if(boost != 1.0f){
			currentBoost = Mathf.Lerp(boost, 1.0f, lerpTime);
			lerpTime += Time.deltaTime / boostDuration;
		}

		// Left/right
		//Debug.Log(currentBoost);
		this.GetComponent<Rigidbody>().AddTorque( transform.up * rotationSpeed * Input.GetAxis("Player_Horizontal"));
		this.GetComponent<Rigidbody>().AddTorque( transform.right * currentBoost * moveSpeed * Input.GetAxis("Player_Vertical"));
		this.GetComponent<Rigidbody>().AddTorque(- transform.forward * sidewaysSpeed * Input.GetAxis("Player_Right_Horizontal"));

        // LEFT STICK : MOVE FORWARD / BACKWARD & ROTATE 		
		//transform.Rotate(new Vector3(currentBoost * moveSpeed * Input.GetAxis("Player_Vertical"), rotationSpeed * Input.GetAxis("Player_Horizontal"), 0));

        // RIGHT STICK : DRIFT LEFT / RIGHT
        //transform.Rotate(new Vector3( 0f, 0f, Input.GetAxis("Player_Right_Horizontal")));

        // BOOSTER
        //Input.GetButton("")
        if (Input.GetButton("CalibrationRotation"))
        {
            // RECALIBRATE
			Debug.Log("Recalibrate");
			UnityEngine.VR.InputTracking.Recenter();
        }
        
    }

	public void Boost(){
		boost = boostFactor;
		lerpTime = 0;
	}
}
