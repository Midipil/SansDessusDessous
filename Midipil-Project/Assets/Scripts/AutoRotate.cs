using UnityEngine;
using System.Collections;

public class AutoRotate : MonoBehaviour {

	public float speed;
	
	// Update is called once per frame
	void Update () {
		//transform.Rotate(Vector3.left * speed * Time.deltaTime);
		transform.Rotate(new Vector3(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), 0));
	}
}
