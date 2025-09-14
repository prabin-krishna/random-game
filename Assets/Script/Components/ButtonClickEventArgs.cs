using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ButtonClickEventArgs : EventArgs
{
    public string ButtonName { get; }
    
    public ButtonClickEventArgs(string buttonName)
    {
        ButtonName = buttonName;
    }
}