using UnityEngine;
using System.Collections.Generic;
using System;

public class MinMax : AiAlgorithm {

    Player m_player;

    private int MaxDepth = 4;

    public override Move Run(CheckerData[,] board, int boardSize, int playerIdx)
    {
        m_player = GameController.GetInstance().Players[playerIdx];
        //kopiujemy właściwości pionków
        bool[,] copy = GameModel.CopyIsKingProperty(board);

        //dla wyszystkich możliwych aktualnie ruchów wykonujemy RunMinMax(), szukamy najlepszego
        List<Move> possibleMoves = GameModel.GetPossibleMoves(board, boardSize, m_player);

        float currentMaxValue = float.MinValue;
        Move currentSelectedMove = null;
        foreach(Move move in possibleMoves)
        {
            float value = RunMinMax(move, board, boardSize, playerIdx, 0, true);
            if(value>=currentMaxValue)
            {
                currentMaxValue = value;
                currentSelectedMove = move;
            }
        }

        //przywracamy właściwości pionków
        GameModel.RestoreOrginalData(board, copy);

        return currentSelectedMove;
    }

    private float RunMinMax(Move move, CheckerData[,] board, int boardSize, int playerIdx, int depth, bool min) //min or max
    {
        float grade = (min ? float.MaxValue : float.MinValue);
        //zapisz / skopiuj właściwości planszy
        bool[,] propertyCopy = GameModel.CopyIsKingProperty(board);
        CheckerData[] dataCopy = GameModel.CopyLine(board, move.From, move.To);

        //wykonaj ruch
        GameModel.MoveSimulation(move.From, move.To, board);
        
        //jeżeli ruch zawiera "dzieci"
        if(move.Children!=null && move.Children.Count>0)
        {
            //wykonaj dla wszystkich dzieci
            Move selected = null;
            foreach(Move child in move.Children)
            {
                float ret = RunMinMax(child, board, boardSize, playerIdx, depth, min);
                //znajdź największą / najmniejszą
                if(min && ret<=grade)
                {
                    grade = ret;
                }

                if(!min && ret>=grade)
                {
                    grade = ret;
                    //zapisz najlepsze dziecko
                    selected = child;
                }
            }

            //usuń wszystkie dzieci i dopisz najlepsze
            if(!min)
            {
                move.Children.Clear();
                Debug.Assert(selected != null);
                move.Children.Add(selected);
            }

            //odwtórz wlaściwości planszy
            GameModel.RestoreLine(board, dataCopy, move.From, move.To);
            GameModel.RestoreOrginalData(board, propertyCopy);
            // zwróc ocenę ruchu
            return grade;
        }
        else //w przeciwnym wypadku
        {
            if(depth < MaxDepth)//jeżeli depth < MaxDepth
            {
                //znadjź wszystkie możliwe ruchy dla przeciwnika aktualnego gracza
                List<Move> possibleEnemyMoves = GameModel.GetPossibleMoves(board, boardSize, GameController.GetInstance().Players[1 - playerIdx]);
                
                //dla każdego ruchu
                foreach (Move enemyMove in possibleEnemyMoves)
                {
                    //RunMinMax(depth+1,!min)
                    float val = RunMinMax(enemyMove, board, boardSize, 1 - playerIdx, depth + 1, !min);
                    //znajdź najlepszy / najgorszy
                    if( ( min && val<=grade) || ( !min && val >= grade ) )
                    {
                        grade = val;
                    }
                }
            }
            //w przeciwnym wypadku
            else
            {
                //odwtorz wl. planszy
                GameModel.RestoreLine(board, dataCopy, move.From, move.To);
                GameModel.RestoreOrginalData(board, propertyCopy);
                //zwroc ocene ruchu
                return UnityEngine.Random.Range(0, 100);
            }
        }

        //odwtórz wlaściwości planszy
        GameModel.RestoreLine(board, dataCopy, move.From, move.To);
        GameModel.RestoreOrginalData(board, propertyCopy);
        //zwracamy ocenę ruchu
        return grade;
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
