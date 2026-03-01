using System.Collections.Generic;
using common;
using UnityEngine;
using UnityEngine.UI;

public class NavigationController : MonoBehaviour
{
    [SerializeField] private NamedButton[] Buttons;
    private Dictionary<string, Button> _buttonDict = new();

    void Awake()
    {
        foreach (var item in Buttons)
        {
            _buttonDict[item.buttonName] = item.button;
        }

        _buttonDict["Back"].onClick.AddListener(OnClickBackButton);
    }

    private void OnClickBackButton()
    {
        GameManager.Instance.ChangeToLoginScene();
    }
}
