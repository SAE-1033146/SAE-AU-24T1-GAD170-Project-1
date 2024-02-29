using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

//REMEMBER: THE MAX LINE COUNT IS 24
//REMEMBER: THE MAX LINE COUNT IS 24
//REMEMBER: THE MAX LINE COUNT IS 24
//REMEMBER: THE MAX LINE COUNT IS 24
//REMEMBER: THE MAX LINE COUNT IS 24
//REMEMBER: THE MAX LINE COUNT IS 24
//REMEMBER: THE MAX LINE COUNT IS 24
//REMEMBER: THE MAX LINE COUNT IS 24
//REMEMBER: THE MAX LINE COUNT IS 24
//REMEMBER: THE MAX LINE COUNT IS 24
//REMEMBER: THE MAX LINE COUNT IS 24
//REMEMBER: THE MAX LINE COUNT IS 24
//REMEMBER: THE MAX LINE COUNT IS 24
//REMEMBER: THE MAX LINE COUNT IS 24
//REMEMBER: THE MAX LINE COUNT IS 24
//REMEMBER: THE MAX LINE COUNT IS 24
//REMEMBER: THE MAX LINE COUNT IS 24
//REMEMBER: THE MAX LINE COUNT IS 24
//REMEMBER: THE MAX LINE COUNT IS 24
//REMEMBER: THE MAX LINE COUNT IS 24
//REMEMBER: THE MAX LINE COUNT IS 24
//REMEMBER: THE MAX LINE COUNT IS 24
//REMEMBER: THE MAX LINE COUNT IS 24
//REMEMBER: THE MAX LINE COUNT IS 24
//REMEMBER: THE MAX LINE COUNT IS 24
//REMEMBER: THE MAX LINE COUNT IS 24

public class TEXT_Display : MonoBehaviour
{
    public TextMeshProUGUI leftDisplay;
    public TextMeshProUGUI rightDisplay;

    // Start is called before the first frame update
    void Start()
    {
        //UpdateText(false, true, "Hello World \nlol");
        rightDisplay.alignment = TextAlignmentOptions.Center;
        UpdateText(true, true, "Dumb\nDungeons\n\n\n\nPress Space to Start");
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateText(true, true, "a");
    }

    internal void UpdateText(bool Append, bool onLeft, string Text)
    {
        if (onLeft)
        {
            if (Append) { leftDisplay.text += Text; return; }
            leftDisplay.text = Text; return;
        } else {
            if(Append) { rightDisplay.text += Text; return; }
            rightDisplay.text = Text; return;
        }
    }
}
