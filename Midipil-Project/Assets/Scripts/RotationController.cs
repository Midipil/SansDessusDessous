﻿using UnityEngine;
using System.Collections;

public class RotationController : MonoBehaviour {

    private float boost = 1.0f;
	private float moveSpeed = 1.0f;
    private float rotationSpeed = 3.0f;

	// Update is called once per frame
	void Update () {

        // LEFT STICK : MOVE FORWARD / BACKWARD & ROTATE 		
		transform.Rotate(new Vector3(boost * moveSpeed * Input.GetAxis("Player_Vertical"), rotationSpeed * Input.GetAxis("Player_Horizontal"), 0));

        // RIGHT STICK : DRIFT LEFT / RIGHT
        transform.Rotate(new Vector3( 0f, 0f, Input.GetAxis("Player_Right_Horizontal")));

        // BOOSTER
        //Input.GetButton("")

    }
}
