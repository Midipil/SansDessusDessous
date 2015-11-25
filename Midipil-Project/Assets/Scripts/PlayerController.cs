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

	private AudioSource shipSound;

	void Start(){
		GameObject ab = GameObject.Find("Afterburner").gameObject;
		shipSound = ab.GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void FixedUpdate () {

		// Update sound volume
		float maxMag = 0.5f;
		float vol = this.GetComponent<Rigidbody>().angularVelocity.magnitude / maxMag;
		if (vol > 1.0f) vol = 1.0f;
		shipSound.volume = vol;

		float currentBoost = 1;
		if(boost != 1.0f){
			currentBoost = Mathf.Lerp(boost, 1.0f, lerpTime);
			lerpTime += Time.deltaTime / boostDuration;
			shipSound.pitch = currentBoost/5+0.8f;
		}
		if (currentBoost <= 1.0f){
			shipSound.pitch = vol;
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
