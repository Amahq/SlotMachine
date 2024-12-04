 using System.Collections;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

public delegate void UINotification();
public delegate void UIRollStarted(float duration);

public class UIManager : MonoBehaviour
{
    public UIRoller[] UIRollers;

    private SOWin[] winPatterns = null;
    public GameObject WinPatterMarkers;
    public UnityEngine.UI.Button SpinBtn;
    public TMPro.TMP_Text credits;
    public TMPro.TMP_Text creditWin;
    public GameObject SlotMachine;

    private void ResetWinPatterns()
    {
        foreach(Transform t in WinPatterMarkers.transform)
        {
            t.gameObject.SetActive(false);
        }
        creditWin.text = "";
    }

    IEnumerator ShowWinningSequence()
    {

        foreach (SOWin win in winPatterns)
        {
            string winPatternName = "";
            for (int i = 0;i < win.Cols.Length;i++)
            {
                string tempname = (i + 1).ToString() + win.Cols[i].ToString();
                GameObject winpattern = WinPatterMarkers.transform.Find(tempname).gameObject;
                winpattern.SetActive(true);
                if (i < win.Winstreak)
                {
                    winpattern.GetComponent<UnityEngine.UI.Image>().color = Color.green;
                }
                else
                {
                    winpattern.GetComponent<UnityEngine.UI.Image>().color = Color.red;
                }
                winPatternName += tempname + " - ";
            }
            Debug.Log("Winpattern = " + winPatternName);
            Debug.Log(win.name + " - Figure (" + win.WinningFigure.name + ") - WinStreak (" + win.Winstreak + ") - Credits Earned (" + win.WinningFigure.WintreakPrize[win.Winstreak - 1] + ")");
            int newCredits = win.WinningFigure.WintreakPrize[win.Winstreak - 1];
            int previouscredits = System.Convert.ToInt32(credits.text);
            int totalCredits = newCredits + previouscredits;
            creditWin.text = " +" + newCredits.ToString();
            creditWin.rectTransform.LeanMove(SlotMachine.transform.localPosition, 0f);
            Vector2 originalSize = creditWin.rectTransform.sizeDelta;
            creditWin.rectTransform.LeanSize(new Vector2(originalSize.x * 5, originalSize.y *5),0.1f).setOnComplete(
                delegate ()
                {
                    creditWin.rectTransform.LeanSize(originalSize, 0.3f);
                    creditWin.rectTransform.LeanMove(credits.rectTransform.localPosition, 0.3f);
                });
            yield return new WaitForSeconds(1);
            credits.text = totalCredits.ToString();
            ResetWinPatterns();
        }
        SpinBtn.interactable = true;
        yield return null;
    }

    IEnumerator StartRollerSequence(float duration)
    {
        float delayTime = 0;
        float timer = 0;
        for (int i = 0; i < UIRollers.Length; i++)
        {
            delayTime = Random.Range(0.05f, 0.2f);
            UIRollers[i].StartRoller(duration, GameManager.Instance.Cols[i][0]);
            timer += delayTime + Time.deltaTime;
            yield return new WaitForSeconds(delayTime);
        }
        
        yield return new WaitForSeconds(duration - timer);

        foreach (UIRoller uIRoller in UIRollers)
        {
            delayTime = Random.Range(0.2f, 0.5f);
            uIRoller.StopRoller();
            while (uIRoller.isRolling)
            {
                yield return new WaitForSeconds(delayTime);
            }
        }
        StartCoroutine("ShowWinningSequence");

        yield return null;
    }

    private void LoadRollerVisuals()
    {
        //Debug.Log("Loading the Rollers from UI Manager");
        for (int i = 0; i < UIRollers.Length; i++)
        {
            UIRollers[i].LoadRoller(GameManager.Instance.Rollers[i]);
        }
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.rollStarted += OnRollStartNotification;
        LoadRollerVisuals();
    }

    public void OnRollStartNotification(float duration)
    {
        winPatterns = GameManager.Instance.winners;
        StartCoroutine("StartRollerSequence", duration);
    }


    public void Roll()
    {

        GameManager.Instance.Roll();
    }
}
