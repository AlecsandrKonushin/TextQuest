using UnityEngine;

public class MainController : DontDestroySingleton<MainController>
{
    private bool _pause;
    //public bool Pause => _pause;
    private bool _canTap = false;
    public bool CanTap => _canTap;
    private bool _miniGame;
    public bool MiniGame;

    private void Start()
    {
        PlayGame();
    }

    public void PlayGame()
    {
        _pause = false;
        _canTap = true;
    }

    public void PauseGame()
    {
        _pause = true;
        _canTap = false;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
