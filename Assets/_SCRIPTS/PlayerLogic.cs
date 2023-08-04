using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerLogic : MonoBehaviour
{
    #region Variables
    
    public GameLogic GameLogicObject;
    public ViewPanelLogic PanelLogicObject;
    public TileLogic StartingTile;
    TileLogic currentTile;
    TileLogic destinationTile;
    string choice1, choice2, choice3, Q;
    Vector3 targetPosition;
    Vector3 currentVelocity ;
    float smoothTime = 0.17f;
    float smoothDistance = 0.0001f;
    TileLogic[] steps;
    int stepsIndex;
    bool isMoving = false;
    public GameObject EnemyPlayerObject;

    #endregion
    // Start is called before the first frame update
    void Start()
    {
        targetPosition = this.transform.position;
     //        PanelLogicObject = GameObject.FindObjectOfType<ViewPanelLogic>();

    }

    // Update is called once per frame
    void Update()
    {
       if (isMoving == false){return;}
        
       if (Vector3.Distance(this.transform.position, targetPosition) < smoothDistance)  //reached destination
        {
            /*if (steps != null && stepsIndex < steps.Length)
            {
                SetNewTargetPosition(steps[stepsIndex].transform.position);
                stepsIndex++;
            }
            else
            {*/
                this.isMoving = false;
                GameLogicObject.PlayerMoved();
              //  StateManagerObject.DoneMoving = true;
            //}
        }
        this.transform.position = Vector3.SmoothDamp(this.transform.position, targetPosition, ref currentVelocity, smoothTime); // this is smooth teleport
    }

    void OnMouseUp()
    {
        //check if dice rolled
        if (GameLogicObject.RollDone == false)
        {
            Debug.Log("Roll first!");
            PanelLogicObject.HintPlayer("Roll dice first.");
            return;
        }
        if (this.tag == "Player1" && GameLogicObject.CurrentPlayerIndex == 1 || this.tag == "Player2" && GameLogicObject.CurrentPlayerIndex == 0)
        {
            Debug.Log("Wrong pawn!");
            PanelLogicObject.HintPlayer("Please click your own player.");
            return;
        }
        // check if clicked already
        if (GameLogicObject.PlayerClicked == true){return;}

        int StepsToMove = GameLogicObject.D6Result;
       // steps = new TileLogic[StepsToMove];
        destinationTile = currentTile;
        for (int i = 0; i < StepsToMove; i++)
        {
            if (destinationTile == null)
            {
                destinationTile = StartingTile;
            }
            else
            {
                destinationTile = destinationTile.NextTile;
                // check pass-through points
                if (destinationTile.NextTile.tag == "Node_GainMoneyPoint" && i<StepsToMove-1 || (i == StepsToMove-1 && destinationTile.tag == "Node_GainMoneyPoint"))
                {
                    //pass the gainmoneyPoint by taking one more step only if the destination is the exact GMPtile or a next one
                    StepsToMove++;
                    if(this.name == "Player1") {PanelLogicObject.GainMoney(0);}
                    else if (this.name == "Player2"){PanelLogicObject.GainMoney(1);}
                }
                else if (destinationTile.NextTile.tag == "Node_BuyEntrancePoint" && i < StepsToMove - 1 || (i == StepsToMove-1 && destinationTile.tag == "Node_BuyEntrancePoint"))
                {
                    StepsToMove++;
                    Debug.Log("Buy your entrance!");
                    GameLogicObject.EntrancePoint = true;
                }
                //check if other player is on destination tile
                if (Vector3.Distance(EnemyPlayerObject.transform.position, destinationTile.transform.position) < smoothDistance && (i==StepsToMove-1 || StepsToMove==1))
                {
                    StepsToMove++;
                }
                // if enemy entrance -> invoke action func for that
            }
        //    steps[i] = destinationTile;
        }
       // stepsIndex = 0;

        // this.transform.position = destinationTile.transform.position;    //this is teleport
        SetNewTargetPosition(destinationTile.transform.position);           // this for smooth teleport
        currentTile = destinationTile;
        NodeManager(destinationTile);
        //click done
        //  StateManagerObject.DoneClicking = true;
        GameLogicObject.PlayerClicked = true;
        this.isMoving = true;
    }

    void SetNewTargetPosition( Vector3 pos)
    {
        targetPosition = pos;
        currentVelocity = Vector3.zero; //this resets it for starting a new target move
    }

    /*void MoveQueue()
    {
        //if haven't reach the destination yet
        //find our new current postitionn
        //smoothDamp takes current and destination position and a certain amount of time to reach destination
        //ref cause the function can change what the value is

            if (steps != null && stepsIndex < steps.Length)
            {
                TileLogic nextTile = steps[stepsIndex];
                SetNewTargetPosition(nextTile.transform.position);
                stepsIndex++;
            }
    }*/

    public void NodeManager(TileLogic theTile)
    {
        //check for enemy entrance
        if (EnemyPlayerObject.tag == "Player1" && (theTile.transform.GetChild(0).tag == "EntranceOwner1" || theTile.transform.GetChild(1).tag == "EntranceOwner1"))
        {
            Debug.Log("pay your enemy!");
            PanelLogicObject.SpentMoney(1, 100);
            PanelLogicObject.DisplayActionReset();
            PanelLogicObject.HintPlayer("You paid 100 to Knight for the night.");
            
        }
        else if (EnemyPlayerObject.tag == "Player2" && (theTile.transform.GetChild(0).tag == "EntranceOwner2" || theTile.transform.GetChild(1).tag == "EntranceOwner2"))
        {
            Debug.Log("pay your enemy!");
            PanelLogicObject.SpentMoney(0, 100);
            PanelLogicObject.DisplayActionReset();
            PanelLogicObject.HintPlayer("You paid 100 to Hornet for the night.");
        }
        //check for entrance point
        if (GameLogicObject.EntrancePoint)
        {
            GameLogicObject.buildEntrance = true;
            Q = "Do you want to build any Entrance?";
            choice1 = "YES";
            choice2 = "NO";
            choice3 = null;
            PanelLogicObject.DisplayAction(Q, choice1, choice2, choice3);
        }
        // then do the actual tile action
        if (theTile.tag == "NodeBuy")
        {
            Q = "Select which area you wish to buy.";
            if (theTile.name == "Node_Buy (11)" || theTile.name == "Node_Buy (12)")
            {
                choice1 = "1.";
                choice2 = "2.";
            }
            else if (theTile.name == "Node_Buy (10)")
            {
                choice1 = "3.";
                choice2 = "8.";
            }
            else if (theTile.name == "Node_Buy (9)" || theTile.name == "Node_Buy (8)")
            {
                choice1 = "3.";
                choice2 = "7.";
            }
            else if (theTile.name == "Node_Buy (7)" || theTile.name == "Node_Buy (6)")
            {
                choice1 = "5.";
                choice2 = "6.";
            }
            else if (theTile.name == "Node_Buy (5)" || theTile.name == "Node_Buy (4)" || theTile.name == "Node_Buy (3)")
            {
                choice1 = "4.";
                choice2 = "5.";
            }
            else if (theTile.name == "Node_Buy (2)")
            {
                theTile.RightBorder.tag = "EntranceOwner1";
                //theTile.Entrance.SetActive(true);
                choice1 = "3.";
                choice2 = "4.";
            }
            else if (theTile.name == "Node_Buy (1)")
            {
                theTile.RightBorder.tag = "EntranceOwner1";
                choice1 = "3.";
                choice2 = null;
            }
            choice3 = "None";
            PanelLogicObject.DisplayAction(Q, choice1, choice2, choice3);
            GameLogicObject.buy = true;
        }
        else if (theTile.tag == "NodeBuild")
        {
            GameLogicObject.build = true;
            Q = "Do you want to build any hotels?";
            choice1 = "YES";
            choice2 = "NO";
            choice3 = null;
            PanelLogicObject.DisplayAction(Q, choice1, choice2, choice3);
        }
        else if (theTile.tag == "NodeFreeBuild")
        {
            GameLogicObject.buildFree = true;
            Q = "Do you want to build any hotels?";
            choice1 = "YES";
            choice2 = "NO";
            choice3 = null;
            PanelLogicObject.DisplayAction(Q, choice1, choice2, choice3);
        }
        else if (theTile.tag == "NodeEntrance")
        {
            GameLogicObject.buildEntrance = true;
            Q = "Do you want to build any Entrance?";
            choice1 = "YES";
            choice2 = "NO";
            choice3 = null;
            PanelLogicObject.DisplayAction(Q, choice1, choice2, choice3);
        }
        else
        {
            Q = "No action to be done";
            choice1 = null;
            choice2 = null;
            choice3 = null;
            PanelLogicObject.DisplayAction(Q, choice1, choice2, choice3);
            GameLogicObject.PlayerChoseAction(3);
        }
    }
}
