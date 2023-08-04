using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceLogic : MonoBehaviour
{
    public GameObject DiceObject;
    public List<GameObject> DiceSides;
    public int diceSide;
    
    public void DiceRotate()
    {
        DiceObject.transform.rotation = Random.rotation;

    }

    public void DiceResultNumber()
    {
        if (DiceSides[0].transform.position.y > DiceSides[1].transform.position.y)
        {
            diceSide = 1;
        }
        else
        {
            diceSide = 2;
        }
    }
}
