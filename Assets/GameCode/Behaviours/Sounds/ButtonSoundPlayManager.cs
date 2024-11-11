using UnityEngine;

public class ButtonSoundPlayManager : MonoBehaviour
{
    public static ButtonSoundPlayManager Instance;

    [SerializeField] private AudioClip defaultButtonClip;
    [SerializeField] private AudioClip lockedButtonClip;
    [SerializeField] private AudioSource source;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if(source != null)
        {
            source.clip = defaultButtonClip;
        }
    }

    public void PlayDefaultClip()
    {
        if(source != null)
        {
            source.clip = defaultButtonClip;
            source.Play();
        }
    }

    public void PlayLockedClip()
    {
        if (source != null)
        {
            source.clip = lockedButtonClip;
            source.Play();
        }
    }
}
