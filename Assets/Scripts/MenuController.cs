using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {


	public void loadGame()
	{
		SceneManager.LoadScene("Play");
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape)) 
			Application.Quit(); 
	}
}
