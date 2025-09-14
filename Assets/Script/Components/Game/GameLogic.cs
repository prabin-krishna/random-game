using System;
using System.Collections.Generic;
using UnityEngine;
public class GameLogic
{
    public event Action<PlayerData> OnGameStateUpdated;
    public event Action<int, int> OnCardsMatched;
    public event Action<int, int> OnCardsNotMatched;
    public event Action OnGameWon;
    public event Action OnGameOver;
    public event Action<int> OnStreakUpdated;
    public event Action<List<int>> OnCardLayoutGenerated;
    private PlayerData _playerData;
    private SaveManager _saveManager;
    private List<int> _clickedCards = new List<int>();
    private int _streak = 0;
    private bool _processingCards = false;
    private ImageDataList _imageDataList;
    [Serializable]
    public class ImageData
    {
        public string id;
        public string fileName;
    }
    
    [Serializable]
    public class ImageDataList
    {
        public List<ImageData> images;
    }
    public GameLogic()
    {
        _saveManager = SaveManager.Instance;
    }
    public void Initialize(ImageDataList imageDataList)
    {
        _imageDataList = imageDataList;
    }
public void StartNewGame(string difficulty)
{
    _playerData = _saveManager.StartNewGame(difficulty);
    _streak = 0;
    _processingCards = false;
    
        
    GenerateCardLayout();
    NotifyGameStateUpdate();
}
public void LoadGame(PlayerData savedData)
{
    _playerData = savedData;
    _streak = 0;
    _processingCards = false;
    
            
    if (_playerData.ImageIndexList == null || _playerData.ImageIndexList.Count == 0)
    {
                GenerateCardLayout();
    }
    else
    {
                OnCardLayoutGenerated?.Invoke(_playerData.ImageIndexList);
    }
    
    NotifyGameStateUpdate();
}
    private void GenerateCardLayout()
    {
        int totalCards = _playerData.GridColumn * _playerData.GridRow;
        int totalPairs = totalCards / 2;
        
        _playerData.ImageIndexList.Clear();
        
                        
        if (_imageDataList == null || _imageDataList.images == null || _imageDataList.images.Count == 0)
        {
                        return;
        }
        
        if (totalPairs > _imageDataList.images.Count)
        {
        }
        
        for (int i = 0; i < totalPairs; i++)
        {
            int imageIndex = i % _imageDataList.images.Count; 
            _playerData.ImageIndexList.Add(imageIndex);
            _playerData.ImageIndexList.Add(imageIndex);
        }
        
        ShuffleList(_playerData.ImageIndexList);
                OnCardLayoutGenerated?.Invoke(_playerData.ImageIndexList);
    }
    public void OnCardSelected(int cardId)
    {
        _clickedCards.Add(cardId);
                if (_clickedCards.Count == 2)
        {
            _processingCards = true;
            ProcessCardPair(_clickedCards[0], _clickedCards[1]);
         
            _clickedCards.Clear();
        }
    }
   
private void ProcessCardPair(int cardId1, int cardId2)
{
    _playerData.TotalMove++;
        bool isMatch = _playerData.ImageIndexList[cardId1] == _playerData.ImageIndexList[cardId2];
        
    if (isMatch)
    {
        HandleMatch(cardId1, cardId2);
    }
    else
    {
        HandleNoMatch(cardId1, cardId2);
    }
    NotifyGameStateUpdate();
}
    private void HandleMatch(int cardId1, int cardId2)
    {
        _playerData.MatchCounter++;
        _streak++;
        _playerData.Score++;
        if (_playerData.ImageDataList == null)
            _playerData.ImageDataList = new List<int>();
            
        _playerData.ImageDataList.Add(cardId1);
        _playerData.ImageDataList.Add(cardId2);
        OnCardsMatched?.Invoke(cardId1, cardId2);
        OnStreakUpdated?.Invoke(_streak);
        
            }
    private void HandleNoMatch(int cardId1, int cardId2)
    {
                _playerData.Score += _streak; 
        _streak = 0;
        _playerData.Lives--;
        OnCardsNotMatched?.Invoke(cardId1, cardId2);
        OnStreakUpdated?.Invoke(0);
        
            }
    private void CheckGameEndConditions()
    {
        if (_playerData.Lives <= 0)
        {
            OnGameOver?.Invoke();
            ResetGameData();
        }
        else if (_playerData.MatchCounter >= GetTotalPairs())
        {
            OnGameWon?.Invoke();
        }
    }
   public void OnCardProcessingComplete()
{
    _processingCards = false;
    SaveGame();
    
    CheckGameEndConditions();
}
    private int GetTotalPairs()
    {
        return (_playerData.GridColumn * _playerData.GridRow) / 2;
    }
    private bool IsCardRevealed(int cardId)
    {
        return _playerData.ImageDataList != null && _playerData.ImageDataList.Contains(cardId);
    }
    private void ResetGameData()
    {
        _playerData.TotalMove = 0;
        _playerData.MatchCounter = 0;
        _playerData.CurrentRevealedCardId = -1;
        _playerData.CurrentRevealedCardIndex = -1;
        _playerData.RevealedCardCounter = 0;
        _playerData.ImageDataList = new List<int>();
        _playerData.ImageIndexList = new List<int>();
        SaveGame();
    }
    private void SaveGame()
    {
        _saveManager.UpdatePlayerData(_playerData);
        _saveManager.AutoSave();
    }
    private void NotifyGameStateUpdate()
    {
        OnGameStateUpdated?.Invoke(_playerData);
    }
    private List<T> ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = UnityEngine.Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
        return list;
    }
    public PlayerData GetPlayerData() => _playerData;
    public ImageDataList GetImageDataList() => _imageDataList;
    public bool IsProcessingCards() => _processingCards;
    public int GetStreak() => _streak;
    public void ResetForNewRound()
    {
        _playerData.TotalMove = 0;
        _playerData.MatchCounter = 0;
        _playerData.Score = 0;
        _playerData.CurrentRevealedCardId = -1;
        _playerData.CurrentRevealedCardIndex = -1;
        _playerData.RevealedCardCounter = 0;
        _playerData.ImageDataList = new List<int>();
        _streak = 0;
        
        GenerateCardLayout();
        NotifyGameStateUpdate();
    }
}