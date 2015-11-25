using UnityEngine;
using UnityEngine.VR;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public float boostFactor = 3.0f;
	public float moveSpeed = 1.0f;
    public float rotationSpeed = 3.0f;

	public float boostDuration = 4;
	private float boost = 1.0f;
	private float lerpTime = 0;

	// Update is called once per frame
	void Update () {

		float currentBoost = 1;
		if(boost != 1.0f){
			currentBoost = Mathf.Lerp(boost, 1.0f, lerpTime);
			lerpTime += Time.deltaTime / boostDuration;
		}

        // LEFT STICK : MOVE FORWARD / BACKWARD & ROTATE 		
		transform.Rotate(new Vector3(currentBoost * moveSpeed * Input.GetAxis("Player_Vertical"), rotationSpeed * Input.GetAxis("Player_Horizontal"), 0));

        // RIGHT STICK : DRIFT LEFT / RIGHT
        transform.Rotate(new Vector3( 0f, 0f, Input.GetAxis("Player_Right_Horizontal")));

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
