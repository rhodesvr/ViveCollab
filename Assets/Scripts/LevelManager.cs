using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;                   //Allows us to use Lists 
using Random = UnityEngine.Random;                  //Tells Random to use the Unity Engine random number generator

public class LevelManager : MonoBehaviour {


    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        //assignment constructor so we can set values when declare new Count
        public Count (int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    // dimensions of our game board 
    public int columns = 8;                         //Number of columns in out game board
    public int rows = 8;                            //Number of rows in our game board
    public Count wallCount = new Count(5, 9);       //Lower and upper limit for our random number of walls per level
    public Count foodCount = new Count(1, 5);       //Lower and upper limit for our random number of food items per level

    public GameObject exit;                         //Prefab to spawn for exit.
    public GameObject[] floorTiles;                 //Array of floor prefabs.
    public GameObject[] wallTiles;                  //Array of wall prefabs
    public GameObject[] foodTiles;                  //Array of food prefabs
    public GameObject[] enemyTiles;                 //Array of enemy prefabs
    public GameObject[] outerWallTiles;             //Array of outer tile prefabs


    private Transform boardHolder;                  //A variable to store a reference to the transform of our Board object
                                                    //to keep the hierarchy clean. make board holder parent of game objects (collapses)
    private List<Vector3> gridPositions = new List<Vector3>();   //A list of possible locations to place tiles.
    // to track all potential positions on our game board and to keep track of whether an object has been spawned in that position or not


   //Clears our list gridPositions and prepares it to generate a new board.
   void InitialiseList ()
    {
        //Clear our list gridPositions.
        gridPositions.Clear();

        //Loop through x axis (columns).
        for (int x = 1; x < columns-1; x++)
        {
            //Within each column, loop through y axis (rows).
            for(int y = 1; y < rows - 1; y++)
            {
                //At each index add a new Vector3 to our list with the x and y coordinates of that position.
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }

    }

    //Sets up the outer walls and floor (background of the game board.
    void BoardSetup ()
    {
        //Instantiate Board and set boardHolder to its transform.
        boardHolder = new GameObject("Board").transform;

        //Loop along x axis, starting from -1 (to fill corner) with floor or outerwall edge tiles.
        for(int x = -1; x < columns + 1; x++)
        {
            //Loop along y axis, starting from -1 to place floor or outerwall tiles.
            for (int y = -1; y < rows + 1; y++)
            {
                //Choose a random tile from our array of floor tiles prefabs and prepare to instantiate it.
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];

                //Check if current position is at board edge, if so choose a random outer wall prefab from our array of outer wall tiles.
                if (x == -1 || x == columns || y == -1 || y == rows)
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];

                //Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
                GameObject instance =
                    Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                //Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    //RandomPosition returns a random position from our list gridPositions.
    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);         //grab a random index from the exisiting positions list
        Vector3 randomPosition = gridPositions[randomIndex];            //grab the positions
               
        gridPositions.RemoveAt(randomIndex);                            //remove entry at randomIndex from the list so that it can't be reused
        return randomPosition;
    }

    void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);

        for(int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];

            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    //SetupScene initializes our level and calls the previous function to lay out the game board.
    public void SetupScene(int level)
    {

        //Creates the outer walls and floor
        BoardSetup();
        //Reset our list of grid positions
        InitialiseList();

        //Instantiate a random number of wall tiles based on minimum and maximum, at randomized positions.
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);

        //Determine number of enemies based on current level number, based on a logarithmic progression
        int enemyCount = (int)Mathf.Log(level, 2f);             //NEATO

        //Instantiate the exit tile in the upper right hand corner of our game board
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
    }
}
