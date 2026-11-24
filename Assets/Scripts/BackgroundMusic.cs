using System.Collections;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public enum MusicTrackId
    {
        NONE = -1,
        TRACK1 = 0,
        TRACK2 = 1,
        TRACK3 = 2
    }

    public static BackgroundMusic musicMgr;

    public AudioSource m_audioTrackSrc;
    public AudioClip[] m_audioTracks;

    [HideInInspector]
    public MusicTrackId currentTrack = MusicTrackId.NONE;

    MusicTrackId pendingTrack = MusicTrackId.NONE;
    bool pendingTrackLoop;

    public bool isMuted;
    void Awake()
    {
        //keep a single music player across scenes
        if (musicMgr == null)
        {
            //sound is on by default on first launch
            if (!PlayerPrefsWrapper.Exists(AppData.s_soundsKey))
            {
                AppData.SoundsEnabled = true;
            }

            musicMgr = this;
            Object.DontDestroyOnLoad(gameObject);
            m_audioTrackSrc = base.GetComponent<AudioSource>();
            isMuted = !AppData.SoundsEnabled;
            m_audioTrackSrc.mute = isMuted;
        }
        else if (this != musicMgr)
        {
            Object.Destroy(gameObject);
        }
    }

    public void RefreshSettings()
    {
        isMuted = !AppData.SoundsEnabled;
        m_audioTrackSrc.mute = isMuted;
        if (!isMuted)
        {
            currentTrack = MusicTrackId.NONE;
            musicMgr.PlayTrack(MusicTrackId.TRACK1, true);
        }
    }

    void Start()
    {
        if (AppData.SoundsEnabled)
        {
            StartDelayTrack(MusicTrackId.TRACK1, true);
        }
    }

    static bool musicPlayFirstTime = true;
    public static void PlayMainBackground()
    {
        if (musicMgr != null)
        {
            if (musicPlayFirstTime)
            {
                musicMgr.PlayTrack(MusicTrackId.TRACK1, true);
                musicPlayFirstTime = false;
            }
            else
            {
                musicMgr.StartDelayTrack(MusicTrackId.TRACK1, true);
            }
        }
    }

    static BackgroundMusic.MusicTrackId randomMusicTrack = BackgroundMusic.MusicTrackId.TRACK3;
    public static void PlayInGameBackground()
    {
        if (musicMgr != null)
        {
            //alternate between track 2 and 3 for each game
            if (randomMusicTrack == BackgroundMusic.MusicTrackId.TRACK2)
            {
                randomMusicTrack = BackgroundMusic.MusicTrackId.TRACK3;
            }
            else
            {
                randomMusicTrack = BackgroundMusic.MusicTrackId.TRACK2;
            }
            musicMgr.StartDelayTrack(randomMusicTrack, true);
        }
    }

    public void PlayTrack(MusicTrackId trackID, bool loop)
    {
        if (!m_audioTrackSrc.mute)
        {
            if (currentTrack != trackID && pendingTrack == MusicTrackId.NONE)
            {
                m_audioTrackSrc.volume = 1f;
                m_audioTrackSrc.loop = loop;
                m_audioTrackSrc.clip = m_audioTracks[(int)trackID];
                m_audioTrackSrc.Play();
                currentTrack = trackID;
            }
        }
    }

    public void StopTrack()
    {
        if (m_audioTrackSrc.isPlaying)
        {
            m_audioTrackSrc.Stop();
        }
        currentTrack = MusicTrackId.NONE;
    }

    public void PreparePendingTrack(MusicTrackId id, bool bLoop)
    {
        pendingTrack = id;
        pendingTrackLoop = bLoop;
    }

    public void FadeOutTrack()
    {
        base.StartCoroutine("ProcessFadeOutTrack");
    }

    public void StartDelayTrack(MusicTrackId id, bool bLoop)
    {
        PreparePendingTrack(id, bLoop);
        FadeOutTrack();
    }

    IEnumerator ProcessFadeOutTrack()
    {
        if (m_audioTrackSrc.mute)
        {
            yield return null;
        }
        // fade out over about 1 second, then start the pending track
        float fadeVolume = 1f;
        while (fadeVolume > 0f)
        {
            m_audioTrackSrc.volume = fadeVolume;
            fadeVolume -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        fadeVolume = 0f;
        m_audioTrackSrc.volume = fadeVolume;
        if (pendingTrack != MusicTrackId.NONE)
        {
            var id = pendingTrack;
            pendingTrack = MusicTrackId.NONE;
            PlayTrack(id, pendingTrackLoop);
            pendingTrackLoop = false;
        }
    }
}
