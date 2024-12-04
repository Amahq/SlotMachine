using System.Collections;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class UIRoller : MonoBehaviour
{

    public class UIRollerConfig
    {
        public int RollerIndex;
        public float duration;
        public int TargetIndex;


        public UIRollerConfig() { }

        public UIRollerConfig(float duration, int targetIndex)
        {
            this.duration = duration;
            TargetIndex = targetIndex;
        }

        public UIRollerConfig(float duration, int targetIndex, int rollerIndex)
        {
            this.duration = duration;
            TargetIndex = targetIndex;
            RollerIndex = rollerIndex;
        }
    }

    private SORoller baseRoller;
    public UISlot[] slots;
    public GameObject[] SlotPositions;
    public GameObject VisualHolder;

    private int CurrentIndex = 0;
    public const int RollerOffset = 11;
    private bool KeepRolling = false;
    public bool isRolling=false;

    public void StopRoller()
    {
        KeepRolling = false;
    }

    public void LoadRoller(SORoller roller)
    {
        //Debug.Log("Loading Roller");
        baseRoller = roller;
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].LoadFigure(baseRoller.GetFigure(i), SlotPositions[i].GetComponent<RectTransform>());
        }
    }

    public void StartRoller(float duration, int targetIndex)
    {
        KeepRolling = true;
        isRolling = true;
        UIRollerConfig config = new UIRollerConfig(duration, targetIndex);
        StartCoroutine("RollerHandler", config);
    }

    IEnumerator RollerHandler(UIRollerConfig config)
    {
        float timer = 0;
        float stepSlow = 0.3f;
        float stepFast = 0.1f;
        float currentStep = stepSlow;
        float weight = 0;
        float duration = config.duration;

        while (KeepRolling)
        {
            weight = timer / (duration / 2);
            currentStep = Mathf.Lerp(stepSlow, stepFast, weight);
            AdvanceRoller(currentStep);
            timer += Time.deltaTime + currentStep;
            yield return new WaitForSeconds(currentStep);
        }
        int remainingSteps = (slots.Length - CurrentIndex+1);
        remainingSteps += RollerOffset - config.TargetIndex - 1;
        //Debug.Log("Normal Sequence Finished for Roller (" + gameObject.name + ") - CurrentIndex (" + CurrentIndex + ") - TargetIndex (" + config.TargetIndex + ") - RemainingSteps (" + remainingSteps + ")");
        for (int i = 0; i < remainingSteps; i++)
        {
            weight = timer / (duration / 2);
            currentStep = Mathf.Lerp(stepSlow, stepFast, weight);
            AdvanceRoller(currentStep);
            yield return new WaitForSeconds(currentStep);
        }
        isRolling = false;
    }



    public void AdvanceRoller(float normalTransitionTime)
    {
        int newIndex = 0;
        float transitionTime = 0.0f;
        for (int i = 0; i < slots.Length ; i++)
        {
            newIndex = i + CurrentIndex;
            if ( i + CurrentIndex > slots.Length - 1)
            {
                newIndex -= slots.Length;
            }

            if ((newIndex >= 0 && newIndex < 8) || newIndex > 13)
            {
                transitionTime = 0;
            }
            else
            {
                transitionTime = normalTransitionTime;
            }
            slots[i].SetNewPosition(SlotPositions[newIndex].GetComponent<RectTransform>(), transitionTime);
        }
        if (CurrentIndex == slots.Length - 1)
        {
            CurrentIndex = 0;
        }
        else
        {
            CurrentIndex++;
        }
    }
}
