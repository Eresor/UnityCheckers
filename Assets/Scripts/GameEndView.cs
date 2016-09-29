using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameEndView : MonoBehaviour {

    public Text GameEndText;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetWinnerText(int winner)
    {
        GameEndText.text = "Game ended, Player " + winner + " won.";
    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
