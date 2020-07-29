using System.Collections;
using UnityEngine;

public class TapController : Singleton<TapController>
{
    public bool CanTap = true;
    private bool _timeCanTap = true;
    private float _timeBetweenTap = .1f;

    private void Update()
    {
        if (MainController.Instance.CanTap && _timeCanTap && CanTap)
            if (Input.GetMouseButtonDown(0))
                StartCoroutine(CoWaitTap());
    }
    
    private IEnumerator CoWaitTap()
    {
        yield return null;
        if (!UiController.Instance.ClickButton)
        {
            _timeCanTap = false;
            FrameController.Instance.NextPoint();
            yield return new WaitForSeconds(_timeBetweenTap);
            _timeCanTap = true;
        }
    }
}
