using System.Collections;
using UnityEngine;

public class TapController : Singleton<TapController>
{
    public bool CanTap = true;
    private bool _timeCanTap = true;
    private float _timeBetweenTap = .1f;
    
    private void OnMouseDown()
    {
        if (MainController.Instance.CanTap && _timeCanTap && CanTap)
            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(CoWaitTap());
                GameController.Instance.NextPoint();
            }
    }

    private IEnumerator CoWaitTap()
    {
        _timeCanTap = false;
        yield return new WaitForSeconds(_timeBetweenTap);
        _timeCanTap = true;
    }
}
