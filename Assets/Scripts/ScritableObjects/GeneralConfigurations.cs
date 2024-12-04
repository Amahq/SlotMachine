using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New General Configuration", menuName = "Scriptable Objects/General Configuration"), Serializable]
public class GeneralConfigurations : ScriptableObject
{
    public const int MachineRows = 3;
    public const int MachineColumns = 5;
    public const int ColumnItemLength = 15;
    public const int RollCreditCost = 0;
    
}
