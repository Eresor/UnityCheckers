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

    private IEnumerator moveCameraCoroutineObject;

    public GameObject FieldPrefab;

    public GameObject CheckerPrefab;

    public GameObject GameEndUI;

    public Transform Player1CameraPosition;

    public Transform Player2CameraPosition;

    public Color Player1Color = Color.white;

    public Color Player1KingColor = new Color(0.2f,0.2f,0.2f,1);

    public Color Player2Color = Color.black;

    public Color Player2KingColor = new Color(0.8f, 0.8f, 0.8f, 1);

    public Color HighlightColor = Color.blue;

    private Quaternion initialRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);

    public bool FinishedAnimation { get; private set; }

    private Vector3 GetFieldPosition(int x, int y, bool isChecker)
    {
        MeshRenderer renderer = FieldPrefab.GetComponent<MeshRenderer>();
        float fieldSize = renderer.bounds.size.x * FieldPrefab.transform.localScale.x;
        float startFieldPosition = 0.0f-(fieldSize * GameModel.GetInstance().BoardSize / 2);
        return new Vector3(startFieldPosition + x*fieldSize, startFieldPosition + y*fieldSize, (isChecker ? -0.1f : 0.0f));
    }

	// Use this for initialization
	public void Init() {
        FinishedAnimation = true;
        for (int y=0;y<GameModel.GetInstance().BoardSize;y++)
        {
            for(int x=0;x<GameModel.GetInstance().BoardSize;x++)
            {
                MeshRenderer newField = (Instantiate(FieldPrefab, GetFieldPosition(x, y,false), initialRotation) as GameObject).GetComponent<MeshRenderer>();
                newField.GetComponent<FieldData>().X = x;
                newField.GetComponent<FieldData>().Y = y;
                if((x%2==0 && y%2==0) || (x % 2 == 1 && y % 2 == 1))
                {
                    newField.material.color = Color.grey;
                    if(y<GameModel.GetInstance().NumCheckersRows)
                    {
                        GameObject checker = Instantiate(CheckerPrefab, GetFieldPosition(x, y, true), initialRotation) as GameObject;
                        checker.GetComponent<MeshRenderer>().material.color = Player1Color;
                        GameModel.GetInstance().RegiesterCheckerData(checker.GetComponent<CheckerData>(), new Vec2(x, y));
                    }
                    else if(y>=GameModel.GetInstance().BoardSize-GameModel.GetInstance().NumCheckersRows)
                    {
                        GameObject checker = Instantiate(CheckerPrefab, GetFieldPosition(x, y, true), initialRotation) as GameObject;
                        checker.GetComponent<MeshRenderer>().material.color = Player2Color;
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
            previouslyHighlited.gameObject.GetComponent<MeshRenderer>().material.color = (previouslyHighlited.Owner == GameController.GetInstance().Players[0] ? Player1Color : Player2Color);
        }
        if(toHighlight)
        {
            toHighlight.gameObject.GetComponent<MeshRenderer>().material.color = HighlightColor;
        }
    }

    public void SetCameraToPlayer(int playerIdx)
    {
        if (moveCameraCoroutineObject != null)
            StopCoroutine(moveCameraCoroutineObject);
        moveCameraCoroutineObject = CameraCoroutine(playerIdx);
        StartCoroutine(moveCameraCoroutineObject);
    }

    private IEnumerator CameraCoroutine(int playerIdx)
    {
        Transform camTransform = Camera.main.transform;
        Transform destTransform = (playerIdx == 0 ? Player1CameraPosition : Player2CameraPosition);
        Quaternion rotationDir = Quaternion.Slerp(camTransform.rotation, destTransform.rotation, Time.deltaTime * 0.9f);

        float speed = 0.2f;
        for (float t = 0f; t < 1f; t += speed * Time.deltaTime)
        {
            camTransform.position = Vector3.Lerp(camTransform.position, destTransform.position, t);
            transform.rotation = Quaternion.Lerp(camTransform.rotation, destTransform.rotation, t);
            yield return null;
        }
        yield return null;
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
        FinishedAnimation = false;
        StartCoroutine(MoveAnimation(checker, target));
    }

    private IEnumerator MoveAnimation(CheckerData checker, Vec2 target)
    {
        Vector3 targetPos = GetFieldPosition(target.x, target.y, true);

        float speed = 0.5f;
        for (float t = 0f; t < 1f && Vector3.Distance(checker.transform.position,targetPos)>0.1f; t += speed * Time.deltaTime)
        {
            checker.transform.position = Vector3.Lerp(checker.transform.position, targetPos, t);
            yield return null;
        }

        checker.transform.position = targetPos;
        FinishedAnimation = true;
        yield return null;
    }

    public void Promote(CheckerData checker)
    {
        checker.transform.localScale = new Vector3(checker.transform.localScale.x,3 * checker.transform.localScale.y,checker.transform.localScale.z);
        checker.GetComponent<MeshRenderer>().material.color = (checker.Owner == GameController.GetInstance().Players[0] ? Player1KingColor : Player2KingColor);
    }

}
