using UnityEngine;
using System.Collections;

public abstract class Player : MonoBehaviour {

    public abstract void ProcessTurn();

    public int Direction;

}
