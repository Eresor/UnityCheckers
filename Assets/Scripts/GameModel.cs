using UnityEngine;
using System.Collections.Generic;

public class Vec2
{
    public Vec2()
    {

    }

    public Vec2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public int x;

    public int y;
}

public class Move
{
    public Vec2 From;

    public Vec2 To;

    public List<Move> Children;

    public Move(Vec2 from, Vec2 to)
    {
        From = from;
        To = to;
    }
}

public class GameModel : MonoBehaviour {

    public static GameModel GetInstance()
    {
        if(instance==null)
        {
            instance = GameObject.FindGameObjectWithTag("GameModel").GetComponent<GameModel>();
        }
        return instance;
    }

    private static GameModel instance = null;

    public int BoardSize = 10;

    public int NumCheckersRows = 2;

    public CheckerData[,] board;

    public CheckerData[,] Board
    {
        get
        {
            return board;
        }
    }

	// Use this for initialization
	void Start () {

        board = new CheckerData[BoardSize, BoardSize];
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void RegiesterCheckerData(CheckerData data, Vec2 position)
    {
        board[position.x, position.y] = data;
    }

    static int SimulateCombo(Vec2 position, CheckerData[,] board, int size, int counter, Move move)
    {
        int insideCounter = 1;
        List<Vec2> captured = GetCaptureList(position, board, size);
        if (captured.Count == 0)
        {
            move.Children = null;
            return 1;
        }
        else
        {
            foreach(Vec2 target in captured)
            {
                Move child = new Move(position, target);
                CheckerData[,] copy = board.Clone() as CheckerData[,];
                MoveSimulation(position, target, copy);
                int newComboCounter = SimulateCombo(target, copy, size, counter+1, child);
                if(newComboCounter>insideCounter)
                {
                    move.Children.Clear();
                    move.Children.Add(child);
                    insideCounter = newComboCounter;
                }
                else if(newComboCounter==insideCounter)
                {
                    move.Children.Add(child);
                }
            }
        }

        return insideCounter;
    }

    static List<Vec2> GetNonKillingMovesForField(Vec2 field, CheckerData[,] board, int size)
    {
        List<Vec2> ret = new List<Vec2>();

        CheckerData checker = board[field.x, field.y];

        if(checker.IsKing == false)
        {
            Vec2 target = new Vec2(field.x + 1, field.y + checker.Owner.Direction);
            if(target.x>=0 && target.x<size && target.y>=0 && target.y <size && board[target.x,target.y]==null)
            {
                ret.Add(target);
            }
            target = new Vec2(field.x -1, field.y + checker.Owner.Direction);
            if (target.x >= 0 && target.x < size && target.y >= 0 && target.y < size && board[target.x, target.y] == null)
            {
                ret.Add(target);
            }
        }
        else
        {
            throw new System.Exception("Not implemented");
        }

        return ret;
    }

    static List<Vec2> GetCaptureList(Vec2 field, CheckerData[,] board, int size)
    {
        List<Vec2> ret = new List<Vec2>();
        CheckerData checker = board[field.x, field.y];
        if(checker.IsKing==false)
        {
            for (int y = -1; y <= 1; y+=2)
            {
                for(int x=-1;x<=1;x+=2)
                {
                    Vec2 target = new Vec2(field.x + x, field.y + y);
                    Vec2 behind = new Vec2(field.x + 2*x, field.y + 2*y);
                    if (behind.x >= 0 && behind.x<size && behind.y>=0 && behind.y<size)
                    {
                        CheckerData targetChecker = board[target.x, target.y];
                        if(targetChecker!=null && targetChecker.Owner!=checker.Owner && board[behind.x,behind.y]==null)
                        {
                            ret.Add(behind);
                        }
                    }
                }
            }
        }


        return ret;
    }

    static void MoveSimulation(Vec2 from, Vec2 to, CheckerData[,] board)
    {
        Vec2 step = new Vec2(from.x - to.x, from.y - to.y);
        if(Mathf.Abs(step.y)>1)
        {
            if(board[from.x,from.y].IsKing==false)
            {
                CheckerData killed = board[from.x + (step.x) / Mathf.Abs(step.x), from.y + step.y / Mathf.Abs(step.y)];
                if(killed!=null)
                {
                    board[from.x + (step.x) / Mathf.Abs(step.x), from.y + step.y / Mathf.Abs(step.y)] = null;
                    board[to.x, to.y] = board[from.x, from.y];
                    board[from.x, from.y] = null;
                }
            }
            else
            {
                throw new System.Exception("Not implemented");
            }
        }
        else
        {
            board[to.x, to.y] = board[from.x, from.y];
            board[from.x, from.y] = null;
        }
    }
}
