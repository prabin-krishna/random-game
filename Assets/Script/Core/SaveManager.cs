using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SaveManager
{
    private static SaveManager _instance;
    private PlayerData _currentPlayerData;
    public static SaveManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new SaveManager();
            return _instance;
        }
    }
    private SaveManager()
    {
        _currentPlayerData = new PlayerData();
    }
    public PlayerData GetPlayerData()
    {
        return _currentPlayerData;
    }
    public PlayerData LoadGame()
    {
        if (HasSavedGame())
        {
            _currentPlayerData = PlayerData.LoadFromPlayerPrefs();
            return _currentPlayerData;
        }
        else
        {
            _currentPlayerData = new PlayerData();
            return _currentPlayerData;
        }
    }
    public void SaveGame()
    {
        if (_currentPlayerData != null)
        {
            _currentPlayerData.SaveToPlayerPrefs();
        }
    }
    public PlayerData StartNewGame(string difficulty)
    {
        _currentPlayerData = new PlayerData();
        var (gridColumn, gridRow) = GetGridSizeForDifficulty(difficulty);
        _currentPlayerData.GridColumn = gridColumn;
        _currentPlayerData.GridRow = gridRow;
        _currentPlayerData.Lives = GetLivesForDifficulty(difficulty);
        return _currentPlayerData;
    }
    public bool HasSavedGame()
    {
        return PlayerPrefs.HasKey("playerDataExists");
    }
    public void DeleteSaveData()
    {
        PlayerData.DeleteAllData();
        _currentPlayerData = new PlayerData();
    }
    public void UpdatePlayerData(PlayerData newData)
    {
        _currentPlayerData = newData;
    }
    public void AutoSave()
    {
        SaveGame();
    }
    private (int column, int row) GetGridSizeForDifficulty(string difficulty)
    {
        return difficulty switch
        {
            "L1" => (3, 2),
            "L2" => (4, 3),
            "L3" => (4, 4),
            "L4" => (5, 4),
            "L5" => (6, 4),
            _ => (3, 2)
        };
    }
    private int GetLivesForDifficulty(string difficulty)
    {
        return difficulty switch
        {
            "L1" => 8,
            "L2" => 6,
            "L3" => 4,
            "L4" => 3,
            "L5" => 2,
            _ => 8
        };
    }
}