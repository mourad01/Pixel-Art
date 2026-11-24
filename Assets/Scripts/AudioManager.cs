using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioClip m_clickClip;
    [SerializeField] AudioClip m_bombClip;
    [SerializeField] AudioClip m_wandClip;
    [SerializeField] AudioClip m_completeClip;
    [SerializeField] AudioClip m_victoryClip;
    [SerializeField] AudioClip m_colorClip;

    public AudioSource m_audioTrackSrc;
    public bool isMuted;

    public static AudioManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Object.DontDestroyOnLoad(gameObject);
            m_audioTrackSrc = base.GetComponent<AudioSource>();

            isMuted = !AppData.SoundsEnabled;
            m_audioTrackSrc.mute = isMuted;
        }
        else if (this != Instance)
        {
            Object.Destroy(gameObject);
        }
    }

    public void RefreshSettings()
    {
        isMuted = !AppData.SoundsEnabled;
        m_audioTrackSrc.mute = isMuted;
    }

    public void PlayClick()
    {
        if (!isMuted)
        {
            PlayClip(m_clickClip);
        }
    }

    public void PlayBomb()
    {
        if (!isMuted)
        {
            PlayClip(m_bombClip);
        }
    }

    public void PlayWand()
    {
        if (!isMuted)
        {
            PlayClip(m_wandClip);
        }
    }

    public void PlayCompleteColor()
    {
        if (!isMuted)
        {
            PlayClip(m_completeClip);
        }
    }

    public void PlayVictory()
    {
        if (!isMuted)
        {
            PlayClip(m_victoryClip);
        }
    }
    public void PlayColor()
    {
        if (!isMuted)
        {
            PlayClip(m_colorClip);
        }
    }
    public void PlayClip(AudioClip clip)
    {
        m_audioTrackSrc.PlayOneShot(clip);
    }
}


