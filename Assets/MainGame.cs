using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGame : MonoBehaviour
{
    bool gameStarted = false;
    int pos = 35;
    int X = 0;
    int Y = 0;
    bool debug = true;

    int movesTakenMAP = 0;
    int movesTakenFIGHT = 0;

    Dictionary<int, int> gameFloor = new Dictionary<int, int>();
    Dictionary<string, int> gameInventory = new Dictionary<string, int>();
    Dictionary<string, int> gameStats = new Dictionary<string, int>();

    List<string> potionTypes = new() { "RevealRoom", "RevealEnemy", "AgilityUp", "StrengthUp", "DefenseUp" };

    bool mapRoaming = false;
    bool inBattle = false;
    bool pointSelect = false;
    // Start is called before the first frame update
    void Start()
    {
        gameStats["Health"] = 100;
        gameStats["Points"] = 30;
        gameStats["Defense"] = 1;
        gameStats["Strength"] = 1;
        gameStats["Agility"] = 1;
        gameStats["XP"] = 0;
        gameStats["Level"] = 1;
        gameStats["Score"] = 0;

        gameInventory["RevealRoom"] = 0;
        gameInventory["RevealEnemy"] = 0;
        gameInventory["AgilityUp"] = 0;
        gameInventory["StrengthUp"] = 0;
        gameInventory["DefenseUp"] = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && (!gameStarted))
        { 
            gameStarted = true;
            pointSelect = true;
            Step();
            gameFloor = this.GetComponent<FLOOR_Main>().GenerateFloor(7, 0.2);
            
        }

        if (gameStarted)
        {
            if (pointSelect)
            {
                if (Input.GetKeyDown(KeyCode.Z)) { gameStats["Points"]--; gameStats["Defense"]++; Step(); }
                if (Input.GetKeyDown(KeyCode.X)) { gameStats["Points"]--; gameStats["Strength"]++; Step(); }
                if (Input.GetKeyDown(KeyCode.C)) { gameStats["Points"]--; gameStats["Agility"]++; Step(); }
                if (gameStats["Points"] <= 0) { pointSelect = false; mapRoaming = true;  Step(); }
            }
            if (mapRoaming)
            { 
                    // -1 is left
                    // +1 is right
                    // -10 is down
                    // +10 is up
                    if (debug && Input.GetKeyDown(KeyCode.X))
                    {
                        pos = 35;
                        gameFloor = this.GetComponent<FLOOR_Main>().GenerateFloor(10, 0.8);
                        Step();
                    }
                if (Input.GetKeyDown(KeyCode.LeftArrow) && gameFloor.ContainsKey(pos - 1) && X > 1)
                {
                    updatePos(-1);
                }
                if (Input.GetKeyDown(KeyCode.RightArrow) && gameFloor.ContainsKey(pos + 1) && X < 9)
                {
                    updatePos(1);
                }
                if (Input.GetKeyDown(KeyCode.DownArrow) && gameFloor.ContainsKey(pos - 10) && Y > 1)
                {
                    updatePos(-10);
                }
                if (Input.GetKeyDown(KeyCode.UpArrow) && gameFloor.ContainsKey(pos + 10) && Y < 9)
                {
                    updatePos(10);
                }
            }
        }
    }
    void updatePos(int add)
    {
        pos = pos + add;
        movesTakenMAP++;
        Step();
    }
    void Step()
    {
        if (pointSelect)
        {
            this.GetComponent<TEXT_Display>().UpdateText(false, 0, "Z - Defense - " + gameStats["Defense"] + "\nX - Strength - " + gameStats["Strength"] + "\nC - Agility - " + gameStats["Agility"]);
            this.GetComponent<TEXT_Display>().UpdateText(false, 1, "Upgrade your stats, dummy.\nPoints remaining: " + gameStats["Points"]);
            return;
        }
        if (mapRoaming)
        {
            X = pos % 10;
            Y = pos / 10;

            this.GetComponent<TEXT_Display>().UpdateText(false, 0, "X=" + X + "\nY=" + Y);
            if (gameInventory["RevealRoom"] > 0) { this.GetComponent<TEXT_Display>().UpdateText(true, 0, "\n\n\nRoom Reveals: " + gameInventory["RevealRoom"]); }
            this.GetComponent<TEXT_Display>().UpdateText(false, 1, this.GetComponent<FLOOR_Main>().displayFloor(pos));
            this.GetComponent<TEXT_Display>().UpdateText(false, 2, "");

            switch (gameFloor[pos])
            {
                case 0: //empty
                    break;
                case 1: //player spawn
                    break;
                case 2: //item
                    int rand = Random.Range(0, potionTypes.Count);
                    this.GetComponent<TEXT_Display>().UpdateText(true, 2, "Picked up.. " + potionTypes[rand]  + "!\n");
                    gameInventory[potionTypes[rand]]++;
                    gameFloor[pos] = 0;
                    if (gameInventory["RevealRoom"] == 1) { this.GetComponent<TEXT_Display>().UpdateText(true, 0, "\n\n\nRoom Reveals: 1"); }
                    break;
                case 3: //enemy
                    mapRoaming = false;
                    inBattle = true;
                    this.GetComponent<TEXT_Display>().UpdateText(false, 0, "Z - Attack\nX - Defend\nC - Item");
                    this.GetComponent<TEXT_Display>().UpdateText(false, 1, "Your Stats:\nDefense - " + gameStats["Defense"] + "\nStrength - " + gameStats["Strength"] + "\nAgility - " + gameStats["Agility"]);
                    this.GetComponent<TEXT_Display>().UpdateText(false, 2, "You've encountered an enemy!");
                    break;
                case 4: //exit
                    break;
            }
            return;
        }



    }
}
