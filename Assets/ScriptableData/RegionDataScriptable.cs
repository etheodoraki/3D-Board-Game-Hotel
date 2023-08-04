using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRegionDataObject", menuName = "Tutorial/RegionData")]
public class RegionDataScriptable : ScriptableObject
{
    public string HotelName;
    public int posx;
    public int posz;
    public int AreaPrice;
    public int[] HotelPrices;
    public int[] PricePerNight;
}
