using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGame : MonoBehaviour
{
    bool gameStarted = false;
    int pos = 35;
    int X = 0;
    int Y = 0;
    bool debug = false;

    public int floorLevel = 0;

    int movesTakenMAP = 0;
    int movesTakenFIGHT = 0;

    private BATTLE_Enemy enemyGen;
    private FLOOR_Main floorGen;
    private TEXT_Display txtDisplay;

    Dictionary<int, int> gameFloor = new Dictionary<int, int>();
    Dictionary<string, int> gameInventory = new Dictionary<string, int>();

    Dictionary<string, double> gameStats = new Dictionary<string, double>();
    Dictionary<string, double> enemy = new Dictionary<string, double>();

    List<string> potionTypes = new() { "RevealRoom", "RevealEnemy", "DefenseUp", "StrengthUp", "AgilityUp" };
    List<string> potionAdd = new() { "BDefense", "BStrength", "BAgility" };

    bool mapRoaming = false;
    bool inBattle = false;
    bool pointSelect = false;
    bool choosingItem = false;
    bool revealedEnemy = false;
    bool noPotions = false;
    bool tooTired = false;
    bool roomRevealed = false;

    double globalDifficulty = 0.3;

    string enemyMove = "";
    // Start is called before the first frame update
    void Start()
    {
        enemyGen = GetComponent<BATTLE_Enemy>();
        floorGen = GetComponent<FLOOR_Main>();
        txtDisplay = GetComponent<TEXT_Display>();

        gameStats["Health"] = 100;
        gameStats["Points"] = 30;
        gameStats["Defense"] = 10;
        gameStats["Strength"] = 10;
        gameStats["Agility"] = 10;
        gameStats["BDefense"] = 1; //battle stats, used only in battle and can be modified by potions
        gameStats["BStrength"] = 1;
        gameStats["BAgility"] = 1;
        gameStats["XP"] = 0;
        gameStats["Level"] = 1;
        gameStats["Score"] = 0;

        gameInventory["RevealRoom"] = 0;
        gameInventory["RevealEnemy"] = 1;
        gameInventory["DefenseUp"] = 1;
        gameInventory["StrengthUp"] = 1;
        gameInventory["AgilityUp"] = 1;

    }
    void resetGame()
    {
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && (!gameStarted))
        { 
            gameStarted = true;
            pointSelect = true;
            Step();
            gameFloor = floorGen.GenerateFloor(floorLevel+1, globalDifficulty);
            
        }

        if (gameStarted)
        {
            if (inBattle)
            {
                if (choosingItem)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1)) { usePotion(1); }
                    if (Input.GetKeyDown(KeyCode.Alpha2)) { usePotion(2); }
                    if (Input.GetKeyDown(KeyCode.Alpha3)) { usePotion(3); }
                    if (Input.GetKeyDown(KeyCode.Alpha4)) { usePotion(4); }

                }else{
                    if (Input.GetKeyDown(KeyCode.A))
                    {
                        doMove("defend");
                    }

                    if (Input.GetKeyDown(KeyCode.S)) 
                    {
                        if (gameStats["BAgility"] > 0)
                        {
                            doMove("attack");
                        } else {
                            if (!tooTired) { txtDisplay.UpdateText(true, 2, "\nYou're too tired to swing."); }
                            tooTired = true;
                        }

                    }
                    
                    if (Input.GetKeyDown(KeyCode.D))
                    {
                        if (gameInventory["RevealEnemy"] + gameInventory["StrengthUp"] + gameInventory["DefenseUp"] + gameInventory["AgilityUp"] > 0) { 
                            choosingItem = true;
                            movesTakenFIGHT++;
                            string itemList = "";
                            for (int i = 1; i < 5; i++)
                            {
                                itemList = itemList + i + " - " + potionTypes[i] + ": " + gameInventory[potionTypes[i]] + "\n";
                            }
                            txtDisplay.UpdateText(false, 0, itemList);
                        }else{
                            if (!noPotions) { txtDisplay.UpdateText(true, 2, "\nYou have no potions."); }
                            noPotions = true;
                        }
                    }
                }
            }
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
                if (debug && Input.GetKeyDown(KeyCode.Q))
                {
                    pos = 35;
                    gameFloor = floorGen.GenerateFloor(10, 0.8);
                    roomRevealed = false;
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
                if (Input.GetKeyDown(KeyCode.Space) && gameInventory["RevealRoom"] > 0)
                {
                    gameInventory["RevealRoom"]--;
                    roomRevealed = true;
                    Step();

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
    void usePotion(int num)
    {
        if (gameInventory[potionTypes[num]] > 0)
        {
            gameInventory[potionTypes[num]]--;
            if (num == 1) { revealedEnemy = true; } else { gameStats[potionAdd[num - 2]] += gameStats["Level"]; }
            //print(potionAdd[num - 2] + " OR " + potionAdd[num]);
            choosingItem = false;
            txtDisplay.UpdateText(false, 2, "Used " + potionTypes[num] + "!");
            Step();
        }
    }

    void doMove(string choice)
    {
        //txtDisplay.UpdateText(false, 2, "You " + choice + ".");
        enemyMove = enemyGen.BATTLE_AI(choice);
        //print(enemyMove);
        doDamage(choice, enemyMove);
    }
    void doDamage(string playerChoice, string enemyChoice)
    {
        if (playerChoice == "attack" && enemyChoice == "attack")
        {
            if (gameStats["BAgility"] >= enemy["Agility"]) //player gets priority
            {
                double damageDealt = Math.Max(gameStats["BAgility"] + gameStats["BStrength"] - enemy["Defense"],0);
                double agilityLost = (enemy["Defense"] + gameStats["BStrength"] / gameStats["Agility"]);

                enemy["Health"] -= damageDealt;
                gameStats["BAgility"] -= agilityLost;

                enemy["Defense"] -= enemy["Defense"] / gameStats["BStrength"];

                txtDisplay.UpdateText(false, 2, "You swing first for " + Math.Round(damageDealt, 1) + " Health.");
                txtDisplay.UpdateText(true, 2, "\nYou lose " + Math.Round(agilityLost, 1) + " Agility.");
            } else {
                double damageDealt = Math.Max(enemy["Agility"] + enemy["Strength"] - gameStats["BDefense"], 0);
                double enemyAgilityLost = (gameStats["BDefense"] + enemy["Strength"] / enemy["MAgility"]);

                gameStats["Health"] -= damageDealt;
                enemy["Agility"] -= enemyAgilityLost;

                txtDisplay.UpdateText(false, 2, "The enemy outspeeds you, dealing " + Math.Round(damageDealt, 1) + " Health.");
                if (revealedEnemy) { txtDisplay.UpdateText(true, 2, "\nThe enemy loses " + Math.Round(enemyAgilityLost, 1) + " Agility."); }
            }

            
            
        }

        if (playerChoice == "defend" && enemyChoice == "defend")
        {
            enemy["Agility"] += (enemy["MAgility"] / 10);
            gameStats["BAgility"] += (gameStats["Agility"] / 10);

            txtDisplay.UpdateText(false, 2, "You defend, gaining " + Math.Round((gameStats["Agility"] / 10), 1) + " Agility.");
            if (revealedEnemy) { txtDisplay.UpdateText(true, 2, "\nThe enemy defends, gaining " + Math.Round((enemy["MAgility"] / 10), 1) + " Agility."); }
        }

        if (playerChoice == "defend" && enemyChoice == "attack")
        {
            double damageDealt = Math.Max(enemy["Agility"] + enemy["Strength"] - (gameStats["BDefense"] + (gameStats["BAgility"] + gameStats["BStrength"]) / 4),0);
            double enemyAgilityLost = (gameStats["BDefense"] + enemy["Strength"] / enemy["MAgility"]);

            gameStats["Health"] -= damageDealt;
            gameStats["BAgility"] += (gameStats["Agility"] / Math.Max(damageDealt,1) / 2);
            enemy["Agility"] -= enemyAgilityLost;

            txtDisplay.UpdateText(false, 2, "The enemy attacks you for " + Math.Round(damageDealt, 1) + " Health.");
            if (revealedEnemy) { txtDisplay.UpdateText(true, 2, "\nThe enemy loses " + Math.Round(enemyAgilityLost, 1) + " Agility."); }
            txtDisplay.UpdateText(true, 2, "\nYou defend, gaining " + Math.Round((gameStats["Agility"] / Math.Max(damageDealt, 1) / 2), 1) + " Agility.");
        }

        if (playerChoice == "attack" && enemyChoice == "defend")
        {
            double damageDealt = Math.Max(gameStats["BAgility"] + gameStats["BStrength"] - (enemy["Defense"] + (enemy["Agility"] + enemy["Strength"]) / 10), 0);
            double agilityLost = (enemy["Defense"] + gameStats["BStrength"] / gameStats["Agility"]);

            enemy["Health"] -= damageDealt;
            enemy["Agility"] += (enemy["MAgility"] / Math.Max(damageDealt,1) / 2);
            gameStats["BAgility"] -= agilityLost;

            enemy["Defense"] -= enemy["Defense"] / gameStats["BStrength"];

            txtDisplay.UpdateText(false, 2, "You attack the enemy for " + Math.Round(damageDealt, 1) + " Health.");
            txtDisplay.UpdateText(true, 2, "\nYou lose " + Math.Round(agilityLost, 1) + " Agility.");
            if (revealedEnemy) { txtDisplay.UpdateText(true, 2, "\nThe enemy defends, gaining " + Math.Round((enemy["MAgility"] / Math.Max(damageDealt, 1) / 2), 1) + " Agility."); }
        }

        enemy["Agility"] = Math.Max(enemy["Agility"], 0);
        gameStats["BAgility"] = Math.Max(gameStats["BAgility"], 0);
        enemy["Agility"] = Math.Min(enemy["Agility"], enemy["MAgility"]);
        gameStats["BAgility"] = Math.Min(gameStats["BAgility"], gameStats["Agility"]);

        if (gameStats["Health"] <= 0)
        {
            resetGame();
            txtDisplay.UpdateText(true, 2, "\nAh Fuck I've been Killed.");
            return;
        }

        if (enemy["Health"] <= 0)
        {
            inBattle = false;
            double xpGained = enemy["Strength"] + enemy["MAgility"] / (enemy["Defense"] / 5);
            txtDisplay.UpdateText(true, 2, "\nYou beat the enemy and get " + Math.Round(xpGained, 1) + " XP.");
            bool leveled = false;
            gameStats["XP"] += xpGained;
            while (gameStats["XP"] > (10 * Math.Pow(1.5, gameStats["Level"]))) //so you can level up multiple times juuuust in case
            {
                gameStats["Level"]++;
                gameStats["Points"] += 10;
                gameStats["Health"] += 45;
                leveled = true;
            }
            if (leveled)
            {
                pointSelect = true;
                txtDisplay.UpdateText(true, 2, "\nYou've levelled up to level " + gameStats["Level"] + "!");
                Step();
            } else {
                mapRoaming = true;
                Step();
                txtDisplay.UpdateText(false, 2, "You beat the enemy and get " + Math.Round(xpGained, 1) + " XP.");
            }

        } else { Step(); }
        
    }
    void Step()
    {
        if (inBattle)
        {
            noPotions = false;
            tooTired = false;
            txtDisplay.UpdateText(false, 0, "A - Defend\nS - Attack\nD - Item");
            txtDisplay.UpdateText(false, 1, "Your Stats:\nDefense - " + gameStats["BDefense"]);
            txtDisplay.UpdateText(true, 1, "\nStrength - " + gameStats["BStrength"]);
            txtDisplay.UpdateText(true, 1, "\nAgility - " + Math.Round(gameStats["BAgility"], 1));
            txtDisplay.UpdateText(true, 1, "\nHealth - " + Math.Round(gameStats["Health"], 1));
            if (revealedEnemy) { txtDisplay.UpdateText(true, 1, "\nEnemy Stats:\nDefense - " + Math.Floor(enemy["Defense"]) + "\nStrength - " + enemy["Strength"] + "\nAgility - " + Math.Round(enemy["Agility"], 1)); }
            //txtDisplay.UpdateText(true, 2, "\nFighting a level " + enemy["Level"] + " enemy.");
            txtDisplay.UpdateText(true, 2, "\nEnemy has " + Math.Ceiling(enemy["Health"]) + " health.");
        }
        if (pointSelect)
        {
            txtDisplay.UpdateText(false, 0, "Z - Defense - " + gameStats["Defense"] + "\nX - Strength - " + gameStats["Strength"] + "\nC - Agility - " + gameStats["Agility"]);
            txtDisplay.UpdateText(false, 1, "Upgrade your stats, dummy.\nPoints remaining: " + gameStats["Points"]);
            return;
        }
        if (mapRoaming)
        {
            X = pos % 10;
            Y = pos / 10;

            txtDisplay.UpdateText(false, 0, 
                    "Floor: " + floorLevel + 
                    "\nX: " + X + 
                    "\nY: " + Y + 
                    "\nHP: " + Math.Ceiling(gameStats["Health"]) + 
                    "\nLevel: " + gameStats["Level"] + " | XP: " + Math.Ceiling(gameStats["XP"]) + "/" + Math.Ceiling((10 * Math.Pow(1.5, gameStats["Level"]))) + 
                    "\nScore: " + gameStats["Score"]
                    );
            if (gameInventory["RevealRoom"] > 0) { txtDisplay.UpdateText(true, 0, "\n\n\nSpace:\nRoom Reveals: " + gameInventory["RevealRoom"]); }
            txtDisplay.UpdateText(false, 1, floorGen.displayFloor(pos, roomRevealed));
            txtDisplay.UpdateText(false, 2, "");

            switch (gameFloor[pos])
            {
                case 0: //empty
                    break;
                case 1: //player spawn
                    break;
                case 2: //item
                    int rand = UnityEngine.Random.Range(0, potionTypes.Count);
                    txtDisplay.UpdateText(true, 2, "Picked up.. " + potionTypes[rand]  + "!\n");
                    gameInventory[potionTypes[rand]]++;
                    gameFloor[pos] = 0;
                    //if (gameInventory["RevealRoom"] == 1) { txtDisplay.UpdateText(true, 0, "\n\n\nRoom Reveals: 1"); }
                    break;
                case 3: //enemy
                    mapRoaming = false;
                    inBattle = true;
                    noPotions = false;
                    tooTired = false;
                    gameStats["BDefense"] = gameStats["Defense"];
                    gameStats["BStrength"] = gameStats["Strength"];
                    gameStats["BAgility"] = gameStats["Agility"];
                    txtDisplay.UpdateText(false, 0, "A - Defend\nS - Attack\nD - Item");
                    txtDisplay.UpdateText(false, 1, "Your Stats:\nDefense - " + gameStats["BDefense"]);
                    txtDisplay.UpdateText(true, 1, "\nStrength - " + gameStats["BStrength"]);
                    txtDisplay.UpdateText(true, 1, "\nAgility - " + Math.Round(gameStats["BAgility"], 1));
                    txtDisplay.UpdateText(true, 1, "\nHealth - " + Math.Round(gameStats["Health"], 1));
                    txtDisplay.UpdateText(false, 2, "You've encountered an enemy!");
                    enemy = enemyGen.generateEnemy();
                    txtDisplay.UpdateText(true, 2, "\nA level " + enemy["Level"] + " enemy approaches!");
                    txtDisplay.UpdateText(true, 2, "\nEnemy has " + enemy["Health"] + " health.");
                    revealedEnemy = false;
                    gameFloor[pos] = 0;
                    break;
                case 4: //exit
                    globalDifficulty = Math.Min(0.9, globalDifficulty + (UnityEngine.Random.Range(1, 100) / 1000));
                    gameStats["Score"] = (10000 * globalDifficulty) - (movesTakenMAP*10) - (movesTakenFIGHT*3);
                    movesTakenMAP = 0;
                    movesTakenFIGHT = 0;
                    pos = 35;
                    gameFloor = floorGen.GenerateFloor(floorLevel+1, globalDifficulty);
                    roomRevealed = false;
                    Step();
                    break;
            }
            return;
        }



    }
}
