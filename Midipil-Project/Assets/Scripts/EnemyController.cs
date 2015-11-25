using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

	float speed = 1.5f; 

	// Update is called once per frame
	void FixedUpdate() {

		// Old method
		//transform.Rotate(new Vector3(Input.GetAxis("Enemy_Vertical"), Input.GetAxis("Enemy_Horizontal"), 0));

		this.GetComponent<Rigidbody>().AddTorque(transform.up * Input.GetAxis("Enemy_Horizontal") * speed);
		this.GetComponent<Rigidbody>().AddTorque(- transform.right * Input.GetAxis("Enemy_Vertical") * speed);
	}
}
