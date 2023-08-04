using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameLogic : MonoBehaviour
{
    #region Variables
    public RegionData RegionDataObject;
    public ViewPanelLogic PanelObject;
    public TextMeshProUGUI StateTextField;
    public TextMeshProUGUI ActionChoice1TextField;
    public TextMeshProUGUI ActionChoice2TextField;
    public GameState previousGameState;
    public GameState currentGameState;
    public int Geo1 = 12000;
    public int Geo2 = 12000;
    public BuildDiceFaces BDresult;
    public int CurrentPlayerIndex = 0;
    public int NumOfPlayers = 2;
    public GameObject HotelPrefab1;
    public GameObject HotelPrefab2;
    public GameObject HotelPrefab3;
    public GameObject HotelPrefab4;
    public GameObject HotelPrefab5;
    public GameObject HotelPrefab6;
    public GameObject HotelPrefab7;
    public GameObject HotelPrefab8;
    public GameObject[] BuildButtons;
    public GameObject[] GateButtons;
    public GameObject[] Gates1;
    public GameObject[] Gates2;
    public GameObject[] Gates3;
    public GameObject[] Gates4;
    public GameObject[] Gates5;
    public GameObject[] Gates6;
    public GameObject[] Gates7;
    public GameObject[] Gates8;
    public TileLogic[] Nodes;
    public int[] AreasOwnership;
    public int[] HotelsOwnership;
    public int[] AvailableAreas;
    public int[] hotelCount;
    
    public int D6Result;
    public bool RollDone = false;
    public bool RollBuildDone = false;
    public bool PlayerClicked = false;
    public bool buy = false;
    public bool build = false;
    public bool buildFree = false;
    public bool buildEntrance = false;
    public bool choseGate = false;
    public bool waitToChooseGate = false;
    public bool EntrancePoint = false;
    public bool gameover = false;
    int BuildHotelCost;
    int areaForHotel;
    int areaForGate;
    int nodeForGate;
    GameObject theHotel;
    GameObject failedHotel;

    #endregion
    // Start is called before the first frame update
    void Start()
    {
        previousGameState = GameState.SWITCH_PLAYER;
        currentGameState = GameState.ROLL_DICE;          //arxiko state
        #region Disappear Gate Torches
        for (int i = 0; i < Gates1.Length; i++)
        {
            Gates1[i].SetActive(false);
        }
        for (int i = 0; i < Gates2.Length; i++)
        {
            Gates2[i].SetActive(false);
        }
        for (int i = 0; i < Gates3.Length; i++)
        {
            Gates3[i].SetActive(false);
        }
        for (int i = 0; i < Gates4.Length; i++)
        {
            Gates4[i].SetActive(false);
        }
        for (int i = 0; i < Gates5.Length; i++)
        {
            Gates5[i].SetActive(false);
        }
        for (int i = 0; i < Gates6.Length; i++)
        {
            Gates6[i].SetActive(false);
        }
        for (int i = 0; i < Gates7.Length; i++)
        {
            Gates7[i].SetActive(false);
        }
        for (int i = 0; i < Gates8.Length; i++)
        {
            Gates8[i].SetActive(false);
        }
        #endregion
    }
    #region Update States
    // Update is called once per frame
    void Update()
    {
        if (currentGameState == GameState.ROLL_DICE && previousGameState != GameState.ROLL_DICE)
        {
            StateTextField.text = "Roll the Step Dice to begin with the Knight.";
            for (int i = 0; i < 8; i++)
            {
                BuildButtons[i].SetActive(false);
            }
            for (int i = 0; i < 29; i++)
            {
                GateButtons[i].SetActive(false);
            }

        }
        else if (currentGameState == GameState.MOVE_PLAYER && previousGameState != GameState.MOVE_PLAYER)
        {

            StateTextField.text = "Click on your player to move.";
            //  Players[currentPlayerIndex].MovePlayer();
        }
        else if (currentGameState == GameState.NODE_ACTION && previousGameState != GameState.NODE_ACTION)
        {

        }
        else if (currentGameState == GameState.SWITCH_PLAYER && previousGameState != GameState.SWITCH_PLAYER)
        {
            FindObjectOfType<AudioManager>().Play("turn");
            if (gameover)
            {
                //StateTextField.text = "Please Exit.";
            }
            else
            {
                StateTextField.text = "Roll for next player.";
                // player index must circle back to 1 that occurs when first playerIndex is 0
                CurrentPlayerIndex = (CurrentPlayerIndex + 1) % NumOfPlayers;
                PanelObject.TogglePlayers(CurrentPlayerIndex);
                ResetVariables();
            }
        }
        previousGameState = currentGameState;
    }
    #endregion

    #region Movement Action
    public void DiceRolled()
    {
        currentGameState = GameState.MOVE_PLAYER;
    }

    public void PlayerMoved()
    {
        currentGameState = GameState.NODE_ACTION;
    }
    #endregion
    
    #region Action Declaration and Termination
    public void PlayerChoseAction(int choice)
    {
        if (choice == 1)
        {
            if (buy)                                 //Player chose to buy 2nd option
            {
                string area = ActionChoice1TextField.text;
                BuyAction(area);
            }
            else if (build || buildFree)     //player chose to build - see if it's possible
            {
               PanelObject.PlayerChoseBuild();
            }
            else if (buildEntrance)
            {
                PanelObject.PlayerChoseBuild();
            }
        }
        else if (choice == 2)
        {
            if (buy)                                 //Player chose to buy 2nd option
            {
                string area = ActionChoice2TextField.text;    
                BuyAction(area);
            }
            else if (build || buildFree || buildEntrance)     //player chose not to build
            {
                ActionDone();
            }
        }
        else
        {
            ActionDone();
        }
    }

    public void ActionDone()
    {
        for (int i = 0; i < 8; i++)
        {
            BuildButtons[i].SetActive(false);
        }
        for (int i = 0; i < 29; i++)
        {
            GateButtons[i].SetActive(false);
        }
        PanelObject.DisplayActionReset();
        currentGameState = GameState.SWITCH_PLAYER;
    }
    #endregion

    #region Buy Area Action
    public void BuyAction(string areaName)
    {
        #region costs
        int area;
        int cost;
        if (areaName == "1.")
        {
            area = 1;
            cost = RegionDataObject.regionArray[0].AreaPrice;
        }
        else if (areaName == "2.")
        {
            area = 2;
            cost = RegionDataObject.regionArray[1].AreaPrice;
        }
        else if (areaName == "3.")
        {
            area = 3;
            cost = RegionDataObject.regionArray[2].AreaPrice;
        }
        else if (areaName == "4.")
        {
            area = 4;
            cost = RegionDataObject.regionArray[3].AreaPrice;
        }
        else if (areaName == "5.")
        {
            area = 5;
            cost = RegionDataObject.regionArray[4].AreaPrice;
        }
        else if (areaName == "6.")
        {
            area = 6;
            cost = RegionDataObject.regionArray[5].AreaPrice;
        }
        else if (areaName == "7.")
        {
            area = 7;
            cost = RegionDataObject.regionArray[6].AreaPrice;
        }
        else if (areaName == "8.") {
            area = 8;
            cost = RegionDataObject.regionArray[7].AreaPrice;
        }
        else
        { area = 0;
            cost = 0;
            ActionDone();
        }
        int hcost = cost / 2;
        #endregion cost 
        for (int i = 1; i < 9; i++)
        {
            if (i == area)
            {
                // if no previous owner
                if (AreasOwnership[i - 1] == 0)
                {
                    PanelObject.DisplayActionReset();
                    StateTextField.text = "You bought the area at full cost.";
                    PanelObject.SpentMoney(CurrentPlayerIndex, cost);
                    AreasOwnership[i - 1] = CurrentPlayerIndex + 1;
                    PanelObject.UpdateOwnershipScore(area, AreasOwnership[i - 1]);
                }
                // if has previous owner buy half price
                else if (AreasOwnership[i - 1] != CurrentPlayerIndex + 1)
                {
                    PanelObject.DisplayActionReset();
                    StateTextField.text = "You bought half price from enemy.";
                    PanelObject.SpentMoney(CurrentPlayerIndex, hcost);
                    AreasOwnership[i - 1] = CurrentPlayerIndex + 1;
                    // if previous owner had hotels, take them
                    if (HotelsOwnership[i-1] != 0)
                    {
                        HotelsOwnership[i - 1] = CurrentPlayerIndex + 1;
                    }
                    PanelObject.UpdateOwnershipScore(area, AreasOwnership[i - 1]);
                }
            }
        }
        Invoke("ActionDone",1f);
    }
    #endregion

    public void PlayerChoseBuild(int area)
    {
        for (int i = 0; i < 8; i++)
        {
            BuildButtons[i].SetActive(false);
        }
        if (build || buildFree)
        {
            for (int i = 0; i < 8; i++)
            {
                // if you want to build hotel, spawn it
                if (i == area - 1 && hotelCount[i] < 4)
                {
                    if (buildFree)
                    {
                        BuildHotelCost = 0;
                        HotelsOwnership[i] = CurrentPlayerIndex + 1;
                        hotelCount[i] += 1;
                        PanelObject.UpdateHotelScore(i, hotelCount[i]);
                        SpawnHotel(area);
                        PanelObject.DisplayAction("Built for free!", null, null, "Ok");
                    }
                    else if (build)
                    {
                        BuildHotelCost = RegionDataObject.regionArray[i].HotelPrices[0];
                        // ask to roll build dice for final cost
                        PanelObject.DisplayActionReset();
                        PanelObject.HintPlayer("Roll the Build Dice.");
                        areaForHotel = area;
                     //   PlayerBuysHotel(area);
                    }
                }
                //if you try to build over 4 hotels your action is done 
                else if (i == area - 1 && hotelCount[i] == 4)
                {
                    //PanelObject.DisplayAction("Max hotels built.", null, null, "Ok");
                    PanelObject.HintPlayer("Max hotels built.");
                    Invoke("ActionDone", 1f);
                }
            }
        }
        else if (buildEntrance)
        {
            if (waitToChooseGate)
            {
                choseGate = true;
                nodeForGate = area;
            }
            else
            {
                PanelObject.PlayerChoseGates(area);
                areaForGate = area;
            }
            if (choseGate)
            {
                PlayerChoseGate(areaForGate, nodeForGate);
                PanelObject.DisplayAction("You built a gate for 200g",null,null,"Ok");
            }
        }
        else
        {
            ActionDone();
        }
    }

    #region Build Hotel Action
    public void BuildDiceRolled(BuildDiceFaces BDresult)
    {
        if (BDresult == BuildDiceFaces.Success)
        {
            HotelsOwnership[areaForHotel-1] = CurrentPlayerIndex + 1;
            hotelCount[areaForHotel - 1] += 1;
            PanelObject.UpdateHotelScore(areaForHotel - 1, hotelCount[areaForHotel - 1]);
            PanelObject.DisplayActionReset();
            SpawnHotel(areaForHotel);
            PanelObject.SpentMoney(CurrentPlayerIndex, BuildHotelCost);
            if (!gameover)
            {
                PanelObject.DisplayAction("Built at intial price.", null, null, "Ok");

            }
        }
        else if (BDresult == BuildDiceFaces.Fail)
        {
            BuildHotelCost = 0;
            PanelObject.DisplayActionReset();
            PanelObject.DisplayAction("You are not able to build yet.", null, null, "Ok");
        }
        else if (BDresult == BuildDiceFaces.Double)
        {
            BuildHotelCost += BuildHotelCost;
            HotelsOwnership[areaForHotel - 1] = CurrentPlayerIndex + 1;
            hotelCount[areaForHotel - 1] += 1;
            PanelObject.UpdateHotelScore(areaForHotel - 1, hotelCount[areaForHotel - 1]);
            PanelObject.DisplayActionReset();
            SpawnHotel(areaForHotel);
            PanelObject.SpentMoney(CurrentPlayerIndex, BuildHotelCost);
            if (!gameover)
            {
                PanelObject.DisplayAction("Built at double price.", null, null, "Ok");
            }
        }
        else if (BDresult == BuildDiceFaces.Free)
        {
            BuildHotelCost = 0;
            HotelsOwnership[areaForHotel - 1] = CurrentPlayerIndex + 1;
            hotelCount[areaForHotel - 1] += 1;
            PanelObject.UpdateHotelScore(areaForHotel - 1, hotelCount[areaForHotel - 1]);
            SpawnHotel(areaForHotel);
            PanelObject.SpentMoney(CurrentPlayerIndex, BuildHotelCost);
            PanelObject.DisplayAction("Built for free!", null, null, "Ok");
        }
        
    }

    #region Spawn Hotel Logic
    public void SpawnHotel(int area)
    {
        int x = 0;
        int z = 0;
        for (int i = 0; i < 8; i++)
        {
            if (i == area - 1)
            {
                if (area == 1)
                {
                    if (hotelCount[i] == 1)
                    {
                        x = RegionDataObject.regionArray[i].posx;
                        z = RegionDataObject.regionArray[i].posz;
                    }
                    else if (hotelCount[i] == 2)
                    {
                        x = RegionDataObject.regionArray[i].posx - 4;
                        z = RegionDataObject.regionArray[i].posz - 1;
                    }
                    else if (hotelCount[i] == 3)
                    {
                        x = RegionDataObject.regionArray[i].posx + 1;
                        z = RegionDataObject.regionArray[i].posz - 3;
                    }
                    else if (hotelCount[i] == 4)
                    {
                        x = RegionDataObject.regionArray[i].posx;
                        z = RegionDataObject.regionArray[i].posz - 5;
                    }
                    theHotel = Instantiate(HotelPrefab1, new Vector3(x, 0, z), Quaternion.Euler(0, 0, 0));
                }
                else if (area == 2)
                {
                    if (hotelCount[i] == 1)
                    {
                        x = RegionDataObject.regionArray[i].posx;
                        z = RegionDataObject.regionArray[i].posz;
                    }
                    else if (hotelCount[i] == 2)
                    {
                        x = RegionDataObject.regionArray[i].posx + 2;
                        z = RegionDataObject.regionArray[i].posz - 2;
                    }
                    else if (hotelCount[i] == 3)
                    {
                        x = RegionDataObject.regionArray[i].posx - 2;
                        z = RegionDataObject.regionArray[i].posz - 2;
                    }
                    else if (hotelCount[i] == 4)
                    {
                        x = RegionDataObject.regionArray[i].posx;
                        z = RegionDataObject.regionArray[i].posz - 4;
                    }
                    theHotel = Instantiate(HotelPrefab2, new Vector3(x, 0, z), Quaternion.Euler(0, 0, 0));
                }
                else if (area == 3)
                {
                    if (hotelCount[i] == 1)
                    {
                        x = RegionDataObject.regionArray[i].posx;
                        z = RegionDataObject.regionArray[i].posz;
                    }
                    else if (hotelCount[i] == 2)
                    {
                        x = RegionDataObject.regionArray[i].posx + 4;
                        z = RegionDataObject.regionArray[i].posz + 1;
                    }
                    else if (hotelCount[i] == 3)
                    {
                        x = RegionDataObject.regionArray[i].posx + 2;
                        z = RegionDataObject.regionArray[i].posz - 3;
                    }
                    else if (hotelCount[i] == 4)
                    {
                        x = RegionDataObject.regionArray[i].posx + 7;
                        z = RegionDataObject.regionArray[i].posz - 4;
                    }
                    theHotel = Instantiate(HotelPrefab3, new Vector3(x, 0, z), Quaternion.Euler(0, 0, 0));
                }
                else if (area == 4)
                {
                    if (hotelCount[i] == 1)
                    {
                        x = RegionDataObject.regionArray[i].posx;
                        z = RegionDataObject.regionArray[i].posz;
                    }
                    else if (hotelCount[i] == 2)
                    {
                        x = RegionDataObject.regionArray[i].posx + 2;
                        z = RegionDataObject.regionArray[i].posz + 5;
                    }
                    else if (hotelCount[i] == 3)
                    {
                        x = RegionDataObject.regionArray[i].posx + 6;
                        z = RegionDataObject.regionArray[i].posz + 8;
                    }
                    else if (hotelCount[i] == 4)
                    {
                        x = RegionDataObject.regionArray[i].posx + 10;
                        z = RegionDataObject.regionArray[i].posz + 9;
                    }
                    theHotel = Instantiate(HotelPrefab4, new Vector3(x, 0, z), Quaternion.Euler(0, 0, 0));
                }
                else if (area == 5)
                {
                    if (hotelCount[i] == 1)
                    {
                        x = RegionDataObject.regionArray[i].posx;
                        z = RegionDataObject.regionArray[i].posz;
                    }
                    else if (hotelCount[i] == 2)
                    {
                        x = RegionDataObject.regionArray[i].posx + 2;
                        z = RegionDataObject.regionArray[i].posz;
                    }
                    else if (hotelCount[i] == 3)
                    {
                        x = RegionDataObject.regionArray[i].posx + 4;
                        z = RegionDataObject.regionArray[i].posz;
                    }
                    else if (hotelCount[i] == 4)
                    {
                        x = RegionDataObject.regionArray[i].posx + 6;
                        z = RegionDataObject.regionArray[i].posz;
                    }
                    theHotel = Instantiate(HotelPrefab5, new Vector3(x, 0, z), Quaternion.Euler(0, 0, 0));
                }
                else if (area == 6)
                {
                    if (hotelCount[i] == 1)
                    {
                        x = RegionDataObject.regionArray[i].posx;
                        z = RegionDataObject.regionArray[i].posz;
                    }
                    else if (hotelCount[i] == 2)
                    {
                        x = RegionDataObject.regionArray[i].posx - 1;
                        z = RegionDataObject.regionArray[i].posz - 4;
                    }
                    else if (hotelCount[i] == 3)
                    {
                        x = RegionDataObject.regionArray[i].posx - 3;
                        z = RegionDataObject.regionArray[i].posz + 3;
                    }
                    else if (hotelCount[i] == 4)
                    {
                        x = RegionDataObject.regionArray[i].posx - 7;
                        z = RegionDataObject.regionArray[i].posz + 2;
                    }
                    theHotel = Instantiate(HotelPrefab6, new Vector3(x, 0, z), Quaternion.Euler(0, 0, 0));
                }
                else if (area == 7)
                {
                    if (hotelCount[i] == 1)
                    {
                        x = RegionDataObject.regionArray[i].posx;
                        z = RegionDataObject.regionArray[i].posz;
                    }
                    else if (hotelCount[i] == 2)
                    {
                        x = RegionDataObject.regionArray[i].posx + 4;
                        z = RegionDataObject.regionArray[i].posz - 3;
                    }
                    else if (hotelCount[i] == 3)
                    {
                        x = RegionDataObject.regionArray[i].posx + 7;
                        z = RegionDataObject.regionArray[i].posz - 6;
                    }
                    else if (hotelCount[i] == 4)
                    {
                        x = RegionDataObject.regionArray[i].posx + 7;
                        z = RegionDataObject.regionArray[i].posz;
                    }
                    theHotel = Instantiate(HotelPrefab7, new Vector3(x, 0, z), Quaternion.Euler(0, 0, 0));
                }
                else if (area == 8)
                {
                    if (hotelCount[i] == 1)
                    {
                        x = RegionDataObject.regionArray[i].posx;
                        z = RegionDataObject.regionArray[i].posz;
                    }
                    else if (hotelCount[i] == 2)
                    {
                        x = RegionDataObject.regionArray[i].posx + 3;
                        z = RegionDataObject.regionArray[i].posz - 1;
                    }
                    else if (hotelCount[i] == 3)
                    {
                        x = RegionDataObject.regionArray[i].posx + 7;
                        z = RegionDataObject.regionArray[i].posz - 2;
                    }
                    else if (hotelCount[i] == 4)
                    {
                        x = RegionDataObject.regionArray[i].posx + 8;
                        z = RegionDataObject.regionArray[i].posz + 2;
                    }
                    theHotel = Instantiate(HotelPrefab8, new Vector3(x, 0, z), Quaternion.Euler(0, 0, 0));
                }
            }
        }
    }
    #endregion
    
    #endregion

    #region Build Gate Action
    public void PlayerChoseGate(int area, int node)
    {
        //the unavailable gates are not chosen at all because they where never displayed as an option
        string Q = "Gate at Area " + area + " on node " + node;

        // update logic for tiles
        for (int i = 0; i < 30; i++)
        {
            if (i == node)
            {
                if (area == 1 || area == 4 || area == 6 || area == 7 || area == 8)
                {
                    //outter side of nodes so the 2nd child of the tile object
                    if (CurrentPlayerIndex == 0)
                    {
                        if (Nodes[i].transform.GetChild(0).tag != "EntranceOwner2")
                        {
                            Nodes[i].transform.GetChild(1).tag = "EntranceOwner1";
                        }
                        else
                        {
                            Q = "You can't add a gate here, your enemy has one.";
                            PanelObject.DisplayAction(Q, null, null, "Ok");
                            return;

                            Invoke("ActionDone", 2f);
                        }
                    }
                    else if (CurrentPlayerIndex == 1)
                    {
                        if (Nodes[i].transform.GetChild(0).tag != "EntranceOwner1")
                        {
                            Nodes[i].transform.GetChild(1).tag = "EntranceOwner2";
                        }
                        else
                        {
                            Q = "You can't add a gate here, your enemy has one.";
                            PanelObject.DisplayAction(Q, null, null, "Ok");
                            return;
                            Invoke("ActionDone", 2f);
                        }
                    }
                }
                else if (area == 2 || area == 3 || area == 5)
                {
                    //inner side of nodes so the 1st child of the tile object
                    if (CurrentPlayerIndex == 0)
                    {
                        if (Nodes[i].transform.GetChild(1).tag != "EntranceOwner2")
                        {
                            Nodes[i].transform.GetChild(0).tag = "EntranceOwner1";
                        }
                        else
                        {
                            Q = "You can't add a gate here, your enemy has one.";
                            PanelObject.DisplayAction(Q, null, null, "Ok");
                            return;

                            Invoke("ActionDone", 2f);
                        }
                    }
                    else if (CurrentPlayerIndex == 1)
                    {
                        if (Nodes[i].transform.GetChild(1).tag != "EntranceOwner!")
                        {
                            Nodes[i].transform.GetChild(0).tag = "EntranceOwner2";
                        }
                        else
                        {
                            Q = "You can't add a gate here, your enemy has one.";
                            PanelObject.DisplayAction(Q, null, null, "Ok");
                            return;

                            Invoke("ActionDone", 2f);
                        }
                    }
                }
            }
        }
        if (!EntrancePoint)
        {
            PanelObject.DisplayAction(Q, null, null, "Ok");
        }
        else
        {
            PanelObject.HintPlayer(Q);
        }
        PanelObject.SpentMoney(CurrentPlayerIndex, 200);

        //spawn gates
        #region Spawn Gates
        if (area == 1)
        {
            if (node == 2) { Gates1[0].SetActive(true); }
            else if (node == 3) { Gates1[1].SetActive(true); }
            else if (node == 4) { Gates1[2].SetActive(true); }
            else if (node == 5) { Gates1[3].SetActive(true); }
        }
        else if (area == 2)
        {
            if (node == 1) { Gates2[0].SetActive(true); }
            else if (node == 2) { Gates2[1].SetActive(true); }
            else if (node == 3) { Gates2[2].SetActive(true); }
            else if (node == 4) { Gates2[3].SetActive(true); }
            else if (node == 5) { Gates2[4].SetActive(true); }
            else if (node == 6) { Gates2[5].SetActive(true); }
        }
        else if (area == 3)
        {
            if (node == 7) { Gates3[0].SetActive(true); }
            else if (node == 8) { Gates3[1].SetActive(true); }
            else if (node == 20) { Gates3[2].SetActive(true); }
            else if (node == 21) { Gates3[3].SetActive(true); }
            else if (node == 22) { Gates3[4].SetActive(true); }
            else if (node == 23) { Gates3[5].SetActive(true); }
            else if (node == 27) { Gates3[6].SetActive(true); }
            else if (node == 28) { Gates3[7].SetActive(true); }
            else if (node == 29) { Gates3[8].SetActive(true); }
        }
        else if (area == 4)
        {
            if (node == 8) { Gates4[0].SetActive(true); }
            else if (node == 9) { Gates4[1].SetActive(true); }
            else if (node == 10) { Gates4[2].SetActive(true); }
            else if (node == 11) { Gates4[3].SetActive(true); }
            else if (node == 12) { Gates4[4].SetActive(true); }
            else if (node == 13) { Gates4[5].SetActive(true); }
            else if (node == 14) { Gates4[6].SetActive(true); }
        }
        else if (area == 5)
        {
            if (node == 9) { Gates5[0].SetActive(true); }
            else if (node == 10) { Gates5[1].SetActive(true); }
            else if (node == 11) { Gates5[2].SetActive(true); }
            else if (node == 12) { Gates5[3].SetActive(true); }
            else if (node == 13) { Gates5[4].SetActive(true); }
            else if (node == 14) { Gates5[5].SetActive(true); }
            else if (node == 15) { Gates5[6].SetActive(true); }
            else if (node == 16) { Gates5[7].SetActive(true); }
            else if (node == 17) { Gates5[8].SetActive(true); }
            else if (node == 18) { Gates5[9].SetActive(true); }
            else if (node == 19) { Gates5[10].SetActive(true); }
        }
        else if (area == 6)
        {
            if (node == 15) { Gates6[0].SetActive(true); }
            else if (node == 16) { Gates6[1].SetActive(true); }
            else if (node == 17) { Gates6[2].SetActive(true); }
            else if (node == 18) { Gates6[3].SetActive(true); }
            else if (node == 19) { Gates6[4].SetActive(true); }
        }
        else if (area == 7)
        {
            if (node == 20) { Gates7[0].SetActive(true); }
            else if (node == 21) { Gates7[1].SetActive(true); }
            else if (node == 22) { Gates7[2].SetActive(true); }
            else if (node == 23) { Gates7[3].SetActive(true); }
            else if (node == 24) { Gates7[4].SetActive(true); }
        }
        else if (area == 8)
        {
            if (node == 25) { Gates8[0].SetActive(true); }
            else if (node == 26) { Gates8[1].SetActive(true); }
            else if (node == 27) { Gates8[2].SetActive(true); }
            else if (node == 28) { Gates8[3].SetActive(true); }
            else if (node == 29) { Gates8[4].SetActive(true); }
        }
        #endregion 
    }
    #endregion

    public bool FindAvailablePlots()
    {
        int j = 0;
        for (int i = 0; i < 8; i++)
        {
            if (AreasOwnership[i] == CurrentPlayerIndex+1)
            {
                AvailableAreas[j] = i + 1;
                BuildButtons[i].SetActive(true);
                j++;
            }
        }
        if (j != 0)
        {
            return true;
        }
        else //player has no areas owned
        {
            return false;
        }
    }

    public bool FindAvailableGates(int area)
    {
        if (HotelsOwnership[area-1] == CurrentPlayerIndex+1)
        {
                if (area == 1)
                {
                    GateButtons[1].SetActive(true);
                    GateButtons[2].SetActive(true);
                    GateButtons[3].SetActive(true);
                    GateButtons[4].SetActive(true);
                }else if (area == 2)
                {
                    GateButtons[0].SetActive(true);
                    GateButtons[1].SetActive(true);
                    GateButtons[2].SetActive(true);
                    GateButtons[3].SetActive(true);
                    GateButtons[4].SetActive(true);
                    GateButtons[5].SetActive(true);
                }
                else if (area == 3)
                {
                    GateButtons[6].SetActive(true);
                    GateButtons[7].SetActive(true);
                    GateButtons[19].SetActive(true);
                    GateButtons[20].SetActive(true);
                    GateButtons[21].SetActive(true);
                    GateButtons[22].SetActive(true);
                    GateButtons[28].SetActive(true);
                    GateButtons[27].SetActive(true);
                    GateButtons[26].SetActive(true);
                }
                else if (area == 4)
                {
                    GateButtons[7].SetActive(true);
                    GateButtons[8].SetActive(true);
                    GateButtons[9].SetActive(true);
                    GateButtons[10].SetActive(true);
                    GateButtons[11].SetActive(true);
                    GateButtons[12].SetActive(true);
                    GateButtons[13].SetActive(true);
                }
                else if (area == 5)
                {
                    GateButtons[9].SetActive(true);
                    GateButtons[10].SetActive(true);
                    GateButtons[11].SetActive(true);
                    GateButtons[12].SetActive(true);
                    GateButtons[13].SetActive(true);
                    GateButtons[14].SetActive(true);
                    GateButtons[15].SetActive(true);
                    GateButtons[16].SetActive(true);
                    GateButtons[17].SetActive(true);
                    GateButtons[18].SetActive(true);
                }
                else if (area == 6)
                {
                    GateButtons[14].SetActive(true);
                    GateButtons[15].SetActive(true);
                    GateButtons[16].SetActive(true);
                    GateButtons[17].SetActive(true);
                    GateButtons[18].SetActive(true);
                }
                else if (area == 7)
                {
                    GateButtons[19].SetActive(true);
                    GateButtons[20].SetActive(true);
                    GateButtons[21].SetActive(true);
                    GateButtons[22].SetActive(true);
                    GateButtons[23].SetActive(true);
                }
                else if (area == 5)
                {
                    GateButtons[24].SetActive(true);
                    GateButtons[25].SetActive(true);
                    GateButtons[26].SetActive(true);
                    GateButtons[27].SetActive(true);
                    GateButtons[28].SetActive(true);
                }
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ResetVariables()
    {
        RollDone = false;
        RollBuildDone = false;
        PlayerClicked = false;
        buy = false;
        build = false;
        buildFree = false;
        buildEntrance = false;
        choseGate = false;
        waitToChooseGate = false;
        EntrancePoint = false;
        D6Result = 0;
        BuildHotelCost = 0;
        currentGameState = GameState.ROLL_DICE;
        areaForGate = 0;
        nodeForGate = 0;
        areaForHotel = 0;
}
    public void ActionExit()
    {
        Application.Quit();
    }
}


public enum GameState
{
    ROLL_DICE, MOVE_PLAYER, NODE_ACTION, SWITCH_PLAYER
}