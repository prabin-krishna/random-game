using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ViewManager
{
    private MenuHandler _menuHandler;
    private MainGameWindow _mainGameWindow;
    private GameObject _gameWindowObj;
    private SaveManager _saveManager;

    public void init()
    {
        _saveManager = SaveManager.Instance;
    }

    public void SetMenu(MenuHandler menuHandler)
    {
        _menuHandler = menuHandler;
        _menuHandler.SetViewManager(this);
        _menuHandler.init();
    }
    public MenuHandler GetMenuHandler()
    {
        return _menuHandler;
    }

    public void SetGameWindow(MainGameWindow mainGameWindow, GameObject gameWindowObj)
    {
        _mainGameWindow = mainGameWindow;
        _gameWindowObj = gameWindowObj;
        _mainGameWindow.SetViewManager(this);
        _mainGameWindow.init();
    }

    public void LoadGameWithDifficulty(string difficulty)
    {
        if (_mainGameWindow != null)
        {
            ShowGameWindow();

            PlayerData gameData = _saveManager.StartNewGame(difficulty);
            _mainGameWindow.SetPlayerData(gameData);
        }
    }

    public void LoadSavedGame()
    {
        if (_saveManager.HasSavedGame())
        {

            ShowGameWindow();

            if (_gameWindowObj != null)
            {
                _gameWindowObj.GetComponent<MonoBehaviour>().StartCoroutine(DelayedLoadSavedGame());
            }
        }
    }

    private System.Collections.IEnumerator DelayedLoadSavedGame()
    {
        yield return null;

        PlayerData savedData = _saveManager.LoadGame();
        _mainGameWindow.SetPlayerData(savedData);

    }

    public void ShowGameWindow()
    {
        HideGameWindow();
        if (_gameWindowObj != null)
        {
            _gameWindowObj.SetActive(true);
        }
        HideAllMenus();
    }

    public void ShowMainMenu()
    {
        HideGameWindow();
        if (_menuHandler != null)
        {
            _menuHandler.showMainMenu();

            if (_menuHandler.GetComponent<MainMenu>() != null)
            {
                _menuHandler.GetComponent<MainMenu>().RefreshContinueButtonState();
            }
        }
    }

    public void ShowGameMenu()
    {
        HideGameWindow();
        if (_menuHandler != null)
        {
            _menuHandler.showGameMenu();
        }
    }

    public void ShowResultMenu()
    {
        HideGameWindow();
        if (_menuHandler != null)
        {
            _menuHandler.showResultMenu();
        }
    }

    private void HideAllMenus()
    {
        _menuHandler?.hideAllMenus();
    }

    private void HideGameWindow()
    {
        if (_gameWindowObj != null)
        {
            _gameWindowObj.SetActive(false);
        }
    }
}