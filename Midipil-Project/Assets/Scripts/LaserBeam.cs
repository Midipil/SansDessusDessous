﻿using UnityEngine;
using System.Collections;

public class LaserBeam : MonoBehaviour
{
    LineRenderer line;

    public float rayLength = 100;
    public float minRadiusRayCast = 0;
    public float laserDamages = 1;
    public GameObject impactFX;
    public static int timeToKill = 3000;
    public static int currentTimeHit = timeToKill;
    private static System.Timers.Timer timerToKill;


    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();
    }

    public static void TimerKill()
    {
        timerToKill = new System.Timers.Timer(timeToKill);
        timerToKill.Enabled = true;
        timerToKill.AutoReset = true;
        timerToKill.Start();
        timerToKill.Elapsed +=
            (object sender, System.Timers.ElapsedEventArgs e) => currentTimeHit -= 1000;
    }


    void Update()
    {

        if (currentTimeHit <= 0)
			gameObject.transform.Find("GameManager").GetComponent<GameManager>().enemyWin = true;

        Ray ray = new Ray(transform.position + minRadiusRayCast * transform.forward, transform.forward);
        RaycastHit hit;

        line.SetPosition(0, transform.position);

        if (Physics.Raycast(ray, out hit, rayLength))
        {
            line.SetPosition(1, hit.point);
            if (hit.transform.tag == "Player")
            {
                //hit.transform.GetComponent<Player>().life -= laserDamages;
                TimerKill();
                impactFX.transform.position = hit.point;
                impactFX.SetActive(true);
            }
            else
            {
                timerToKill.Stop();
            }
        }
        else
        {
            line.SetPosition(1, ray.GetPoint(rayLength));
            impactFX.SetActive(false);
        }
    }
}