using UnityEngine;

public class MyButton : MonoBehaviour
{
    [SerializeField] private int myNumber;

    private void OnMouseDown()
    {
        GameController.Instance.ChooseButton(myNumber);
    }
}
