using UnityEngine;
using System.Collections;
using System;

public class AiPlayer : Player {

    public AiAlgorithm Algorithm;

    private bool coroutineStarted = false;

    public override void ProcessTurn()
    {
        if (!coroutineStarted)
        {
            StartCoroutine("Process");
        }
    }

    private IEnumerator Process()
    {
        coroutineStarted = true;
        //właczamy algorytm
        Move selected = Algorithm.Run(GameModel.GetInstance().Board, GameModel.GetInstance().BoardSize, GameController.GetInstance().CurrentPlayerIdx);

        while (GameModel.GetInstance().MoveChecker(selected))
        {
            while (!GameView.GetInstance().FinishedAnimation)
            {
                yield return null;
            }
            selected = selected.Children[0];
        }
        GameController.GetInstance().NextTurn();
        coroutineStarted = false;
        yield return null;
    }

    // Use this for initialization
    void Start () {

        Algorithm = new AlfaBeta();

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
