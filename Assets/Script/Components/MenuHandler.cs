using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
public class MenuHandler : MonoBehaviour
{
    [SerializeField] private GameObject _gameMenuObj;
    [SerializeField] private GameObject _mainMenuObj;
    [SerializeField] private GameObject _resultMenuObj;
    private GameMenu _gameMenu;
    private MainMenu _mainMenu;
    private ResultMenu _resultMenu;

    private ViewManager _viewManager;
    private PlayerData _currentResultData;
    public UnityEvent onGameMenuClick;
    public UnityEvent onResultMenuClick;
    public void init()
    {
        _gameMenu = _gameMenuObj.GetComponent<GameMenu>();
        _mainMenu = _mainMenuObj.GetComponent<MainMenu>();
        _resultMenu = _resultMenuObj.GetComponent<ResultMenu>();

        initEventListener();
        showMainMenu();
    }

    public void SetViewManager(ViewManager viewManager)
    {
        _viewManager = viewManager;
    }
    void initEventListener()
    {
        _mainMenu.onMainMenuClick += mainMenuEventHandler;
        _gameMenu.onGameMenuClick += gameMenuEventHandler;
        _resultMenu.onResultMenuClick += resultMenuEventHandler;
    }
    private void mainMenuEventHandler(object sender, ButtonClickEventArgs args)
    {
        Debug.Log("Main Menu Event: " + args.ButtonName);

        switch (args.ButtonName)
        {
            case "Play":
                showGameMenu();
                break;
            case "Continue":
                if (SaveManager.Instance.HasSavedGame())
                {
                    _viewManager?.LoadSavedGame();
                }
                else
                {
                    Debug.LogWarning("Continue clicked but no save data exists!");
                    _mainMenu.RefreshContinueButtonState();
                }
                break;
            case "Settings":
                break;
            case "Exit":
                Application.Quit();
                break;
        }
    }
    private void gameMenuEventHandler(object sender, ButtonClickEventArgs args)
    {
        Debug.Log("Game Menu Event: " + args.ButtonName);

        switch (args.ButtonName)
        {
            case "L1":
            case "L2":
            case "L3":
            case "L4":
            case "L5":
                hideAllMenus();
                _viewManager?.LoadGameWithDifficulty(args.ButtonName);
                break;
            case "Back":
                showMainMenu();
                break;
        }
    }
    private void resultMenuEventHandler(object sender, ButtonClickEventArgs args)
    {
        Debug.Log("Result Menu Event: " + args.ButtonName);

        switch (args.ButtonName)
        {
            case "NextGame":
                showGameMenu();
                break;
            case "MainMenu":
                showMainMenu();
                break;
        }
    }
    public void ShowGameOverResult(PlayerData playerData)
    {
        _currentResultData = playerData;
        showResultMenu();
        _resultMenu.ShowGameOver(playerData);
        Debug.Log($"Showing Game Over result with score: {playerData.Score}");
    }
    public void ShowGameWonResult(PlayerData playerData)
    {
        _currentResultData = playerData;
        showResultMenu();
        _resultMenu.ShowGameWon(playerData);
        Debug.Log($"Showing Game Won result with score: {playerData.Score}");
    }
    public void showGameMenu()
    {
        _gameMenu.gameObject.SetActive(true);
        _mainMenu.gameObject.SetActive(false);
        _resultMenu.gameObject.SetActive(false);
    }
    public void showMainMenu()
    {
        _gameMenu.gameObject.SetActive(false);
        _mainMenu.gameObject.SetActive(true);
        _resultMenu.gameObject.SetActive(false);

        if (_mainMenu != null)
        {
            _mainMenu.RefreshContinueButtonState();
        }
    }
    public void showResultMenu()
    {
        _gameMenu.gameObject.SetActive(false);
        _mainMenu.gameObject.SetActive(false);
        _resultMenu.gameObject.SetActive(true);
    }
    public void hideAllMenus()
    {
        _gameMenu.gameObject.SetActive(false);
        _mainMenu.gameObject.SetActive(false);
        _resultMenu.gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        if (_mainMenu != null)
            _mainMenu.onMainMenuClick -= mainMenuEventHandler;
        if (_gameMenu != null)
            _gameMenu.onGameMenuClick -= gameMenuEventHandler;
        if (_resultMenu != null)
            _resultMenu.onResultMenuClick -= resultMenuEventHandler;
    }
}