using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Figure Configuration", menuName = "Scriptable Objects/Figures"), Serializable]
public class SOFigure : ScriptableObject
{
    public string Name;

    public Sprite Image;

    public int[] WintreakPrize = new int[GeneralConfigurations.MachineColumns];

    public int GetAccumulatedValue()
    {
        int returnedvalue = 0;

        foreach (int i in WintreakPrize) { returnedvalue += i; }
        return returnedvalue;
    }
}
