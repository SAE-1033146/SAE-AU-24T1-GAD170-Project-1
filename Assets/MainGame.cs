using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGame : MonoBehaviour
{
    bool gameStarted = false;
    int pos = 35;
    int X = 0;
    int Y = 0;
    Dictionary<int, int> gameFloor = new Dictionary<int, int>();

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && (!gameStarted))
        { 
            gameStarted = true;
            gameFloor = this.GetComponent<FLOOR_Main>().GenerateFloor(5,1);
            Step();

        }   
        if (gameStarted)
        {

        }
    }
    void Step()
    {
        X = pos % 10;
        Y = pos / 10;
        this.GetComponent<TEXT_Display>().UpdateText(false, true, "X=" + X + "\nY=" + Y);

        this.GetComponent<TEXT_Display>().UpdateText(false, false, "O");


    }
}
