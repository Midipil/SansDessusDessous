using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

	// Update is called once per frame
	void Update () {

		transform.Rotate(new Vector3(Input.GetAxis("Enemy_Vertical"), Input.GetAxis("Enemy_Horizontal"), 0));
		
	}
}
