using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	private GameObject winScreen;
	private GameObject gameoverScreen;

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
		if(IsEnemy()){
			winScreen = GameObject.FindWithTag("Enemy").transform.FindChild("Win Text").gameObject;
			gameoverScreen = GameObject.FindWithTag("Enemy").transform.FindChild("Lose Text").gameObject;
		} else if(IsPlayer()){
			winScreen = GameObject.FindWithTag("Player").transform.FindChild("Win Text").gameObject;
			gameoverScreen = GameObject.FindWithTag("Player").transform.FindChild("Lose Text").gameObject;
		}
	}
	
	// Update is called once per frame
	void Update () {
		// if i am the player
		if(IsPlayer() && playerWin && !enemyWin){
			showWinScreen();
			gameFinished = true;
		} else if(IsPlayer() && !playerWin && enemyWin){
			showGameOverScreen();
			gameFinished = true;
		} else if(IsEnemy() && !playerWin && enemyWin){
			showWinScreen();
			gameFinished = true;
		} else if(IsEnemy() && playerWin && !enemyWin){
			showGameOverScreen();
			gameFinished = true;
		}

		if(gameFinished){
			restartTimer += Time.deltaTime;
		}
		if(restartTimer > 5){
			restart();
		}

	}

	void showWinScreen(){
		winScreen.SetActive(true);
	}

	void showGameOverScreen(){
		gameoverScreen.SetActive(true);
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
