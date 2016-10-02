using UnityEngine;
using System.Collections;

public class GameView : MonoBehaviour {

    public static GameView GetInstance()
    {
        if (instance == null)
        {
            instance = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameView>();
        }
        return instance;
    }

    private static GameView instance;

    public GameObject FieldPrefab;

    public GameObject CheckerPrefab;

    public GameObject GameEndUI;

    public Color Player1Color = Color.white;

    public Color Player1KingColor = new Color(0.2f,0.2f,0.2f,1);

    public Color Player2Color = Color.black;

    public Color Player2KingColor = new Color(0.8f, 0.8f, 0.8f, 1);

    public Color HighlightColor = Color.blue;

    private Vector3 GetFieldPosition(int x, int y, bool isChecker)
    {
        SpriteRenderer renderer = FieldPrefab.GetComponent<SpriteRenderer>();
        float fieldSize = renderer.sprite.bounds.size.x * FieldPrefab.transform.localScale.x;
        float startFieldPosition = 0.0f-(fieldSize * GameModel.GetInstance().BoardSize / 2);
        return new Vector3(startFieldPosition + x*fieldSize, startFieldPosition + y*fieldSize, (isChecker ? -1.0f : 0.0f));
    }

	// Use this for initialization
	public void Init() {
        for(int y=0;y<GameModel.GetInstance().BoardSize;y++)
        {
            for(int x=0;x<GameModel.GetInstance().BoardSize;x++)
            {
                SpriteRenderer newField = (Instantiate(FieldPrefab, GetFieldPosition(x, y,false), Quaternion.identity) as GameObject).GetComponent<SpriteRenderer>();
                newField.GetComponent<FieldData>().X = x;
                newField.GetComponent<FieldData>().Y = y;
                if((x%2==0 && y%2==0) || (x % 2 == 1 && y % 2 == 1))
                {
                    newField.color = Color.grey;
                    if(y<GameModel.GetInstance().NumCheckersRows)
                    {
                        GameObject checker = Instantiate(CheckerPrefab, GetFieldPosition(x, y, true), Quaternion.identity) as GameObject;
                        checker.GetComponent<SpriteRenderer>().color = Player1Color;
                        GameModel.GetInstance().RegiesterCheckerData(checker.GetComponent<CheckerData>(), new Vec2(x, y));
                    }
                    else if(y>=GameModel.GetInstance().BoardSize-GameModel.GetInstance().NumCheckersRows)
                    {
                        GameObject checker = Instantiate(CheckerPrefab, GetFieldPosition(x, y, true), Quaternion.identity) as GameObject;
                        checker.GetComponent<SpriteRenderer>().color = Player2Color;
                        GameModel.GetInstance().RegiesterCheckerData(checker.GetComponent<CheckerData>(), new Vec2(x, y));
                    }
                }
            }
        }
	
	}
	
    public void HighlightChecker(CheckerData toHighlight, CheckerData previouslyHighlited=null)
    {
        if(previouslyHighlited)
        {
            previouslyHighlited.gameObject.GetComponent<SpriteRenderer>().color = (previouslyHighlited.Owner == GameController.GetInstance().Players[0] ? Player1Color : Player2Color);
        }
        if(toHighlight)
        {
            toHighlight.gameObject.GetComponent<SpriteRenderer>().color = HighlightColor;
        }
    }

	// Update is called once per frame
	void Update () {
	
	}

    public void DestroyChecker(CheckerData checker)
    {
        Destroy(checker.gameObject);
    }

    public void MoveChecker(CheckerData checker, Vec2 target)
    {
        checker.gameObject.transform.position = GetFieldPosition(target.x, target.y, true);
    }

    public void Promote(CheckerData checker)
    {
        checker.GetComponent<SpriteRenderer>().color = (checker.Owner == GameController.GetInstance().Players[0] ? Player1KingColor : Player2KingColor);
    }

}
