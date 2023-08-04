using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ViewPanelLogic : MonoBehaviour
{
    #region Variables
    public Toggle Player1_toggle;
    public Toggle Player2_toggle;
    public TextMeshProUGUI RollTextField;
    public TextMeshProUGUI GeoTextField1;
    public TextMeshProUGUI GeoTextField2;
    public TextMeshProUGUI StateTextField;
    public TextMeshProUGUI ActionChoice1TextField;
    public TextMeshProUGUI ActionChoice2TextField;
    public TextMeshProUGUI ActionChoice3TextField;
    public TextMeshProUGUI[] OwnershipTextField;
    public TextMeshProUGUI[] HotelsTextField;
    public GameObject[] Views;
    public RegionData RegionDataObject;
    public GameLogic GameLogicObject;
    public int D6result;
    #endregion
    #region Unity functions
    // Start is called before the first frame update
    void Start()
    {
        RegionDataObject = transform.GetComponent<RegionData>();


        //RegionDataObject.regionArray[0].HotelName
        GeoTextField1.text = " " + GameLogicObject.Geo1;
        GeoTextField2.text = " " + GameLogicObject.Geo2;
    }



    // Update is called once per frame
    void Update()
    {

    }
    #endregion

    #region RollD6
    int shuffleTimes, shuffleCounter;
    public void RollDice6()
    {
        
        if (GameLogicObject.RollDone == true){return;}
      //  ToggleView(0);
        shuffleTimes = Random.Range(5, 20);
        shuffleCounter = 0;
        Invoke("RollD6Repeating", 0.1f);
    }
    
    private void RollD6Repeating()
    {
        int rollResult = Random.Range(1, 7);
        RollTextField.text = rollResult.ToString();
        shuffleCounter++;
        if (shuffleCounter < shuffleTimes)
        {
            Invoke("RollD6Repeating", 0.1f);
        }
        else
        {
            GameLogicObject.DiceRolled();
            GameLogicObject.D6Result = rollResult;
            GameLogicObject.RollDone = true;
        }
    }
    #endregion

    #region Roll Build Dice
    public void RollBuild()
    {
        if (GameLogicObject.RollBuildDone == true) { return; }
        //  ToggleView(0);
        shuffleTimes = Random.Range(5, 20);
        shuffleCounter = 0;
        Invoke("RollBuildRepeating", 0.1f);
    }

    private void RollBuildRepeating()
    {
        int rollResult = Random.Range(1, 7);
        BuildDiceFaces BuildDiceResult;
        if (rollResult <= 3)             //success ('GREEN')
        {
            BuildDiceResult = BuildDiceFaces.Success;
        }else if(rollResult <= 4)        // fail ('RED')
        {
            BuildDiceResult = BuildDiceFaces.Fail;
        }else if (rollResult <= 5)       //double (2)
        {
            BuildDiceResult = BuildDiceFaces.Double;
        }else                            // free (H)
        {
            BuildDiceResult = BuildDiceFaces.Free;
        }

        if (BuildDiceResult == BuildDiceFaces.Success)
        {
            RollTextField.text = "<color=#005500>Success</color>";
        }else if (BuildDiceResult == BuildDiceFaces.Fail)
        {
            RollTextField.text = "<color=red>Fail</color>";
        }else if (BuildDiceResult == BuildDiceFaces.Double)
        {
            RollTextField.text = "<color=purple>x2 Cost</color>";
        }else if (BuildDiceResult == BuildDiceFaces.Free)
        {
            RollTextField.text = "<color=green>Free Build</color>";
        }

        shuffleCounter++;
        if (shuffleCounter < shuffleTimes)
        {
            Invoke("RollBuildRepeating", 0.1f);
        }
        else
        {
            GameLogicObject.BuildDiceRolled(BuildDiceResult);
            GameLogicObject.RollBuildDone = true;
        }
    }
    #endregion

    #region Hotel Info Field Logic

    #endregion

    #region View Logic
    /* public void ResetView()
     {
         ToggleView(-1);
     }
     public void ToggleView(int index)
     {
         for (int i = 0; i < Views.Length; i++)
         {
             if (index == i)
             {
                 Views[i].SetActive(true);
             }
             else
             {
                 Views[i].SetActive(false);
             }
         }
     }*/
    #endregion

    #region Money Control
    public void GainMoney(int PlayerIndex)
    {
        int gainMoneyReward = 2000;
        if (PlayerIndex == 0)
        {
            GameLogicObject.Geo1 += gainMoneyReward;
            GeoTextField1.text = " " + GameLogicObject.Geo1;
        }
        else if(PlayerIndex == 1)
        {
            GameLogicObject.Geo2 += gainMoneyReward;
            GeoTextField2.text = " " + GameLogicObject.Geo2;
        }

    }
    public void SpentMoney(int PlayerIndex, int cost)
    {
        if (PlayerIndex == 0 && GameLogicObject.Geo1!>0)
        {
            GameLogicObject.Geo1 -= cost;
            GeoTextField1.text = " " + GameLogicObject.Geo1;
        }
        else if (PlayerIndex == 1 && GameLogicObject.Geo2>0)
        {
            GameLogicObject.Geo2 -= cost;
            GeoTextField2.text = " " + GameLogicObject.Geo2;
        }
        if (GameLogicObject.Geo1 <= 0)
        {
            DisplayActionReset();
            HintPlayer("GAME OVER! Hornet won.");
            Invoke("WaitASec", 10f);
            GameLogicObject.gameover = true;
        }
        if (GameLogicObject.Geo2 <= 0)
        {
            DisplayActionReset();
            HintPlayer("GAME OVER! The Knight won.");
            Invoke("WaitASec", 10f);
            GameLogicObject.gameover = true;
        }
    }
    #endregion
    #region Current State Display
    public void TogglePlayers(int PlayerIndex)
    {
        for (int i = 0; i < 2; i++)
        {
            if (PlayerIndex == 0)
            {
                Player1_toggle.isOn = true;
            }
            else
            {
                Player2_toggle.isOn = true;

            }
        }
    }

    public void DisplayAction(string msg, string c1, string c2, string c3)
    {
        StateTextField.text = msg;
        ActionChoice1TextField.text = c1;
        ActionChoice2TextField.text = c2;
        ActionChoice3TextField.text = c3;
    }
    public void DisplayActionReset()
    {
        StateTextField.text = null;
        ActionChoice1TextField.text = null;
        ActionChoice2TextField.text = null;
        ActionChoice3TextField.text = null;
    }

    public void HintPlayer(string msg)
    {
        StateTextField.text = msg;
        Invoke("WaitASec", 3f);
    }
    #endregion
    #region Score Display
    public void UpdateOwnershipScore(int area, int playerID)
    {
        OwnershipTextField[area-1].text = " " + playerID;
    }
    
    public void UpdateHotelScore(int area, int hotelCount)
    {
        HotelsTextField[area].text = " " + hotelCount;
    }
    #endregion
    public void PlayerChoseBuild()
    {
        bool availableArea = GameLogicObject.FindAvailablePlots();
        if (availableArea)
        {
            string Q = "Select in which area you want to build.";
            DisplayAction(Q, null, null, "None");
           // GameLogicObject.DisplayBuildHotelsButtons();
        }
        else
        {
            DisplayAction("You don't own an area yet.", null, null, "Ok");
        }
    }

    public void PlayerChoseGates(int area)
    {
        //display available entrances
        bool hashotel = GameLogicObject.FindAvailableGates(area);
        if (hashotel)
        {
            Debug.Log("found available gates");
            string Q = "Select on which node you want to build a gate.";
            DisplayAction(Q, null, null, "None");
            GameLogicObject.waitToChooseGate = true;
        }
        else
        {
            DisplayAction("You don't own a Hotel yet.", null, null, "Ok");
        }
    }    
    public void WaitASec()
    {

    }
}

public enum BuildDiceFaces
{
    Success, Fail, Double, Free
} 