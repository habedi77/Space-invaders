using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{


	public GameObject Player;
	
	public Text Message;
	public Text SubMessage;
	public Text ScoreMessage;
	
	
	private bool _enable;
	// Use this for initialization


	private void Update()
	{
		if (_enable)
		{
			if (Input.GetButton("Fire1"))
			{
				//TODO go to menu
				SceneManager.LoadScene("MainMenu");
			}

			
		}
	}

	private void DisablePlayer()
	{
		if(Player)
			Destroy(Player);
	}
//
//
//	private void EnableUI()
//	{
//		
//		
//		
//	}
	
	
	
	public void UpdateScreenMessage(Gamestate gamestate, int score)
	{
		//TODO display won/lost message
//		GameObject player = GameObject.FindGameObjectWithTag("Player");
		switch (gamestate)
		{
			case Gamestate.InProgress:
				Message.text = "";
				SubMessage.text = "";
				break;
			case Gamestate.Won:
				Message.text = "YOU WON";
				SubMessage.text = "Tap to go back to Menu";
				_enable = true;
				DisablePlayer();
				break;
			case Gamestate.Lost:
				Message.text = "GAMEOVER";
				SubMessage.text = "Tap to go back to Menu";
				_enable = true;
				DisablePlayer();
				break;
				
		}
		ScoreMessage.text = "Score: " + score.ToString("D4");
	}
}
