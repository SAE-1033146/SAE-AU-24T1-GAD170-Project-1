using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BATTLE_Enemy : MonoBehaviour
{
    public Dictionary<string, double> enemyStats = new Dictionary<string, double>();
    List<string> upgradables = new() { "Defense", "Strength", "Agility" };
    string lastHit = "";
    bool firstMove = false;
    internal Dictionary<string,double> generateEnemy()
    {
        enemyStats["Health"] = 20 * this.GetComponent<MainGame>().floorLevel;
        enemyStats["Defense"] = 10;
        enemyStats["Strength"] = 10;
        enemyStats["Agility"] = 10;
        
        enemyStats["Level"] = UnityEngine.Random.Range(this.GetComponent<MainGame>().floorLevel, this.GetComponent<MainGame>().floorLevel*2);
        for (int i = 0; i < (20 + UnityEngine.Random.Range(this.GetComponent<MainGame>().floorLevel * 15, this.GetComponent<MainGame>().floorLevel * 25)); i++)
        {
            enemyStats[upgradables[UnityEngine.Random.Range(0, 3)]]++;
        }
        enemyStats["MAgility"] = enemyStats["Agility"];
        lastHit = "";
        firstMove = true;
        return enemyStats;
    }

    internal string BATTLE_AI(string move) 
    {
        string choice = "";
        if (firstMove)
        {
            if (enemyStats["Strength"] >= enemyStats["Defense"]) { choice = "attack"; } else { choice = "defend"; }
            firstMove = false;
        } else {
            if (lastHit == "attack") { choice = ((UnityEngine.Random.Range(0, 100) > 70) ? "attack" : "defend"); }
            if (lastHit == "defend") { choice = ((UnityEngine.Random.Range(0, 100) > 70) ? "defend" : "attack"); }
            if (enemyStats["Agility"] < (enemyStats["Strength"] / enemyStats["MAgility"])) { choice = "defend"; } //defend if agility is too low
        }
        lastHit = move;
        return choice;
    }
}
