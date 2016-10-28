using UnityEngine;
using System.Collections;

public abstract class AiAlgorithm {

    public abstract Move Run(CheckerData[,] board, int boardSize, int playerIdx);
}
