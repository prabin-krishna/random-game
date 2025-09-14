using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _menuHandlerObj;
    [SerializeField] private GameObject _mainGameWindowObj;
    
    private ViewManager _viewManager;
    private SaveManager _saveManager;
    
    void Start()
    {
        initSaveManager();
        initViewManager();
        initMenu();
        initGameWindow();
    }
    
    void initSaveManager()
    {
        _saveManager = SaveManager.Instance;
            }
    
    void initViewManager()
    {
        _viewManager = new ViewManager();
        _viewManager.init();
    }
    
    void initMenu()
    {
        MenuHandler menuHandler = _menuHandlerObj.GetComponent<MenuHandler>();
        _viewManager.SetMenu(menuHandler);
            }
    
    void initGameWindow()
    {
        MainGameWindow mainGameWindow = _mainGameWindowObj.GetComponent<MainGameWindow>();
        _viewManager.SetGameWindow(mainGameWindow, _mainGameWindowObj);
            }
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            _saveManager.AutoSave();
        }
    }
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            _saveManager.AutoSave();
        }
    }
}