using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : NetworkBehaviour
{

    public GameObject enemyPrefab;
    public int numEnemies;
    public GameObject[] collectArray;
    public Vector3[] spawnGrid; //2D array for pos 

    // OnStartServer is called is called on ther server when the server starts listening to the network
    // it's a virtual func available from the NetworkBehavious base class
        
    public override void OnStartServer()
    {
        //when we are spawned on the client
        if (isLocalPlayer)
        {
            GameObject playerPrefab = GameObject.FindGameObjectWithTag("Player"); //bad idea
            DataCollection dataMod = playerPrefab.GetComponent<DataCollection>();
            if (dataMod != null)
                numEnemies = dataMod.numEnemies;
            Debug.Log("numEnemies in Spawner: " + numEnemies);

        }

        //CmdSpawnEnemies();

        //collectArray[0].SetActive(true);     

        //var hitPlayer = GetComponent<PlayerMove>();
        //var dataMod = GetComponent<DataCollection>();
    }




        void Update()
    {
        if (!isLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            //Debug.Log("CLICKED");            
            CmdSpawnEnemies();

        }
    }


    //object must have authority if to spawn
    //This [Command] code is run on the server! See More: Networked Actions
    [Command]
    public void CmdSpawnEnemies()
    {        
        numEnemies = 4;
        //collectArray = new GameObject[numEnemies];
        //spawnGrid = new Vector3[numEnemies];

        int randTurn = randomizeTurn();
        float xpos, zpos, yrot; // probs need floats
        xpos = 0.0f;
        zpos = 0.0f;
        yrot = 0.0f;

        for (int i = 0; i < numEnemies; i++)
        {
            randomizePosition(ref xpos, ref zpos, i, randTurn);
            yrot = Random.Range(0, 360);

            GameObject enemy = Instantiate(enemyPrefab, new Vector3(xpos * 1.5f, 0, zpos), Quaternion.Euler(270, yrot, 0)) as GameObject; // cast as gameObject        
            NetworkServer.Spawn(enemy);
            //NetworkServer.SpawnWithClientAuthority(enemy, connectionToClient);


            //collectArray[i] = enemy;
            //spawnGrid[i] = new Vector3(xpos, 0f, zpos);
            //collectArray[i].SetActive(false);
        }
    }







        void randomizePosition(ref float xpos, ref float zpos, int i, int randTurn)
    {
        if (i % 2 == 0) // even nums on +x side
        {
            xpos = Random.Range(-0.5f, -2.0f);
            zpos = Random.Range(-2.0f, 2.0f);
        }
        else // odd nums on -x side
        {
            xpos = Random.Range(0.5f, 2.0f);
            zpos = Random.Range(-2.0f, 2.0f);
        }
        xpos *= randTurn;
        zpos *= randTurn;
    }


    int randomizeTurn()
    {
        int randTurn = Random.Range(0, 100);
        if (randTurn >= 50)        
            return randTurn = 1;        
        else        
            return randTurn = -1;
        
    }


}
