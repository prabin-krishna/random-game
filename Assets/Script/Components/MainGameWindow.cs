using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MainGameWindow : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private TextAsset textAsset;
    private GameLogic _gameLogic;
    private GameView _gameView;
    private ViewManager _viewManager;
    private string _currentDifficulty;
    private bool _isInitialized = false;
    public void init()
    {
        InitializeComponents();
        LoadImageData();
        SetupEventListeners();
        _isInitialized = true;
    }
    public void SetViewManager(ViewManager vm)
    {
        _viewManager = vm;
    }
    public void SetDifficulty(string difficulty)
    {
        if (!_isInitialized)
        {
            return;
        }
        _currentDifficulty = difficulty;
        _gameLogic.StartNewGame(difficulty);
        _gameView.ShowGameWindow();
    }
    public void SetPlayerData(PlayerData data)
    {
        if (!_isInitialized)
        {
            return;
        }
        _currentDifficulty = GetDifficultyFromGridSize(data.GridColumn);
        _gameLogic.LoadGame(data);
        _gameView.ShowGameWindow();
    }
    private void InitializeComponents()
    {
        _gameLogic = new GameLogic();
        _gameView = GetComponent<GameView>();
        if (_gameView == null)
        {
        }
    }
    private void LoadImageData()
    {
        if (textAsset != null)
        {
            string jsonContent = textAsset.text;
            GameLogic.ImageDataList imageDataList = JsonUtility.FromJson<GameLogic.ImageDataList>(jsonContent);
            if (imageDataList != null && imageDataList.images != null)
            {
                for (int i = 0; i < imageDataList.images.Count; i++)
                {
                }
            }
            else
            {
            }
            _gameLogic.Initialize(imageDataList);
            _gameView.Initialize(imageDataList);
        }
        else
        {
        }
    }
    private void SetupEventListeners()
    {
        _gameLogic.OnGameStateUpdated += HandleGameStateUpdated;
        _gameLogic.OnCardsMatched += HandleCardsMatched;
        _gameLogic.OnCardsNotMatched += HandleCardsNotMatched;
        _gameLogic.OnGameWon += HandleGameWon;
        _gameLogic.OnGameOver += HandleGameOver;
        _gameLogic.OnStreakUpdated += HandleStreakUpdated;
        _gameLogic.OnCardLayoutGenerated += HandleCardLayoutGenerated;
        _gameView.OnCardClicked += HandleCardClicked;
        _gameView.OnNextButtonClicked += HandleNextButtonClicked;
        _gameView.OnGameExited += HandleGameExited;
        _gameView.OnPauseClicked += HandlePauseClicked;
        _gameView.OnResumeClicked += HandleResumeClicked;
        _gameView.OnBackToMenuClicked += HandleBackToMenuClicked;
    }
    private void CleanupEventListeners()
    {
        if (_gameLogic != null)
        {
            _gameLogic.OnGameStateUpdated -= HandleGameStateUpdated;
            _gameLogic.OnCardsMatched -= HandleCardsMatched;
            _gameLogic.OnCardsNotMatched -= HandleCardsNotMatched;
            _gameLogic.OnGameWon -= HandleGameWon;
            _gameLogic.OnGameOver -= HandleGameOver;
            _gameLogic.OnStreakUpdated -= HandleStreakUpdated;
            _gameLogic.OnCardLayoutGenerated -= HandleCardLayoutGenerated;
        }
        if (_gameView != null)
        {
            _gameView.OnCardClicked -= HandleCardClicked;
            _gameView.OnNextButtonClicked -= HandleNextButtonClicked;
            _gameView.OnGameExited -= HandleGameExited;
            _gameView.OnPauseClicked -= HandlePauseClicked;
            _gameView.OnResumeClicked -= HandleResumeClicked;
            _gameView.OnBackToMenuClicked -= HandleBackToMenuClicked;
        }
    }
    private void HandleBackToMenuClicked()
    {
        _viewManager?.ShowGameMenu();
    }
    private void HandleGameStateUpdated(PlayerData playerData)
    {
        _gameView.UpdateGameState(playerData);
    }
    private void HandleCardsMatched(int cardId1, int cardId2)
    {
        _gameView.ShowCardMatch(cardId1, cardId2);
        StartCoroutine(DelayedProcessingComplete());
    }
    private void HandleCardsNotMatched(int cardId1, int cardId2)
    {
        _gameView.ShowCardNoMatch(cardId1, cardId2);
        StartCoroutine(DelayedProcessingComplete());
    }
    private void HandleGameWon()
    {
        _gameView.HideGameplayUI();
        StartCoroutine(DelayedGameWonDisplay());
    }
    private IEnumerator DelayedGameWonDisplay()
    {
        yield return new WaitForSeconds(0.8f);
        PlayerData currentData = _gameLogic.GetPlayerData();
        if (_viewManager != null && _viewManager.GetMenuHandler() != null)
        {
            _viewManager.GetMenuHandler().ShowGameWonResult(currentData);
        }
        else
        {
            _viewManager?.ShowResultMenu();
        }
    }
    private void HandleGameOver()
    {
        _gameView.ShowGameOver();
    }
    private void HandleStreakUpdated(int streak)
    {
        _gameView.UpdateStreak(streak);
    }
    private void HandleCardLayoutGenerated(List<int> cardLayout)
    {
        PlayerData playerData = _gameLogic.GetPlayerData();
        _gameView.CreateCardGrid(cardLayout, playerData);
    }
    private void HandleCardClicked(int cardId)
    {
        _gameLogic.OnCardSelected(cardId);
    }
    private void HandleNextButtonClicked()
    {
        _gameLogic.ResetForNewRound();
    }
    private void HandleGameExited()
    {
        _viewManager?.ShowResultMenu();
    }
    private void HandlePauseClicked()
    {
        _gameView.ShowPausePanel();
    }
    private void HandleResumeClicked()
    {
        _gameView.HidePausePanel();
    }
    private IEnumerator DelayedProcessingComplete()
    {
        yield return new WaitForSeconds(1f);
        _gameLogic.OnCardProcessingComplete();
    }
    private string GetDifficultyFromGridSize(int gridSize)
    {
        return gridSize switch
        {
            3 => "L1",
            4 => "L2",
            5 => "L3",
            6 => "L4",
            7 => "L5",
            _ => "L1"
        };
    }
    public void OnExitGame()
    {
        _viewManager?.ShowMainMenu();
    }
    private void OnDestroy()
    {
        CleanupEventListeners();
    }
}