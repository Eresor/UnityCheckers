using UnityEngine;
using System.Collections;
using System;

public class HumanPlayer : Player {

    enum PlayerState
    {
        SelectingChecker,
        SelectingField
    }

    private PlayerState currentState = PlayerState.SelectingChecker;

    private CheckerData selectedChecker;
    public CheckerData SelectedChecker
    {
        get
        {
            return selectedChecker;
        }
        set
        {
            GameView.GetInstance().HighlightChecker(value, selectedChecker);
            selectedChecker = value;
        }
    }

    public override void ProcessTurn()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit, 50.0f))
            {
                if(currentState==PlayerState.SelectingChecker)
                {
                    StateSelectingChecker(hit);
                }
                else if(currentState == PlayerState.SelectingField)
                {
                    StateSelectingFiled(hit);
                }
            }
            else
            {
                SelectedChecker = null;
                currentState = PlayerState.SelectingChecker;
            }
        }
    }

    private void StateSelectingChecker(RaycastHit hit)
    {
        if(hit.collider.tag==GameController.CheckerTag)
        {
            CheckerData checker = hit.collider.GetComponent<CheckerData>();
            if(GameController.GetInstance().CurrentPlayer==checker.Owner)
            {
                SelectedChecker = checker;
                currentState = PlayerState.SelectingField;
            }
            else
            {
                SelectedChecker = null;
            }
        }
        else if(hit.collider.tag==GameController.FieldTag)
        {
            SelectedChecker = null;
        }
    }

    private void StateSelectingFiled(RaycastHit hit)
    {
        if(hit.collider.tag==GameController.FieldTag)
        {
            FieldData fData = hit.collider.GetComponent<FieldData>();
            Move move = new Move(GameModel.GetInstance().GetCheckerFiled(selectedChecker),new Vec2(fData.X,fData.Y));

            if( ( move = GameModel.GetInstance().IsMoveValid(move) ) != null )
            {
                if( false == GameModel.GetInstance().MoveChecker(move) )
                {
                    SelectedChecker = null;
                    currentState = PlayerState.SelectingChecker;
                    GameController.GetInstance().NextTurn();
                }
            }
            else
            {
                SelectedChecker = null;
                currentState = PlayerState.SelectingChecker;
            }
        }
        else
        {
            SelectedChecker = null;
            currentState = PlayerState.SelectingChecker;
        }
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
