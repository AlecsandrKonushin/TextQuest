using UnityEngine;

public class AudioController : Singleton<AudioController>
{
    [SerializeField] private AudioSource bgMain;
    [SerializeField] private AudioSource importantAudio;

    [SerializeField] private AudioClip schoolBell;

    private void PlayImportantAudio(AudioClip clip)
    {
        importantAudio.clip = clip;
        importantAudio.Play();
    }

    private void PlayBgMainAudio(AudioClip clip)
    {
        bgMain.clip = clip;
        bgMain.Play();
    }

    public void PlaySchoolBell()
    {
        PlayImportantAudio(schoolBell);
    }
}
