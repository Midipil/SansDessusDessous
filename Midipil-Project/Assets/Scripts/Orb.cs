using UnityEngine;
using System.Collections;

public class Orb : MonoBehaviour {

	private bool triggered = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setTriggered(){
		triggered = true;
	}

	public bool isTriggered(){
		return triggered;
	}
}
