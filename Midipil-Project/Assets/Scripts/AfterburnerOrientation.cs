using UnityEngine;
using System.Collections;

public class AfterburnerOrientation : MonoBehaviour {

	public GameObject afterburner;
	public float factor = 32;
	
	private Vector3 previousPosition = new Vector3(0,0,0);

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		afterburner.GetComponent<ParticleSystem>().startSpeed = (transform.position - previousPosition).magnitude * factor;
		//afterburner.transform.forward = - transform.position + previousPosition;
		previousPosition = transform.position;
	}
}
