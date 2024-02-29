using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FLOOR_Main : MonoBehaviour
{
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

    internal Dictionary<int,int> GenerateFloor(int level, int diff)
    {
        cachedFloor.Clear();

        // 0 = no room
        // 1 = player spawn
        // 2 = item spawn
        // 3 = enemy spawn
        // 4 = exit

        cachedFloor.Add(35, 1);

        int roomsLeft = ((int)(level * 2.6f) + UnityEngine.Random.Range(0, 2) + 5);

        //this.GetComponent<TEXT_Display>().UpdateText(true, true, "Room Count set to:" + roomsLeft + "\n");
        int thingsLeft = (int)Math.Ceiling((float)roomsLeft * 2 / 3);
        int emptyRoomsLeft = (int)Math.Floor((float)roomsLeft / 3);

        List<int> trackedFloors = new List<int>();
        List<int> possibleDeadEnds = new List<int>();

        trackedFloors.Add(35);

        //NOW TO START GENERATING YIPPEE

        List<int> adjacent = new List<int>{ 10, -10, 1, -1 };

        int DEBUG_NumAttempts = 400;

        while (roomsLeft > 0)
        {
            DEBUG_NumAttempts--;

            if (DEBUG_NumAttempts == 0)
            {
                this.GetComponent<TEXT_Display>().UpdateText(true, true, "LIMIT HIT");
                break;
            }

            //int checkFloor = trackedFloors[0] + adjacent[UnityEngine.Random.Range(0, 3)];
            int checkFloor = 0;
            for (int j= 0 ; j <= 3;  j++)
            {
                if (roomsLeft == 0) { break; }

                checkFloor = trackedFloors[0] + adjacent[j];

                if (checkFloor < 1 || checkFloor > 79) { continue; } //check if room is out of bounds
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
                if (cachedFloor.ContainsKey(checkFloor + adjacent[i])) { numNeighbors++; }
                if (checkFloor + adjacent[i] == 35) { break; } //exit cannot be adjacent to entry
            }
            if (numNeighbors == 1)
            {
                //this.GetComponent<TEXT_Display>().UpdateText(true, true, "\nfound possible dead end: " + checkFloor);
                trackedFloors.Add(checkFloor);
            }
        }
        //pick a random dead end to be made into an exit

        cachedFloor[trackedFloors[UnityEngine.Random.Range(0, trackedFloors.Count)]] = 4;
        //randomize the rooms contents
        return cachedFloor;
    }
}
