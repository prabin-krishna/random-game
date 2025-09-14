using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[Serializable]
public class PlayerData
{
    public int TotalMove { get; set; }
    public int MatchCounter { get; set; }
    public int CurrentRevealedCardId { get; set; }
    public int CurrentRevealedCardIndex { get; set; }
    public int RevealedCardCounter { get; set; }
    public int GridColumn { get; set; }
    public int GridRow { get; set; }
    public int Score { get; set; }
    public int Lives { get; set; }
    public List<int> ImageDataList { get; set; }
    public List<int> ImageIndexList { get; set; }
    private const string DATA_EXISTS_KEY = "playerDataExists";
    public PlayerData()
    {
        ImageDataList = new List<int>();
        ImageIndexList = new List<int>();
    }
    public PlayerData(int[] playerData, List<int> imageDataList, List<int> imageIndexList)
    {
        if (playerData.Length < 9)
            throw new ArgumentException("Game data array must contain at least 9 elements");
        TotalMove = playerData[0];
        MatchCounter = playerData[1];
        CurrentRevealedCardId = playerData[2];
        CurrentRevealedCardIndex = playerData[3];
        RevealedCardCounter = playerData[4];
        GridColumn = playerData[5];
        GridRow = playerData[6];
        Score = playerData[7];
        Lives = playerData[8];
        ImageDataList = imageDataList ?? new List<int>();
        ImageIndexList = imageIndexList ?? new List<int>();
    }
    public int[] ToPlayerDataArray()
    {
        return new int[]
        {
            TotalMove,
            MatchCounter,
            CurrentRevealedCardId,
            CurrentRevealedCardIndex,
            RevealedCardCounter,
            GridColumn,
            GridRow,
            Score,
            Lives
        };
    }
    public void SaveToPlayerPrefs()
    {
        PlayerPrefs.SetString("totalMove", TotalMove.ToString());
        PlayerPrefs.SetString("matchCounter", MatchCounter.ToString());
        PlayerPrefs.SetString("currentRevealedCardId", CurrentRevealedCardId.ToString());
        PlayerPrefs.SetString("currentRevealedCardIndex", CurrentRevealedCardIndex.ToString());
        PlayerPrefs.SetString("revealedCardCounter", RevealedCardCounter.ToString());
        PlayerPrefs.SetString("gridColumn", GridColumn.ToString());
        PlayerPrefs.SetString("gridRow", GridRow.ToString());
        PlayerPrefs.SetString("score", Score.ToString());
        PlayerPrefs.SetString("lives", Lives.ToString());
        List<string> stringList = ImageDataList.Select(x => x.ToString()).ToList();
        PlayerPrefs.SetString("imageDataList", String.Join(",", stringList));
        List<string> stringIndexList = ImageIndexList.Select(x => x.ToString()).ToList();
        PlayerPrefs.SetString("imageIndexList", String.Join(",", stringIndexList));
        PlayerPrefs.SetInt(DATA_EXISTS_KEY, 1);

        PlayerPrefs.Save();
    }
    public static PlayerData LoadFromPlayerPrefs()
    {
        var playerData = new PlayerData();
        playerData.TotalMove = int.Parse(PlayerPrefs.GetString("totalMove", "0"));
        playerData.MatchCounter = int.Parse(PlayerPrefs.GetString("matchCounter", "0"));
        playerData.CurrentRevealedCardId = int.Parse(PlayerPrefs.GetString("currentRevealedCardId", "-1"));
        playerData.CurrentRevealedCardIndex = int.Parse(PlayerPrefs.GetString("currentRevealedCardIndex", "-1"));
        playerData.RevealedCardCounter = int.Parse(PlayerPrefs.GetString("revealedCardCounter", "0"));
        playerData.GridColumn = int.Parse(PlayerPrefs.GetString("gridColumn", "4"));
        playerData.GridRow = int.Parse(PlayerPrefs.GetString("gridRow", "4"));
        playerData.Score = int.Parse(PlayerPrefs.GetString("score", "0"));
        playerData.Lives = int.Parse(PlayerPrefs.GetString("lives", "3"));
        string imageDataString = PlayerPrefs.GetString("imageDataList", "");
        if (!string.IsNullOrEmpty(imageDataString))
        {
            playerData.ImageDataList = imageDataString.Split(',').Select(int.Parse).ToList();
        }
        string imageIndexString = PlayerPrefs.GetString("imageIndexList", "");
        if (!string.IsNullOrEmpty(imageIndexString))
        {
            playerData.ImageIndexList = imageIndexString.Split(',').Select(int.Parse).ToList();
        }
        return playerData;
    }
    public static void DeleteAllData()
    {
        PlayerPrefs.DeleteKey("totalMove");
        PlayerPrefs.DeleteKey("matchCounter");
        PlayerPrefs.DeleteKey("currentRevealedCardId");
        PlayerPrefs.DeleteKey("currentRevealedCardIndex");
        PlayerPrefs.DeleteKey("revealedCardCounter");
        PlayerPrefs.DeleteKey("gridColumn");
        PlayerPrefs.DeleteKey("gridRow");
        PlayerPrefs.DeleteKey("score");
        PlayerPrefs.DeleteKey("lives");
        PlayerPrefs.DeleteKey("imageDataList");
        PlayerPrefs.DeleteKey("imageIndexList");
        PlayerPrefs.DeleteKey(DATA_EXISTS_KEY);
        PlayerPrefs.Save();

    }
    public void ResetToDefaults()
    {
        TotalMove = 0;
        MatchCounter = 0;
        CurrentRevealedCardId = -1;
        CurrentRevealedCardIndex = -1;
        RevealedCardCounter = 0;
        GridColumn = 4;
        GridRow = 4;
        Score = 0;
        Lives = 3;
        ImageDataList.Clear();
        ImageIndexList.Clear();
    }

}