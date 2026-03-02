using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    private Button settingButton;

    [SerializeField] private GameObject settingPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        settingButton = GetComponent<Button>();

        settingButton.onClick.AddListener(() =>
        {
            Instantiate(settingPrefab);
        });
    }
}
