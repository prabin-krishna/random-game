using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
public class ResultMenu : MonoBehaviour
{
    [SerializeField] private GameObject _nextGame;
    [SerializeField] private GameObject _gameOver;    
    [SerializeField] private TextMeshProUGUI _gameOverScore;
    [SerializeField] private TextMeshProUGUI _nextGameScoreText;
    
     public EventHandler<ButtonClickEventArgs> onResultMenuClick; 
 public void ShowGameWon(PlayerData playerData)
    {
                
        _gameOver.SetActive(false);
        _nextGame.SetActive(true);
        
       
        if (_nextGameScoreText != null)
            _nextGameScoreText.text = $"Score: {playerData.Score}\nMoves: {playerData.TotalMove}";
        
            }
       public void ShowGameOver(PlayerData playerData)
    {
        string score = "";
                
        _nextGame.SetActive(false);
        _gameOver.SetActive(true);
        
       
            score+= $"Final Score: {playerData.Score}";
        
            score += $"\nTotal Moves: {playerData.TotalMove}";
        
            score += $"\nMatches Found: {playerData.MatchCounter}";
        _gameOverScore.text = score;
            }
    public void onButtonClick(int id)
    {
          string buttonName = "";
        switch (id)
        {
            case 1:
                buttonName = "MainMenu";
                break;
            case 2:
                buttonName = "NextGame";
                break;
            
        }
        onResultMenuClick?.Invoke(this, new ButtonClickEventArgs(buttonName));
    }
    
}
 
 
