using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public float life = 100;
	public UnityEngine.UI.Text lifeUI;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		lifeUI.text = "Life : " + life;
	}
}
