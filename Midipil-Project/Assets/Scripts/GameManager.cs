using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public GameObject winScreen;
	public GameObject gameoverScreen;

	[HideInInspector]
	public bool playerWin = false;
	[HideInInspector]
	public bool enemyWin = false;

	private bool gameFinished = false;

	private bool iAmReadyToRestart = false;
	private bool otherReadyToRestart = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// if i am the player
		if(playerWin && !enemyWin){
			showWinScreen();
			gameFinished = true;
		} else if(!playerWin && enemyWin){
			showGameOverScreen();
			gameFinished = true;
		}

		if(gameFinished && iAmReadyToRestart && otherReadyToRestart){
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
}
