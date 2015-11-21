using UnityEngine;
using System.Collections;

public class LaserBeam : MonoBehaviour 
{
	LineRenderer line;

	public float rayLength = 100;
	public float minRadiusRayCast = 0;
	public float laserDamages = 1;
	
	void Start (){
		line = gameObject.GetComponent<LineRenderer>();
	}

	void Update (){

		Ray ray = new Ray(transform.position + minRadiusRayCast * transform.forward, transform.forward);
		RaycastHit hit;
		
		line.SetPosition(0, transform.position);
		
		if(Physics.Raycast(ray, out hit, rayLength))
		{
			line.SetPosition(1, hit.point);
			if(hit.transform.tag == "Player")
			{
				hit.transform.GetComponent<Player>().life -= laserDamages;
			}
		}
		else
			line.SetPosition(1, ray.GetPoint(rayLength));
		
	}
}