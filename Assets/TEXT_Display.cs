using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class TEXT_Display : MonoBehaviour
{
    public TextMeshProUGUI leftDisplay;
    public TextMeshProUGUI topDisplay;
    public TextMeshProUGUI bottomDisplay;

    // Start is called before the first frame update
    void Start()
    {
        //UpdateText(false, true, "Hello World \nlol");
        topDisplay.alignment = TextAlignmentOptions.Center;
        UpdateText(true, 0, "Dumb\nDungeons\n\n\n\nPress Space to Start");
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateText(true, true, "a");
    }

    internal void UpdateText(bool Append, int disp, string Text)
    {
        switch (disp)
        {
            case 0:
                if (Append) { leftDisplay.text += Text; return; }
                leftDisplay.text = Text; return;
            case 1:
                if (Append) { topDisplay.text += Text; return; }
                topDisplay.text = Text; return;
            case 2:
                if (Append) { bottomDisplay.text += Text; return; }
                bottomDisplay.text = Text; return;
        }
    }
}
