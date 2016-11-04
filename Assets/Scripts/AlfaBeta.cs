using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class AlfaBeta : AiAlgorithm
{
    private int MaxDepth = 4;
    public override Move Run(CheckerData[,] board, int boardSize, int playerIdx)
    {
        bool[,] copy = GameModel.CopyIsKingProperty(board);

        //dla wyszystkich możliwych aktualnie ruchów wykonujemy RunMinMax(), szukamy najlepszego
        List<Move> possibleMoves = GameModel.GetPossibleMoves(board, boardSize, GameController.GetInstance().Players[playerIdx]);

        float currentMaxValue = float.MinValue;
        Move currentSelectedMove = null;
        foreach (Move move in possibleMoves)
        {
            float value = RunAlfaBeta(move, board, boardSize, playerIdx, 0, true,float.MinValue,float.MaxValue);
            if (value >= currentMaxValue)
            {
                currentMaxValue = value;
                currentSelectedMove = move;
            }
        }

        //przywracamy właściwości pionków
        GameModel.RestoreOrginalData(board, copy);
        return currentSelectedMove;
    }

    private float RunAlfaBeta(Move move, CheckerData[,] board, int boardSize, int playerIdx, int depth, bool min, float alfa, float beta) //min or max
    {
        float grade = (min ? float.MaxValue : float.MinValue);
        //zapisz / skopiuj właściwości planszy
        bool[,] propertyCopy = GameModel.CopyIsKingProperty(board);
        CheckerData[] dataCopy = GameModel.CopyLine(board, move.From, move.To);

        //wykonaj ruch
        GameModel.MoveSimulation(move.From, move.To, board);

        //jeżeli ruch zawiera "dzieci"
        if (move.Children != null && move.Children.Count > 0)
        {
            //wykonaj dla wszystkich dzieci
            Move selected = null;

            if(min)
            {
                foreach(Move child in move.Children)
                {
                    beta = Math.Min(beta, RunAlfaBeta(child, board, boardSize, playerIdx, depth, min, alfa, beta));
                    grade = beta;
                    if (alfa>=beta)
                    {
                        selected = child;
                        GameModel.RestoreLine(board, dataCopy, move.From, move.To);
                        GameModel.RestoreOrginalData(board, propertyCopy);
                        return grade;
                    }
                }
            }
            else
            {
                foreach(Move child in move.Children)
                {
                    alfa = Math.Max(alfa, RunAlfaBeta(child, board, boardSize, playerIdx, depth, min, alfa, beta));
                    selected = child;
                    grade = alfa;
                    if (alfa>=beta)
                    {
                        //usuń wszystkie dzieci i dopisz najlepsze
                        move.Children.Clear();
                        move.Children.Add(selected);
                        GameModel.RestoreLine(board, dataCopy, move.From, move.To);
                        GameModel.RestoreOrginalData(board, propertyCopy);
                        return grade;
                    }
                }
            }

            if(!min)
            {
                //usuń wszystkie dzieci i dopisz najlepsze
                move.Children.Clear();
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
            if (depth < MaxDepth)//jeżeli depth < MaxDepth
            {
                //znadjź wszystkie możliwe ruchy dla przeciwnika aktualnego gracza
                List<Move> possibleEnemyMoves = GameModel.GetPossibleMoves(board, boardSize, GameController.GetInstance().Players[1 - playerIdx]);

                if(min)
                {
                    foreach(Move enemyMove in possibleEnemyMoves)
                    {
                        beta = Math.Min(beta, RunAlfaBeta(enemyMove, board, boardSize, 1 - playerIdx, depth + 1, !min, alfa, beta));
                        grade = beta;
                        if(alfa>=beta)
                        {
                            GameModel.RestoreLine(board, dataCopy, move.From, move.To);
                            GameModel.RestoreOrginalData(board, propertyCopy);
                            //zwracamy ocenę ruchu
                            return grade;
                        }
                    }
                }
                else
                {
                    foreach(Move enemeMove in possibleEnemyMoves)
                    {
                        alfa = Math.Max(alfa, RunAlfaBeta(enemeMove, board, boardSize, 1 - playerIdx, depth + 1, !min, alfa, beta));
                        grade = alfa;
                        if(alfa>=beta)
                        {
                            GameModel.RestoreLine(board, dataCopy, move.From, move.To);
                            GameModel.RestoreOrginalData(board, propertyCopy);
                            //zwracamy ocenę ruchu
                            return grade;
                        }
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
                return Assets.Scripts.Heuristics.SimpleGrade(board, boardSize, GameController.GetInstance().Players[playerIdx]);
            }
        }

        //odwtórz wlaściwości planszy
        GameModel.RestoreLine(board, dataCopy, move.From, move.To);
        GameModel.RestoreOrginalData(board, propertyCopy);
        //zwracamy ocenę ruchu
        return grade;
    }
}
