using UnityEngine;
using UnityEngine.VR;
using System.Collections;

public class RotationController : MonoBehaviour {

    private float boost = 1.0f;
	private float moveSpeed = 1.0f;
    private float rotationSpeed = 3.0f;

	// Update is called once per frame
	void Update () {

        // LEFT STICK : MOVE FORWARD / BACKWARD & ROTATE 		
        transform.Rotate(new Vector3(boost * moveSpeed * Input.GetAxis("Vertical"), rotationSpeed * Input.GetAxis("Horizontal"), 0f));

        // RIGHT STICK : DRIFT LEFT / RIGHT
        transform.Rotate(new Vector3(0f, 0f, Input.GetAxis("Right_Horizontal")));

        // BOOSTER
        //Input.GetButton("")
        if (Input.GetButton("CalibrationRotation"))
        {
            // RECALIBRATE
           // Camera camera = this.GetComponentInChildren<Camera>();
            //Vector3 rotation = camera.transform.localRotation.eulerAngles;
            // Debug.Log(rotation);
            // rotation
            //camera.transform.localRotation = Quaternion.identity;

            //this.transform.localRotation = Quaternion.Euler( new Vector3(this.transform.localRotation.eulerAngles.x, rotation.y, this.transform.localRotation.eulerAngles.z));
        }
        
    }
}
