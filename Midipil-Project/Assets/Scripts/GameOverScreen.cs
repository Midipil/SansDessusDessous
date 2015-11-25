using UnityEngine;
using System.Collections;

public class GameOverScreen : MonoBehaviour {

	public GameObject winScreen;
	public GameObject gameoverScreen;

	public void showWinScreen(){
		winScreen.SetActive(true);
	}
	
	public void showGameOverScreen(){
		gameoverScreen.SetActive(true);
	}
}
