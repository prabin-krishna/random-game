using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GameMenu : MonoBehaviour
{
    [SerializeField] private GameObject _l1;
    [SerializeField] private GameObject _l2;
    [SerializeField] private GameObject _l3;
    [SerializeField] private GameObject _l4;
    [SerializeField] private GameObject _l5;
    [SerializeField] private GameObject _back;
    public EventHandler<ButtonClickEventArgs> onGameMenuClick;
    public void onClick(int id)
    {
                
        string buttonName = "";
        switch (id)
        {
            case 0:
                buttonName = "L1";
                break;
            case 1:
                buttonName = "L2";
                break;
            case 2:
                buttonName = "L3";
                break;
            case 3:
                buttonName = "L4";
                break;
            case 4:
                buttonName = "L5";
                break;
            case 5:
                buttonName = "Back";
                break;
        }
        
        onGameMenuClick?.Invoke(this, new ButtonClickEventArgs(buttonName));
    }
}