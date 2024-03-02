using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FLOOR_Main : MonoBehaviour
{
    public TEXT_Display gameDisplay;

    Dictionary<int, int> cachedFloor = new Dictionary<int, int>();
    //FlooXY, Floor Contents
    // Start is called before the first frame update
    // Difficulty determines the rate of a monster encounter compared to items or empty
    // the chance to come across an empty room is always 1/3 rooms

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal String displayFloor(int position)
    {
        //int X = position % 10;
        //int Y = position / 10; 
        // ? is padding
        // ? is player
        //   is empty room
        // × is exit
        // ? is an encounter

        //we know our rooms will always be 9 x 8 so we only need to worry about that

        string map = "<mspace=0.75em>";

        for (int y = 8; y >= 1; y--)
        {
            for (int x = 1; x <= 9; x++)
            {
                string res = "";
                if (cachedFloor.ContainsKey((y * 10) + x))
                {
                    switch (cachedFloor[(y * 10) + x])
                    {
                        case 0:
                            res = " ";
                            break;
                        case 1:
                            res = "_";
                            break;
                        case 2:
                        case 3:
                            res = "?";
                            break;
                        case 4:
                            res = "X";
                            break;
                        default:
                            break;
                    }
                    if ((y * 10) + x == position) { res = "¤"; }
                    map = map + res;
                }
                else
                {
                    map = map + "#";
                }

            }
            map = map + ".";
            if (y > 1) { map = map + "\n"; }
        }

        return map + "</mspace>";
    }

    internal Dictionary<int, int> GenerateFloor(int level, double diff)
    {
        cachedFloor.Clear();

        // 0 = empty room
        // 1 = player spawn
        // 2 = item spawn
        // 3 = enemy spawn
        // 4 = exit

        cachedFloor.Add(35, 1);

        int roomsLeft = ((int)(level * 2.6f) + UnityEngine.Random.Range(0, 2) + 5);

        //this.GetComponent<TEXT_Display>().UpdateText(true, true, "Room Count set to:" + roomsLeft + "\n");
        int thingsLeft = (int)Math.Ceiling((float)roomsLeft / 3);
        //int emptyRoomsLeft = (int)Math.Floor((float)roomsLeft / 3);

        List<int> trackedFloors = new List<int>();
        List<int> possibleDeadEnds = new List<int>();

        trackedFloors.Add(35);

        //NOW TO START GENERATING YIPPEE

        List<int> adjacent = new() { 10, -10, 1, -1 };

        int DEBUG_NumAttempts = 400;
        bool restartedGen = false;
        while (roomsLeft > 0)
        {
            DEBUG_NumAttempts--;

            if (DEBUG_NumAttempts == 0)
            {
                gameDisplay.UpdateText(true, 1, "LIMIT HIT");
                break; //todo, try and restart the function if hit
            }

            //int checkFloor = trackedFloors[0] + adjacent[UnityEngine.Random.Range(0, 3)];
            int checkFloor = 0;
            for (int j = 0; j <= 3; j++)
            {
                if (roomsLeft <= 0) { break; }

                if (trackedFloors.Count == 0) { 
                    if (restartedGen)
                    {
                        print("Impossible layout found, regenerating..");
                        return GenerateFloor(level, diff);
                    }
                    else{
                        trackedFloors.Add(35);
                        restartedGen = true;
                    }
                } //restart from starting room if cant generate any more rooms

                checkFloor = trackedFloors[0] + adjacent[j];

                if (checkFloor <= 10 || checkFloor > 89 || (checkFloor % 10 == 0)) { continue; } //check if room is out of bounds
                if (cachedFloor.ContainsKey(checkFloor)) { continue; } //check if room already exists
                bool checkFailed = false;
                for (int i = 0; i <= 3; i++)
                {
                    int numNeighbors = 0;
                    if (cachedFloor.ContainsKey(checkFloor + adjacent[i])) { numNeighbors++; }
                    if (numNeighbors == 2) { checkFailed = true; break; }
                }
                if (checkFailed) { continue; } //check if room has 2 or more neighbors
                if (UnityEngine.Random.Range(0, 2) == 1) { continue; } //give up on random

                //now we have found a good room to generate 
                //print("found room slot: " + checkFloor);
                //this.GetComponent<TEXT_Display>().UpdateText(true, true, "\nfound room slot: " + checkFloor);
                trackedFloors.Add(checkFloor);
                possibleDeadEnds.Add(checkFloor);
                cachedFloor.Add(checkFloor, 0);
                roomsLeft--;
            }
            trackedFloors.RemoveAt(0);
            //break;
        }
        //Room generation complete, now check for dead ends to make into exits
        trackedFloors.Clear();
        for (int j = 0; j < possibleDeadEnds.Count; j++)
        {
            int numNeighbors = 0;
            int checkFloor = possibleDeadEnds[j];
            for (int i = 0; i <= 3; i++)
            {
                //print(i);
                //exit cannot be adjacent to entry
                if (checkFloor == 36 || checkFloor == 34 || checkFloor == 25 || checkFloor == 45) { break; }
                if (cachedFloor.ContainsKey(checkFloor + adjacent[i])) { numNeighbors++; }

            }
            if (numNeighbors == 1)
            {
                //this.GetComponent<TEXT_Display>().UpdateText(true, true, "\nfound possible dead end: " + checkFloor);
                trackedFloors.Add(checkFloor);
            }
        }
        //pick a random dead end to be made into an exit

        //No dead ends? reset the whole thing
        if (trackedFloors.Count == 0)
        {
            print("Impossible layout found, regenerating..");
            return GenerateFloor(level, diff);
        }

        cachedFloor[trackedFloors[UnityEngine.Random.Range(0, trackedFloors.Count)]] = 4;
        //randomize the rooms contents
        //for (int j = 0; j < possibleDeadEnds.Count; j++) //go through all created rooms
        //{
        //    int checkFloor = cachedFloor[possibleDeadEnds[j]];

        //    if (checkFloor != 0) { continue; } //if a room already has stuff, skip it

        //}

        int ENLeft = (int)Math.Ceiling((float)thingsLeft * diff);

        while (thingsLeft > 0)
        {
            // https://stackoverflow.com/questions/13457917/random-iteration-in-for-loop
            // Lifting code always works if you know exactly what you're looking for ;)
            //for (int j = 0; j < possibleDeadEnds.Count; j++) //go through all created rooms
            System.Random r = new System.Random();
            foreach (int i in Enumerable.Range(0, possibleDeadEnds.Count).OrderBy(x => r.Next()))
            {
                if (thingsLeft == 0) { break; }

                if (cachedFloor[possibleDeadEnds[i]] != 0) { continue; } //if a room already has stuff, skip it

                cachedFloor[possibleDeadEnds[i]] = ((ENLeft > 0) ? 3 : 2); //ternary ftw
                ENLeft--;
                thingsLeft--;


                //difficulty will determine the enemy/item ratio, 100 will always give an enemy, 0 will always give an item
                //this still has a chance to create rooms of only enemies or only items, so higher difficulty numbers are reccomended



            }
        }

        //floorLevel++;

        return cachedFloor;
    }
}
