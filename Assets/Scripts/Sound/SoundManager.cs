using UnityEngine;

public enum SoundType
{
    BGM,
    SFX
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    /// <summary>
    /// BGM 재생 함수
    /// </summary>
    /// <param name="clip">재생 시킬 BGM Clip</param>
    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource.isPlaying)
            bgmSource.Stop();
            
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    /// <summary>
    /// 효과음 재생 함수
    /// </summary>
    /// <param name="clip">재생시킬 효과음</param>
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void StopBGM()
    {
        if (bgmSource.isPlaying)
            bgmSource.Stop();
    }

    /// <summary>
    /// AudioClip Load 함수
    /// </summary>
    /// <param name="clipName">로드할 Clip의 이름</param>
    /// <param name="soundType">로드할 Clip의 사운드 타입</param>
    /// <returns></returns>
    public AudioClip LoadClip(string clipName, SoundType soundType)
    {
        string path = $"Sounds/{soundType}/{clipName}.mp3";

        return Resources.Load<AudioClip>(path);
    }
}
