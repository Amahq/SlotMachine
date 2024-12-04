using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class UISlot : MonoBehaviour
{
    private SOFigure figure;
    public Image displayImage;

    public void LoadFigure(SOFigure figure, RectTransform rectTrans)
    {
        //Debug.Log("Loading UI Figure");
        this.figure = figure;
        displayImage.sprite = figure.Image;
        if (figure.Image == null )
        {
            displayImage.color = new Color(0,0,0,0);
        }
        gameObject.GetComponent<RectTransform>().localPosition = rectTrans.localPosition;
    }

    public void SetNewPosition(RectTransform newPosition, float duration)
    {

        gameObject.GetComponent<RectTransform>().LeanMoveLocal(newPosition.localPosition, duration);
    }
}
