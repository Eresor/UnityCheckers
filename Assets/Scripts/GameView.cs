using UnityEngine;
using System.Collections;

public class GameView : MonoBehaviour {

    public GameObject FieldPrefab;

    public GameObject CheckerPrefab;

    private Vector3 GetFieldPosition(int x, int y, bool isChecker)
    {
        SpriteRenderer renderer = FieldPrefab.GetComponent<SpriteRenderer>();
        float fieldSize = renderer.sprite.bounds.size.x * FieldPrefab.transform.localScale.x;
        float startFieldPosition = 0.0f-(fieldSize * GameModel.GetInstance().BoardSize / 2);
        return new Vector3(startFieldPosition + x*fieldSize, startFieldPosition + y*fieldSize, (isChecker ? -1.0f : 0.0f));
    }

	// Use this for initialization
	void Start () {

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
                        GameModel.GetInstance().RegiesterCheckerData(checker.GetComponent<CheckerData>(), new Vec2(x, y));
                    }
                    else if(y>=GameModel.GetInstance().BoardSize-GameModel.GetInstance().NumCheckersRows)
                    {
                        GameObject checker = Instantiate(CheckerPrefab, GetFieldPosition(x, y, true), Quaternion.identity) as GameObject;
                        checker.GetComponent<SpriteRenderer>().color = Color.black;
                        GameModel.GetInstance().RegiesterCheckerData(checker.GetComponent<CheckerData>(), new Vec2(x, y));
                    }
                }
            }
        }
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
