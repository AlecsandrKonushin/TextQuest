using UnityEngine;

public class AlertMessageButton : MyButton
{
    private void OnMouseDown()
    {
        FrameController frameCon = FrameController.Instance;
        if (frameCon.CurrentFrame is FrameAlertMessage)
        {
            UiController.Instance.ClickButtonMessage();
            frameCon.NextPoint();
        }
    }
}
