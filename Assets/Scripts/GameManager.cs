using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Mono.Cecil.Cil;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public delegate void GameNotification();
public delegate void RollStartNotifitacion(float duration);
public delegate void GameInitialization(SOWin[] sOWins, SORoller[] sORoller, SOFigure[] sOFigure);

public class GameManager : MonoBehaviour
{
    public static event RollStartNotifitacion rollStarted;

    public static GameManager Instance { get; private set; }

    private static int ColumnLength = GeneralConfigurations.ColumnItemLength;
    private static int RowAmount = GeneralConfigurations.MachineRows;
    private static int ColumnAmount = GeneralConfigurations.MachineColumns;
    private static int RollCost = GeneralConfigurations.RollCreditCost;

    public int PlayerCredits = 0;

    public SOWin[] winPatterns = null;
    public SORoller[] Rollers = null;
    public SOFigure[] Figures = null;
    public SOWin[] winners = null;

    public int[][] Cols = new int[ColumnAmount][];

    private int[] WinningFruits = new int[ColumnLength];

    public SOFigure GetFigureByName(string name)
    {
        foreach (SOFigure fig in Figures)
        {
            if (fig.Name == name) return fig;
        }
        return null;
    }

    private void CheckWinCondition()
    {
        foreach (SOWin WP in winPatterns)
        {
            int winstreak = 0;
            int figurePosition = 0;
            SOFigure initialFigure = null;
            SOFigure currentFigure = null;
            for (int i = 0; i < WP.Cols.Length; i++)
            {

                if (i == 0)
                {
                    figurePosition = Cols[i][WP.Cols[i] - 1];
                    initialFigure = Rollers[i].GetFigure(figurePosition - 1);
                }
                figurePosition = Cols[i][WP.Cols[i] - 1];
                currentFigure = Rollers[i].GetFigure(figurePosition - 1);
                //Debug.Log("Column " + (i + 1) + " - Row " + WP.Cols[i] + "(" + Cols[i][WP.Cols[i] - 1] + ") = " + fruit);
                if(currentFigure.name == initialFigure.name)
                {
                    winstreak++;
                    if (winstreak > 1)
                    {
                        //Debug.Log("(" + WP.name + ")Column " + (i + 1) + " - Row " + WP.Cols[i] + "(" + Cols[i][WP.Cols[i] - 1] + ") = " + initialFigure.name + " - winstreak (" + winstreak + ")");
                    }
                }
                else
                {
                    break;
                }
            }
            if ( winstreak > 1)
            {
                //Debug.Log(WP.name + " - " + winstreak);
                WP.WinningFigure = initialFigure;
                WP.Winstreak = winstreak;
            }
            if (WP.Winstreak < winstreak)
            {
                WP.Winstreak = winstreak;
            }
        }
    }



    private void RollColumnsTestData(int column1, int column2, int column3, int column4, int column5)
    {
        Cols[0] = FillColAdjacents(column1);
        Cols[1] = FillColAdjacents(column2);
        Cols[2] = FillColAdjacents(column3);
        Cols[3] = FillColAdjacents(column4);
        Cols[4] = FillColAdjacents(column5);
    }

    private void RollColumnsTestData()
    {
        Cols[0] = FillColAdjacents(11);
        Cols[1] = FillColAdjacents(7);
        Cols[2] = FillColAdjacents(10);
        Cols[3] = FillColAdjacents(2);
        Cols[4] = FillColAdjacents(9);
    }

    private void RollColumns()
    {
        for (int i = 0;i < Cols.Length; i++)
        {
            Cols[i] = FillColAdjacents(Random.Range(1, ColumnLength));
            //Debug.Log("Col" + (i + 1) +" (" + Cols[i][0] + "," + Cols[i][1] + "," + Cols[i][2] + ")");
        }
        DebugRollShow();
    }

