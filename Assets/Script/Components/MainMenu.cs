using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _play;
    [SerializeField] private GameObject _continue;
    [SerializeField] private GameObject _settings;
    [SerializeField] private GameObject _exit;
    private Button _continueButton;
    public EventHandler<ButtonClickEventArgs> onMainMenuClick;
    private void Start()
    {
        _continueButton = _continue.GetComponent<Button>();
        UpdateContinueButtonState();
    }
    private void UpdateContinueButtonState()
    {
        bool hasSaveData = SaveManager.Instance.HasSavedGame();
        if (_continueButton != null)
        {
            _continueButton.interactable = hasSaveData;
            var colors = _continueButton.colors;
            colors.normalColor = hasSaveData ? Color.white : Color.gray;
            colors.highlightedColor = hasSaveData ? new Color(0.9f, 0.9f, 0.9f) : Color.gray;
            colors.pressedColor = hasSaveData ? new Color(0.8f, 0.8f, 0.8f) : Color.gray;
            _continueButton.colors = colors;
        }
    }
    public void RefreshContinueButtonState()
    {
        UpdateContinueButtonState();
    }
    public void onButtonClick(int id)
    {
        if (id == 1)
        {
            if (!SaveManager.Instance.HasSavedGame())
            {
                return;
            }
        }
        string buttonName = "";
        switch (id)
        {
            case 0:
                buttonName = "Play";
                break;
            case 1:
                buttonName = "Continue";
                break;
            case 2:
                buttonName = "Settings";
                break;
            case 3:
                buttonName = "Exit";
                break;
        }
        onMainMenuClick?.Invoke(this, new ButtonClickEventArgs(buttonName));
    }
}