using Unity.VisualScripting;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Win Condition", menuName = "Scriptable Objects/Win Conditions"), Serializable]
public class SOWin : ScriptableObject
{
    [Range(1, GeneralConfigurations.MachineRows)]
    public int[] Cols = new int[GeneralConfigurations.MachineColumns];

    public int Winstreak = 0;
    public SOFigure WinningFigure = null;

    public SOWin()
    {
        for (int i = 0; i < Cols.Length; i++)
        {
            Cols[i] = 1;
        }
    }

    public void ResetCondition()
    {
        Winstreak = 0;
        WinningFigure = null;
    }
}
