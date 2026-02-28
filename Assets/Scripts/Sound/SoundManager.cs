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
}
