using UnityEngine;

public class AnswerButton : MyButton
{
    [SerializeField] private int myNumber;

    private void OnMouseDown()
    {
        FrameController.Instance.ChooseButton(myNumber);
    }
}
