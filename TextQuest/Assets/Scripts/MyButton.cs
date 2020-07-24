using UnityEngine;

public class MyButton : MonoBehaviour
{
    public delegate void Click();
    public event Click ClickButton;

    [SerializeField] private int myNumber;

    private void OnMouseDown()
    {
        GameController.Instance.ChooseButton(myNumber);
    }
}
