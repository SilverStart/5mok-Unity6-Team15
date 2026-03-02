using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SoundPopupUI : PanelController
{
    private SoundManager sm;

    [SerializeField] private TextMeshProUGUI masterVolumeText;

    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    [SerializeField] private ToggleSlider muteBGM;
    [SerializeField] private ToggleSlider muteSFX;
    [SerializeField] private ToggleSlider muteAll;

    [SerializeField] private Button closeButton;

    private void Awake()
    {
        sm = SoundManager.Instance;

        transform.SetParent(GameObject.FindFirstObjectByType<Canvas>().transform);
        GetComponent<RectTransform>().offsetMax = Vector2.zero;
        GetComponent<RectTransform>().offsetMin = Vector2.zero;
    }

    private void Start()
    {
        masterSlider.value = sm.MasterVolume;
        bgmSlider.value = sm.BGMVolume;
        sfxSlider.value = sm.SFXVolume;

        masterVolumeText.text = $"{sm.MasterVolume * 100:F0}%";

        muteBGM.Toggle(!sm.IsMute(SoundType.BGM));
        muteSFX.Toggle(!sm.IsMute(SoundType.SFX));
        muteAll.Toggle(sm.IsMute(SoundType.BGM) && sm.IsMute(SoundType.SFX));

        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmSlider.onValueChanged.AddListener(sm.SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(sm.SetSFXVolume);

        muteBGM.Setup(() =>
        {
            sm.MuteBGM(!muteBGM.IsOn);
        });
        muteSFX.Setup(() =>
        {
            sm.MuteSFX(!muteSFX.IsOn);
        });

        muteAll.Setup(() =>
        {
            sm.MuteBGM(!muteAll.IsOn);
            sm.MuteSFX(!muteAll.IsOn);

            muteBGM.OnClickToggle(!muteAll.IsOn);
            muteSFX.OnClickToggle(!muteAll.IsOn);
        });

        closeButton.onClick.AddListener(() => Destroy(this.gameObject));
    }

    private void SetMasterVolume(float value)
    {
        SoundManager.Instance.SetMasterVolume(value);

        masterVolumeText.text = $"{value * 100:F0}%";
    }
}
