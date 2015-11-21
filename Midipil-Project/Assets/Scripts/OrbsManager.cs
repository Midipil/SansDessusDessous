using UnityEngine;
using System.Collections;

public class OrbsManager : MonoBehaviour {

	private int orbsNum = 20; // 20 MAX
	private int orbsQuota = 10;
	private float radius = 45f;
	public GameObject orbPrefab;

	// Use this for initialization
	void Start () {
		for (int i=0; i<orbsNum ; i++){
			Instantiate(orbPrefab, randomSpherePoint(Vector3.zero, radius), Quaternion.identity);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	Vector3 randomSpherePoint(Vector3 p, float radius){
		float u = Random.value;
		float v = Random.value;
		float theta = 2 * Mathf.PI * u;
		float phi = Mathf.Acos(2 * v - 1);
		float x = p.x + (radius * Mathf.Sin(phi) * Mathf.Cos(theta));
		float y = p.y + (radius * Mathf.Sin(phi) * Mathf.Sin(theta));
		float z = p.z + (radius * Mathf.Cos(phi));
		return new Vector3(x,y,z);
	}
}
