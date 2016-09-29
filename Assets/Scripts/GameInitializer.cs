using UnityEngine;
using System.Collections;

public class GameInitializer : MonoBehaviour {

    // Use this for initialization

    public GameObject GameModel;

    public GameObject GameControllerObject;

    public GameObject PlayerPrefab;

    public GameSetupView GameSetupViewUI;

    void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void StartButton()
    {
        GameModel gameModel = Instantiate(GameModel).GetComponent<GameModel>();

        gameModel.BoardSize = (int)GameSetupViewUI.NumRowsSlider.value;

        gameModel.NumCheckersRows = (int)GameSetupViewUI.CheckersRowsSlider.value;

        gameModel.Init();

        GameObject p1 = Instantiate(PlayerPrefab);
        p1.name = GameController.Player1Name;

        GameObject p2 = Instantiate(PlayerPrefab);
        p2.name = GameController.Player2Name;


        Camera.main.GetComponent<GameView>().Init();

        Instantiate(GameControllerObject).GetComponent<GameController>().Init();

        Destroy(GameSetupViewUI.gameObject);
    }
}
