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

    public override bool Equals(object obj)
    {
        if(obj.GetType()==typeof(Vec2))
        {
            return (obj as Vec2).x == this.x && (obj as Vec2).y == this.y;
        }
        return false;
    }
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

    public override bool Equals(object obj)
    {
        if(obj.GetType()==typeof(Move))
        {
            Move other = obj as Move;
            return other.From.Equals(this.From) && other.To.Equals(this.To);
        }
        return false;
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

    private List<Move> PossibleMoves;

    public void UpdatePossibleMoves()
    {
        PossibleMoves = GetPossibleMoves(board, BoardSize, GameController.GetInstance().CurrentPlayer);
    }

    public Move IsMoveValid(Move selectedMove)
    {
        foreach(Move move in PossibleMoves)
        {
            if(selectedMove.Equals(move))
            {
                return move;
            }
        }
        return null;
    }

	// Use this for initialization
	public void Init() {

        board = new CheckerData[BoardSize, BoardSize];
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public Vec2 GetCheckerFiled(CheckerData cheker)
    {
        for(int y=0;y<BoardSize;y++)
        {
            for(int x=0;x<BoardSize;x++)
            {
                if(cheker==board[x,y])
                {
                    return new Vec2(x, y);
                }
            }
        }
        throw new System.Exception("Invalid checker not on board");
    }

    public void RegiesterCheckerData(CheckerData data, Vec2 position)
    {
        board[position.x, position.y] = data;
    }

    private static bool[,] CopyIsKingProperty(CheckerData[,] board)
    {
        Vec2 size = new Vec2(board.GetLength(0), board.GetLength(1));
        bool[,] copy = new bool[size.x,size.y];
        for(int y=0;y<size.y;y++)
        {
            for(int x=0;x<size.x; x++)
            {
                if(board[x,y])
                {
                    copy[x, y] = board[x, y].IsKing;
                }
            }
        }
        return copy;
    }

    private static void RestoreOrginalData(CheckerData[,] board, bool[,] copy)
    {
        Vec2 size = new Vec2(board.GetLength(0), board.GetLength(1));
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                if(board[x,y])
                {
                    board[x, y].IsKing = copy[x, y];
                }
            }
        }
    }

    public static List<Move> GetPossibleMoves(CheckerData[,] board, int size, Player player)
    {
        List<Move> ret = new List<Move>();
        bool[,] copy = CopyIsKingProperty(board);
        bool foundCaptureMove = false;
        int maxCombo = 0;
        for (int y=0;y<size;y++)
        {
            for(int x=0;x<size;x++)
            {
                CheckerData curr;
                if( ( curr = board[x,y] ) != null && curr.Owner == player)
                {
                    Vec2 currPos = new Vec2(x, y);
                    List<Vec2> captured = GetCaptureList(currPos, board, size);
                    if (captured.Count == 0 && foundCaptureMove==false)
                    {
                        List<Vec2> nonKillingMoves = GetNonKillingMovesForField(currPos, board, size);
                        foreach(Vec2 target in nonKillingMoves)
                        {
                            ret.Add(new Move(currPos, target));
                        }
                    }
                    else
                    {
                        foundCaptureMove = true;
                        List<Vec2> possibleTargets = new List<Vec2>();
                        foreach (Vec2 target in captured)
                        {
                            Move move = new Move(currPos, target);
                            CheckerData[,] clone = board.Clone() as CheckerData[,];
                            MoveSimulation(currPos, target, clone);
                            int currCombo = SimulateCombo(target, clone, size, 0, move);
                            if(currCombo > maxCombo)
                            {
                                ret.Clear();
                                maxCombo = currCombo;
                                ret.Add(move);
                            }
                            else if(currCombo==maxCombo)
                            {
                                ret.Add(move);
                            }
                        }
                    }
                }
            }
        }
        RestoreOrginalData(board, copy);
        return ret;
    }

    static int SimulateCombo(Vec2 position, CheckerData[,] board, int size, int counter, Move move)
    {
        int insideCounter = 1;
        List<Vec2> captured = GetCaptureList(position, board, size);
        if (captured.Count == 0)
        {
            move.Children = null;
            return counter;
        }
        else
        {
            move.Children = new List<Move>();
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
            if(target.x>=0 && target.x<size && target.y>=0 && target.y <size && board[target.x,target.y] == null)
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
            //throw new System.Exception("To repair.");
            bool leftFinished = false, rightFinished = false;
            for (int y=field.y+1;y<size;y++)
            {
                int distance = Mathf.Abs(field.y - y);

                if(field.x + distance<size && !leftFinished && board[field.x+distance, y]==null)
                {
                    ret.Add(new Vec2(field.x + distance, y));
                }
                else
                {
                    leftFinished = true;
                }

                if (field.x - distance>=0 && !rightFinished && board[field.x - distance, y] == null)
                {
                    ret.Add(new Vec2(field.x - distance, y));
                }
                else
                {
                    rightFinished = true;
                }
            }

            leftFinished = false;
            rightFinished = false;
            for (int y = field.y - 1; y >= 0; y--)
            {
                int distance = Mathf.Abs(field.y - y);

                if (field.x + distance < size && !leftFinished && board[field.x + distance, y] == null)
                {
                    ret.Add(new Vec2(field.x + distance, y));
                }
                else
                {
                    leftFinished = true;
                }

                if (field.x - distance>=0 && !rightFinished && board[field.x - distance, y] == null)
                {
                    ret.Add(new Vec2(field.x -distance, y));
                }
                else
                {
                    rightFinished = true;
                }
            }
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
        else
        {
            bool leftFinished = false, rightFinished = false;
            for (int y = field.y + 1; y < size-1; y++)
            {

                int targetX = field.x + Mathf.Abs(field.y - y);
                int behindX = targetX + 1;

                if (behindX < size && !leftFinished && board[targetX, y] != null && board[targetX, y].Owner!=checker.Owner)
                {
                    for (int yy = y + 1; yy < size; yy++)
                    {
                        behindX = field.x + Mathf.Abs(field.y - yy);
                        if (behindX >= size || board[behindX, yy] != null)
                        {
                            break;
                        }
                        else
                        {
                            ret.Add(new Vec2(behindX, yy));
                        }
                    }
                    leftFinished = true;
                }

                targetX = field.x - Mathf.Abs(field.y - y);
                behindX = targetX - 1;
                if (behindX >= 0 && !rightFinished && board[targetX, y] != null && board[targetX, y].Owner != checker.Owner)
                {
                    for (int yy = y + 1; yy < size; yy++)
                    {
                        behindX = field.x - Mathf.Abs(field.y - yy);
                        if (behindX < 0 || board[behindX, yy] != null)
                        {
                            break;
                        }
                        else
                        {
                            ret.Add(new Vec2(behindX, yy));
                        }
                    }
                    leftFinished = true;
                }

            }

            leftFinished = false;
            rightFinished = false;
            for (int y = field.y -1; y > 0; y--)
            {
                int targetX = field.x + Mathf.Abs(field.y - y);
                int behindX = targetX + 1;

                if (behindX < size && !leftFinished && board[targetX, y] != null && board[targetX, y].Owner != checker.Owner)
                {
                    for(int yy = y-1; yy>=0; yy--)
                    {
                        behindX = field.x + Mathf.Abs(field.y - yy);
                        if (behindX>=size || board[behindX,yy]!=null)
                        {
                            break;
                        }
                        else
                        {
                            ret.Add(new Vec2(behindX, yy));
                        }
                    }
                    leftFinished = true;
                }

                targetX = field.x - Mathf.Abs(field.y - y);
                behindX = targetX - 1;
                if (behindX >= 0 && !rightFinished && board[targetX, y] != null && board[targetX, y].Owner != checker.Owner)
                {
                    for (int yy = y - 1; yy >= 0; yy--)
                    {
                        behindX = field.x - Mathf.Abs(field.y - yy);
                        if (behindX < 0 || board[behindX, yy] != null)
                        {
                            break;
                        }
                        else
                        {
                            ret.Add(new Vec2(behindX, yy));
                        }
                    }
                    leftFinished = true;
                }
            }
        }


        return ret;
    }

    //this function do not validate move!
    public static void MoveSimulation(Vec2 from, Vec2 to, CheckerData[,] board, System.Action<CheckerData, Vec2> moveAction = null, System.Action<CheckerData> destroyAction = null, System.Action<CheckerData> promoteAction = null)
    {
        CheckerData selected = board[from.x, from.y];
        Vec2 direction = new Vec2((to.x - from.x) / Mathf.Abs(to.x - from.x), (to.y - from.y) / Mathf.Abs(to.y - from.y));

        if (selected.IsKing == false)
        {

            if(Mathf.Abs(to.y-from.y)>1)
            {
                CheckerData killed = board[from.x + direction.x, from.y + direction.y];
                board[from.x + direction.x, from.y + direction.y] = null;
                if (destroyAction != null)
                {
                    destroyAction(killed);
                }
            }

            board[to.x, to.y] = selected;
            board[from.x, from.y] = null;
            if (moveAction != null)
            {
                moveAction(board[to.x, to.y], to);
            }

            //checking promotion into king
            if ((to.y == 0 && selected.Owner.Direction==-1) || (to.y == board.GetUpperBound(0) && selected.Owner.Direction == 1))
            {
                selected.IsKing = true;
                if(promoteAction!=null)
                {
                    promoteAction(selected);
                }
            }
        }
        else
        {
            //iterate through board to check if there are enemies;
            CheckerData killed;
            for (int i = 1; i < Mathf.Abs(to.y - from.y); i++)
            {

                if ((killed = board[from.x + i * direction.x, from.y + i * direction.y]) != null && killed.Owner != selected.Owner)
                {
                    board[from.x + i * direction.x, from.y + i * direction.y] = null;
                    if (destroyAction != null)
                    {
                        destroyAction(killed);
                    }
                    break;
                }

            }

            board[to.x, to.y] = selected;
            board[from.x, from.y] = null;

            if (moveAction != null)
            {
                moveAction(board[to.x, to.y], to);
            }

        }
    }

    public bool MoveChecker(Move move)
    {
        MoveSimulation(move.From, move.To, board, GameView.GetInstance().MoveChecker,GameView.GetInstance().DestroyChecker,GameView.GetInstance().Promote);
        if(move.Children==null || move.Children.Count==0)
        {
            return false;
        }
        else
        {
            PossibleMoves = move.Children;
            return true;
        }
    }

    public int CheckVictory()
    {
        int[] numPCheckers = { 0, 0 };
        foreach(CheckerData checker in Board)
        {
            if(checker)
            {
                numPCheckers[(checker.Owner == GameController.GetInstance().Players[0] ? 0 : 1) ]++;
            }
        }
        return (numPCheckers[0] == 0 ? 1 : ( numPCheckers[1] == 0 ? 0 : -1 ) );
    }
}