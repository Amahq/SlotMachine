using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Roller Configuration", menuName = "Scriptable Objects/Roller"), Serializable]
public class SORoller : ScriptableObject
{
    public SOFigure[] Figures = new SOFigure[GeneralConfigurations.ColumnItemLength];

    public SOFigure GetFigure(int index)
    {
        return Figures[index];
    }
}
