using UnityEngine;

public class QuestionButton : MyButton
{
    [SerializeField] private int myNumber;

    private void OnMouseDown()
    {
        FrameController.Instance.ChooseButton(myNumber);
    }
}
