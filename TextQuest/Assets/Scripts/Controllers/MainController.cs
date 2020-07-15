using UnityEngine;

public class MainController : DontDestroySingleton<MainController>
{
    private bool _pause;
    public bool Pause => _pause;
    private bool _canTap = true;
    public bool CanTap => _canTap;
    private bool _miniGame;
    public bool MiniGame;
}
