using UnityEngine;
using System.Collections;
using System;

public class AiPlayer : Player {

    public AiAlgorithm Algorithm;

    public override void ProcessTurn()
    {
        StartCoroutine("Process");
    }

    private IEnumerator Process()
    {
        //właczamy algorytm
        Move selected = Algorithm.Run(GameModel.GetInstance().Board, GameModel.GetInstance().BoardSize, GameController.GetInstance().CurrentPlayerIdx);

        //wlaściwy ruch
        while(GameModel.GetInstance().MoveChecker(selected))
        {
            selected = selected.Children[0];
        }

        //następna tura
        GameController.GetInstance().NextTurn();

        yield return null;
    }

    // Use this for initialization
    void Start () {

        Algorithm = new MinMax();

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
