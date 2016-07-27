using UnityEngine;
using System.Collections;

namespace Completed
{
    using System.Collections.Generic;       //Allows us to use Lists. 

    public class GameManager : MonoBehaviour
    {

        public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
        private LevelManager levelScript;                       //Store a reference to our LevelManager which will set up the level.
        private int level = 1;                                  //Current level number, expressed in game as "Day 1".

        //Awake is always called before any Start functions
        void Awake()
        {
            //Check if instance already exists
            if (instance == null)
                instance = this;                                //if not, set instance to this

            //If instance already exists and it's not this:
            else if (instance != this)
                Destroy(gameObject);                            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
             
            DontDestroyOnLoad(gameObject);                      //Sets this to not be destroyed when reloading scene

            //Get a component reference to the attached BoardManager script
            levelScript = GetComponent<LevelManager>();
            InitGame();                                         //Call the InitGame function to initialize the first level 
        }

        //Initializes the game for each level.
        void InitGame()
        {
            //Call the SetupScene function of the BoardManager script, pass it current level number.
            levelScript.SetupScene(level);

        }



        //Update is called every frame.
        void Update()
        {

        }
    }
}