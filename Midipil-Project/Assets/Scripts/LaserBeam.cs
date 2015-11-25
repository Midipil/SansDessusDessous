using UnityEngine;
using System.Collections;

public class LaserBeam : MonoBehaviour
{
    LineRenderer line;

    public float rayLength = 100;
    public float minRadiusRayCast = 0;
    public float laserDamages = 1;
    public GameObject impactFX;
    public int timeToKill = 3;
    public float currentTimeHit;


    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();
		currentTimeHit = timeToKill;
    }

    void Update()
    {

        if (currentTimeHit <= 0){
			GameObject.FindWithTag("GameManager").GetComponent<GameManager>().enemyWin = true;
			Debug.Log("Enemy win");
		}

        Ray ray = new Ray(transform.position + minRadiusRayCast * transform.forward, transform.forward);
        RaycastHit hit;

        line.SetPosition(0, transform.position);

        if (Physics.Raycast(ray, out hit, rayLength))
        {
            line.SetPosition(1, hit.point);
            if (hit.transform.parent.tag == "Player")
            {
                //hit.transform.GetComponent<Player>().life -= laserDamages;
                //TimerKill();
				currentTimeHit -= Time.deltaTime;
				AudioSource source;
				source = hit.transform.gameObject.GetComponent<AudioSource>();
				if (!source.isPlaying){
					hit.transform.gameObject.GetComponent<AudioSource>().Play();
				}

                impactFX.transform.position = hit.point;
                impactFX.SetActive(true);
            }
        }
        else
        {
            line.SetPosition(1, ray.GetPoint(rayLength));
            impactFX.SetActive(false);
			currentTimeHit = timeToKill;
        }
    }
}