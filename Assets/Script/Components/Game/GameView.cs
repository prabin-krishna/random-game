using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class GameView : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private GameObject _cardContainer;
    [SerializeField] private GameObject _gameWindow;
    [SerializeField] private GameObject _nextButton;
    [SerializeField] private GameObject _btnBg;
    [Header("UI Text Elements")]
    [SerializeField] private TextMeshProUGUI _moveText;
    [SerializeField] private TextMeshProUGUI _matchText;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _streakText;
    [SerializeField] private TextMeshProUGUI _livesRemainingText;
    [SerializeField] private TextMeshProUGUI _gameOverText;
    [Header("Game Panels")]
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _gameExitButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private GameObject _buttonPanel;
    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    public event Action<int> OnCardClicked;
    public event Action OnNextButtonClicked;
    public event Action OnGameExited;
    public event Action OnPauseClicked;
    public event Action OnResumeClicked;
    public event Action OnBackToMenuClicked;
    private List<GameObject> _cards = new List<GameObject>();
    private PlayerData _currentPlayerData;
    private GameLogic.ImageDataList _imageDataList;
    private static readonly Dictionary<int, float> SCALE_FACTOR = new Dictionary<int, float>
    {
        {2, 0.5f},
        {3, 0.35f},
        {4, 0.25f},
        {5, 0.2f},
        {6, 0.15f},
        {7, 0.12f}
    };
    private void Start()
    {
        _gameOverText?.gameObject.SetActive(false);
        _nextButton?.SetActive(false);
        _pausePanel?.SetActive(false);
        SetupButtonListeners();
    }
    private void SetupButtonListeners()
    {
        _pauseButton?.onClick.AddListener(() =>
        {
            OnPauseClicked?.Invoke();
        });
        _resumeButton?.onClick.AddListener(() =>
        {
            OnResumeClicked?.Invoke();
        });
        _exitButton?.onClick.AddListener(() =>
        {
            HidePausePanel();
            OnGameExited?.Invoke();
        });
        _gameExitButton?.onClick.AddListener(() =>
        {
            Application.Quit();
        });
        _backButton?.onClick.AddListener(() =>
        {
            OnBackToMenuClicked?.Invoke();
        });
    }
    public void Initialize(GameLogic.ImageDataList imageDataList)
    {
        _imageDataList = imageDataList;
    }
    public void UpdateGameState(PlayerData playerData)
    {
        _currentPlayerData = playerData;
        UpdateUI();
    }
    public void CreateCardGrid(List<int> cardLayout, PlayerData playerData)
    {
        ClearGrid();
        _currentPlayerData = playerData;
        _gameWindow?.SetActive(true);
        _cardContainer?.SetActive(true);
        ShowGameplayUI();
        SetupGridLayout();
        CreateCards(cardLayout);
        StartCoroutine(DelayedRevealSequence(playerData));
    }
    private IEnumerator DelayedRevealSequence(PlayerData playerData)
    {
        yield return null;
        yield return StartCoroutine(RevealAndHide(playerData));
    }
    private IEnumerator RevealAndHide(PlayerData playerData)
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < _cards.Count; i++)
        {
            _cards[i].GetComponent<Card>().SetInInitSequence(true);
        }
        yield return StartCoroutine(SequentialInitializationZoom());
        yield return new WaitForSeconds(1.0f);
        for (int i = 0; i < _cards.Count; i++)
        {
            _cards[i].GetComponent<Card>().ToggleHideCard(true);
        }
        yield return new WaitForSeconds(0.7f);
        for (int i = 0; i < _cards.Count; i++)
        {
            Card card = _cards[i].GetComponent<Card>();
            card.ResetToIdleState();
            card.SetInInitSequence(false);
        }
    }
    private void ApplySavedState(PlayerData playerData)
    {
        if (playerData.ImageDataList != null && playerData.ImageDataList.Count > 0)
        {
            foreach (int cardId in playerData.ImageDataList)
            {
                if (cardId >= 0 && cardId < _cards.Count && _cards[cardId] != null)
                {
                    _cards[cardId].GetComponent<Card>().DestroyCard();
                }
            }
        }
        if (playerData.CurrentRevealedCardIndex >= 0 &&
            playerData.CurrentRevealedCardIndex < _cards.Count &&
            _cards[playerData.CurrentRevealedCardIndex] != null)
        {
            _cards[playerData.CurrentRevealedCardIndex].GetComponent<Card>().OnReveal();
        }
        UpdateGameState(playerData);
    }
    public void RestoreGameState(PlayerData playerData, bool isNewGame = false)
    {
        _currentPlayerData = playerData;
        if (isNewGame)
        {
            return;
        }
        if (playerData.ImageDataList != null && playerData.ImageDataList.Count > 0)
        {
            foreach (int cardId in playerData.ImageDataList)
            {
                if (cardId >= 0 && cardId < _cards.Count)
                {
                    _cards[cardId].GetComponent<Card>().DestroyCard();
                }
            }
        }
        if (playerData.CurrentRevealedCardIndex != -1 &&
            playerData.CurrentRevealedCardIndex < _cards.Count)
        {
            _cards[playerData.CurrentRevealedCardIndex].GetComponent<Card>().OnReveal();
        }
    }
    public void ShowCardMatch(int cardId1, int cardId2)
    {
        StartCoroutine(HandleCardMatchAnimation(cardId1, cardId2));
    }
    public void ShowCardNoMatch(int cardId1, int cardId2)
    {
        StartCoroutine(HandleCardNoMatchAnimation(cardId1, cardId2));
    }
    public void ShowPausePanel()
    {
        if (_pausePanel != null)
        {
            _pausePanel.SetActive(true);
            Time.timeScale = 0f;
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PauseAmbient();
            }
        }
    }
    public void HidePausePanel()
    {
        if (_pausePanel != null)
        {
            _pausePanel.SetActive(false);
            Time.timeScale = 1f;
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.ResumeAmbient();
            }
        }
    }
    public void UpdateStreak(int streak)
    {
        if (streak > 0)
        {
            _streakText.gameObject.SetActive(true);
            _streakText.text = streak + "x Streak";
        }
        else
        {
            _streakText.gameObject.SetActive(false);
        }
    }
    public void ShowGameOver()
    {
        StartCoroutine(HandleGameOver());
    }
    public void ShowGameWon()
    {
        StartCoroutine(HandleGameWon());
    }
    private IEnumerator SequentialInitializationZoom()
    {
        for (int i = 0; i < _cards.Count; i++)
        {
            Card card = _cards[i].GetComponent<Card>();
            if (_currentPlayerData.ImageDataList.Contains(card.GetCardId()))
                continue;
            card.ToggleHideCard(false);
            yield return new WaitForSeconds(0.3f);
        }
    }
    public void HideGameplayUI()
    {
        _moveText?.gameObject.SetActive(false);
        _matchText?.gameObject.SetActive(false);
        _scoreText?.gameObject.SetActive(false);
        _streakText?.gameObject.SetActive(false);
        _livesRemainingText?.gameObject.SetActive(false);
        _btnBg?.SetActive(false);
        _buttonPanel?.gameObject.SetActive(false);
        _nextButton?.SetActive(false);
    }
    public void ShowGameplayUI()
    {
        _moveText?.gameObject.SetActive(true);
        _matchText?.gameObject.SetActive(true);
        _scoreText?.gameObject.SetActive(true);
        _streakText?.gameObject.SetActive(true);
        _livesRemainingText?.gameObject.SetActive(true);
        _btnBg?.SetActive(true);
        _buttonPanel?.gameObject.SetActive(true);
    }
    private void SetupGridLayout()
    {
        GridLayoutGroup gridLayoutGroup = _cardContainer.GetComponent<GridLayoutGroup>();
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.Flexible;
        RectTransform containerRect = _cardContainer.GetComponent<RectTransform>();
        float containerWidth = containerRect.rect.width;
        float containerHeight = containerRect.rect.height;
        float cellWidth = containerWidth / _currentPlayerData.GridColumn;
        float cellHeight = containerHeight / _currentPlayerData.GridRow;
        cellWidth = cellWidth * 0.9f;
        cellHeight = (cellHeight * 0.9f) + 30;
        cellWidth = Mathf.Max(cellWidth, 80);
        cellHeight = Mathf.Max(cellHeight, 120);
        gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);
        gridLayoutGroup.spacing = new Vector2(5, 5);
        gridLayoutGroup.childAlignment = TextAnchor.UpperCenter;
    }
    private void CreateCards(List<int> cardLayout)
    {
        int totalCards = _currentPlayerData.GridColumn * _currentPlayerData.GridRow;
        for (int i = 0; i < totalCards; i++)
        {
            GameObject card = Instantiate(_cardPrefab, _cardContainer.transform);
            float scaleFactor = SCALE_FACTOR.ContainsKey(_currentPlayerData.GridRow) ?
                              SCALE_FACTOR[_currentPlayerData.GridRow] : 0.25f;
            card.transform.localScale = Vector3.one * scaleFactor;
            card.SetActive(true);
            int imageIndex = cardLayout[i];
            if (imageIndex >= _imageDataList.images.Count)
            {
                imageIndex = imageIndex % _imageDataList.images.Count;
            }
            Sprite sprite = Resources.Load<Sprite>("Images/" + _imageDataList.images[imageIndex].fileName);
            if (sprite == null)
            {
            }
            Card cardComponent = card.GetComponent<Card>();
            cardComponent.SetResultImage(sprite);
            cardComponent.SetImageId(imageIndex);
            cardComponent.SetCardId(i);
            cardComponent.OnCardClicked += HandleCardClick;
            cardComponent.ToggleHideCard(true);
            _cards.Add(card);
        }
        foreach (int id in _currentPlayerData.ImageDataList)
        {
            if (_currentPlayerData.ImageDataList.Contains(id))
                _cards[id].GetComponent<Card>().HideCard();
        }
    }
    private void HandleCardClick(int cardId)
    {
        if (_pausePanel.activeSelf)
            return;
        OnCardClicked?.Invoke(cardId);
    }
    private void UpdateUI()
    {
        if (_currentPlayerData == null) return;
        _moveText.text = "MOVE : " + _currentPlayerData.TotalMove.ToString();
        _matchText.text = "MATCH : " + _currentPlayerData.MatchCounter.ToString();
        _livesRemainingText.text = "LIVES : " + _currentPlayerData.Lives.ToString();
        _scoreText.text = "SCORE : " + _currentPlayerData.Score.ToString();
    }
    private IEnumerator HandleCardMatchAnimation(int cardId1, int cardId2)
    {
        yield return new WaitForSeconds(0.5f);
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCardMatch();
        }
        _cards[cardId1].GetComponent<Card>().DestroyCard();
        _cards[cardId2].GetComponent<Card>().DestroyCard();
        OnCardProcessingComplete();
    }
    private IEnumerator HandleCardNoMatchAnimation(int cardId1, int cardId2)
    {
        yield return new WaitForSeconds(0.5f);
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCardMismatch();
        }
        _cards[cardId1].GetComponent<Card>().ToggleHideCard(true);
        _cards[cardId2].GetComponent<Card>().ToggleHideCard(true);
        OnCardProcessingComplete();
    }
    private void OnCardProcessingComplete()
    {
        UpdateUI();
    }
    private IEnumerator HandleGameOver()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameOver();
        }
        yield return new WaitForSeconds(1f);
        ClearGrid();
        _gameWindow.SetActive(false);
        _gameOverText.gameObject.SetActive(false);
        OnGameExited?.Invoke();
    }
    private IEnumerator HandleGameWon()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayYouWon();
        }
        ClearGrid();
        _gameWindow.SetActive(false);
        _nextButton.SetActive(true);
        yield return null;
    }
    private void ClearGrid()
    {
        foreach (GameObject card in _cards)
        {
            if (card != null)
            {
                card.GetComponent<Card>().OnCardClicked -= HandleCardClick;
                Destroy(card);
            }
        }
        _cards.Clear();
    }
    public void ShowGameWindow()
    {
        _gameWindow.SetActive(true);
        ShowGameplayUI();
    }
    public void HideGameWindow()
    {
        _gameWindow.SetActive(false);
    }
    public void OnNextClick()
    {
        _nextButton.SetActive(false);
        OnNextButtonClicked?.Invoke();
    }
    private void OnApplicationQuit()
    {
        Time.timeScale = 1f;
        OnGameExited?.Invoke();
    }
    private void OnDestroy()
    {
        Time.timeScale = 1f;
        ClearGrid();
        if (_pauseButton != null)
            _pauseButton.onClick.RemoveAllListeners();
        if (_resumeButton != null)
            _resumeButton.onClick.RemoveAllListeners();
        if (_exitButton != null)
            _exitButton.onClick.RemoveAllListeners();
        if (_backButton != null)
            _backButton.onClick.RemoveAllListeners();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            PlayerPrefs.DeleteAll();
        }
    }
}