    private void DebugRollShow()
    {
        string debugtext = "";
        int figurePosition = 0;
        SOFigure figure = null;
        for (int row = 1; row <= RowAmount; row++)
        {
            debugtext = "Row" + (row) + "(" ;
            for (int i = 0; i < Cols.Length; i++)
            {
                figurePosition = Cols[i][row - 1];
                //Debug.Log("figurePosition = " + figurePosition + " - Col (" + Cols[i] + ") - Row (" + row + ")");
                figure = Rollers[i].GetFigure(figurePosition - 1);
                //debugtext += Cols[i][row - 1];
                debugtext += "(" + figurePosition + ")" + figure.name;
                if (i + 1 < Cols.Length)
                {
                    debugtext += " - ";
                }
                //Debug.Log("Row" + (row + 1) + " (" + Cols[i][0] + "," + Cols[i][1] + "," + Cols[i][2] + ")");
            }
            debugtext = debugtext + ")";
            Debug.Log(debugtext);
        }
    }

    private int[] FillColAdjacents(int Value)
    {
        int[] returnedvalue = new int[RowAmount];
        returnedvalue[0] = Value;
        int controlvalue = Value + RowAmount;
        int newValue = 0;

        for (int i = 1; i < RowAmount; i++)
        {
            newValue = returnedvalue[i - 1] + 1;
            if (newValue > ColumnLength)
            {
                newValue = 1;
            }
            returnedvalue[i] = newValue;
            //Debug.Log(newValue);
        }
        return returnedvalue;
    }

    private void ProcessWinConditions()
    {
        winners = OverlappingPrizesFilter();
        foreach (SOWin win in winners)
        {
            if (win.WinningFigure != null)
            {
                //Debug.Log(win.name + " - Figure (" + win.WinningFigure.name + ") - WinStreak (" + win.Winstreak + ") - Credits Earned (" + win.WinningFigure.WintreakPrize[win.Winstreak - 1] + ")");
                PlayerCredits += win.WinningFigure.WintreakPrize[win.Winstreak - 1];
            }
        }

        Debug.Log("Player New Credit Balance = " + PlayerCredits);
    }

    private int GetOverlappingLevel(SOWin condition1, SOWin condition2)
    {
        int returnedvalue = 0;
        int columnAmount = GeneralConfigurations.MachineColumns;

        for (int i = 0; i < columnAmount; i++)
        {
            if (condition1.Cols[i] == condition2.Cols[i])
            {
                returnedvalue ++;
            }
            else
            {
                break;
            }
        }

        return returnedvalue;
    }

    private SOWin[] OverlappingPrizesFilter()
    {
        List<SOWin> tempconditions = winPatterns.ToList();
        List<SOWin> finalWinners = new List<SOWin>();
        int overlap = 0;
        SOWin[] returnedvalue = null;
        SOWin overlapped = null;
        //return returnedvalue;
        foreach (SOWin win in winPatterns)
        {
            //tempconditions.Remove(win);
            overlap = 0;
            if (win.WinningFigure != null)
            {
                foreach (SOWin win2 in tempconditions)
                {
                    if (win2.WinningFigure == win.WinningFigure && win.name != win2.name)
                    {
                        Debug.Log("Comparing (" + win.name + ") with (" + win2.name + ")");
                        overlap = GetOverlappingLevel(win, win2 );
                        overlapped = win2;
                    }
                }
                if (win.Winstreak >= overlap)
                {
                    finalWinners.Add(win);
                }
            }
        }
        returnedvalue = finalWinners.ToArray();
        return returnedvalue;
    }

    private void ResetGameState()
    {

        foreach (SOWin win in winPatterns)
        {
            win.ResetCondition();
        }

        for (int i = 0;i < WinningFruits.Length;i++)
        {
            WinningFruits[i] = 0;
        }
    }

    public void Roll()
    {
        if ( PlayerCredits >= RollCost)
        {
            float rollDuration = (float)Random.Range(2f, 5f);
            ResetGameState();
            RollColumns();
            //RollColumnsTestData(6,2,3,8,6);
            CheckWinCondition();
            ProcessWinConditions();
            rollStarted?.Invoke(rollDuration);
        }
        else
        {
            Debug.Log("Insufficiente Credits");
        }
    }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
}
