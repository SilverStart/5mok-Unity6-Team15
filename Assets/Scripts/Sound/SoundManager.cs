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

    [SerializeField, Range(0f, 1f)]
    private float masterVolume = 1;
    [SerializeField, Range(0f, 1f)]
    private float bgmVolume = 1;
    [SerializeField, Range(0f, 1f)]
    private float sfxVolume = 1;

    public float MasterVolume
    {
        get => masterVolume;
        set
        {
            masterVolume = Mathf.Clamp01(value);

            bgmSource.volume = bgmVolume * masterVolume;
            sfxSource.volume = sfxVolume * masterVolume;
        }
    }

    public float SFXVolume
    {
        get => sfxVolume;
        set
        {
            sfxVolume = Mathf.Clamp01(value);

            sfxSource.volume = sfxVolume * masterVolume;
        }
    }

    public float BGMVolume
    {
        get => bgmVolume;
        set
        {
            bgmVolume = Mathf.Clamp01(value);

            bgmSource.volume = bgmVolume * masterVolume;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        bgmSource.volume = masterVolume * bgmVolume;
        sfxSource.volume = masterVolume * sfxVolume;
    }

    /// <summary>
    /// BGM ��� �Լ�
    /// </summary>
    /// <param name="clip">��� ��ų BGM Clip</param>
    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource.isPlaying)
            bgmSource.Stop();

        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void PlayBGM(string clipName)
    {
        var bgmClip = LoadClip(clipName, "BGM");

        if (bgmClip == null)
            return;

        PlayBGM(bgmClip);
    }

    /// <summary>
    /// ȿ���� ��� �Լ�
    /// </summary>
    /// <param name="clip">�����ų ȿ����</param>
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlaySFX(string clipName)
    {
        var sfxClip = LoadClip(clipName, "SFX");

        if (sfxClip == null) return;

        PlaySFX(sfxClip);
    }

    public void StopBGM()
    {
        if (bgmSource.isPlaying)
            bgmSource.Stop();
    }

    /// <summary>
    /// AudioClip Load �Լ�
    /// </summary>
    /// <param name="clipName">�ε��� Clip�� �̸�</param>
    /// <param name="soundType">�ε��� Clip�� ���� Ÿ��</param>
    /// <returns></returns>
    public AudioClip LoadClip(string clipName, string soundType)
    {
        string path = $"Sounds/{soundType}/{clipName}";

        return Resources.Load<AudioClip>(path);
    }

    public bool IsMute(SoundType soundType)
    {
        switch (soundType)
        {
            case SoundType.BGM:
                return bgmSource.mute;
            case SoundType.SFX:
                return sfxSource.mute;
        }

        return false;
    }

    public void MuteBGM(bool isMute)
    {
        bgmSource.mute = isMute;
    }

    public void MuteSFX(bool isMute)
    {
        sfxSource.mute = isMute;
    }

    public void SetMasterVolume(float value)
    {
        MasterVolume = value;
    }

    public void SetBGMVolume(float value)
    {
        BGMVolume = value;
    }

    public void SetSFXVolume(float value)
    {
        SFXVolume = value;
    }
}
