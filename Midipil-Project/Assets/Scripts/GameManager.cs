using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	[HideInInspector]
	public bool playerWin = false;
	[HideInInspector]
	public bool enemyWin = false;

	private bool gameFinished = false;

	private bool iAmReadyToRestart = false;
	private bool otherReadyToRestart = false;

	private float restartTimer = 0;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		// if i am the player
		if(IsPlayer() && playerWin && !enemyWin){
			GameObject.FindWithTag("Player").GetComponent<GameOverScreen>().showWinScreen();
			gameFinished = true;
		} else if(IsPlayer() && !playerWin && enemyWin){
			GameObject.FindWithTag("Player").GetComponent<GameOverScreen>().showGameOverScreen();
			gameFinished = true;
		} else if(IsEnemy() && !playerWin && enemyWin){
			GameObject.FindWithTag("Enemy").GetComponent<GameOverScreen>().showWinScreen();
			gameFinished = true;
		} else if(IsEnemy() && playerWin && !enemyWin){
			GameObject.FindWithTag("Enemy").GetComponent<GameOverScreen>().showGameOverScreen();
			gameFinished = true;
		}

		if(gameFinished){
			restartTimer += Time.deltaTime;
		}
		if(restartTimer > 5){
			restart();
		}

	}

	void restart(){
		Application.LoadLevel("Main");
	}

	public bool IsPlayer(){
		return Network.isServer;
	}

	public bool IsEnemy(){
		return Network.isClient;
	}
}
