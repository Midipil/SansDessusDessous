using UnityEngine;
using System.Collections;

public class OrbsCollision : MonoBehaviour {

	private int orbsNum; // Set in orbsmanager
	private int orbsToDestroy = 3;
	private int orbsDestroyed = 0;
	
	// Use this for initialization
	void Start () {
		//orbsNum = GameObject.Find("Orbs").GetComponent<OrbsManager>().getOrbsNum();
        orbsNum = GameObject.Find("OrbsInstanciated2").transform.childCount;
        Debug.Log(orbsNum);
    }
	
	// Update is called once per frame
	void Update () {
	}
	
	void OnTriggerEnter(Collider other) {
		// Get orb component
		GameObject go = other.transform.gameObject;
		Orb o = go.GetComponent<Orb>();
		
		if (!o.isTriggered()){
			o.setTriggered();
			Destroy(other.gameObject);
			orbsDestroyed++;
			//Debug.Log("Orb destroyed ("+orbsDestroyed+"/"+orbsNum+" - "+orbsToDestroy+" mini to win");
			// Play sound
			if (orbsDestroyed >= orbsToDestroy){
				win();
			}
		}
	}
	
	void win(){
		Debug.Log ("WIIIIIIIIIIN");
	}
}
