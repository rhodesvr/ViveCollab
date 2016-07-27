using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class PlayerMove : NetworkBehaviour
{

    //https://docs.unity3d.com/Manual/UNetSetup.html 
    public GameObject bulletPrefab;

    [SyncVar]
    public GameObject enemyPrefab;
    //[SyncVar]
    public GameObject[] enemyArray;

    public GameObject enemy2Prefab;
    GameObject tmpEnemy; 


    public int numEnemies;

    private int numErr;
    //public int enemyCount;

    [SyncVar]
    public int enemyCount = 0;

    private bool playerInArea;
    private NetworkIdentity objNetId;

    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.magenta;
        playerInArea = false;
        numErr = 0;
        //enemyCount = 0;
    }

    void Start()
    {
        playerInArea = false;
    }

        // Update is called once per frame
        void Update()
    {
        if (!isLocalPlayer)
            return;

        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;
        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);
    

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Command function is called from the client, but invoked on the server
            CmdFire();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("CLICKED");            
            CmdPickUp();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            CmdSpawnEnemies();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            CmdMakeEnemy2();
        }
    }


    [Command]
    void CmdFire()
    {
        //This [Command] code is run on the server! See More: Networked Actions
        // create the bullet object locally.
        var bullet = (GameObject)Instantiate(
            bulletPrefab,
            transform.position - transform.forward,
            Quaternion.identity);

        // make the bullet move away in front of the player
        bullet.GetComponent<Rigidbody>().velocity = -transform.forward * 4;
        // spawn bullet on the clients
        NetworkServer.Spawn(bullet);
        // make bullet disappear after 3 seconds
        Destroy(bullet, 3.0f);

    }

    [Command]
    void CmdPickUp()
    {        
        var dataMod = GetComponent<DataCollection>();
        //int count = dataMod.count;
        //var enemyCount = GetComponent<DataCollection>().count;

        if (enemyCount < numEnemies)
        {
            if (playerInArea == true)
            {
                //enemyArray[count].SetActive(false);
                //RpcHide(enemyCount);
                var enemyScrip = GetComponent<EnemyController>();
                //enemyScrip.DestroySelf();
                Destroy(tmpEnemy);
                
                //dataMod.PickUp(true);   //modify count and round accordingly

                //dataMod.count += 1;
                //enemyCount = dataMod.count;
                //enemyCount += 1;
                CmdAddCount();
                //GetComponent<DataCollection>().count = enemyCount;
                Debug.Log("COUNT " + enemyCount + "AND INAREA = " + playerInArea);
                playerInArea = false;

                if (enemyCount < numEnemies)
                    CmdMakeEnemy2();
                    //RpcShow(enemyCount);
                    //enemyArray[count].SetActive(true);                
            }
            else
            {
                //ERR               
                //Vector3 collectPos = enemyArray[count].transform.position;
                //Debug.Log("before dist comp, count is:" + count);
                //Vector3 enemyPos = dataMod.enemyList[count].enemy.transform.position;
                //float errDist = Vector3.Distance(transform.position, enemyPos);
                numErr += 1;
                //Debug.Log("err #" + numErr + "; dist = " + errDist);
            }
        }
    }

    [Command]
    public void CmdAddCount()
    {
        if (!isServer)
            return;
        enemyCount += 1;
    }


    //object must have authority if to spawn
    //This [Command] code is run on the server! See More: Networked Actions
    [Command]
    void CmdSpawnEnemies()
    {
        if (!isLocalPlayer)
            return;

        //numEnemies = 4;
        //enemyArray = new GameObject[numEnemies];

        //spawnGrid = new Vector3[numEnemies];

        int randTurn = randomizeTurn();
        float xpos, zpos, yrot; // probs need floats
        xpos = 0.0f;
        zpos = 0.0f;
        yrot = 0.0f;

        var dataMod = GetComponent<DataCollection>();

        for (int i = 0; i < numEnemies; i++)
        {
            Debug.Log("spawn i: " + i);
            randomizePosition(ref xpos, ref zpos, i, randTurn);
            yrot = Random.Range(0, 360);

            GameObject enemy = Instantiate(enemyPrefab, new Vector3(xpos * 1.5f, 0, zpos), Quaternion.Euler(270, yrot, 0)) as GameObject; // cast as gameObject                   
                                                                                                                                          //dataMod.AddEnemy(enemy);
                                                                                                                                          
            CmdAdd(i, enemy);
            //dataMod.AddEnemy(enemy);

            NetworkServer.Spawn(enemy);
            //this.enemyPrefab = enemy;

            if (dataMod.enemyList == null)
                Debug.Log("Enemy List is EMPTY");
            RpcHide(i);
            //RpcShow(i);

        }

        RpcShow(0);

    }

     


    [Command]
    void CmdAdd(int count, GameObject enemyObj)
    {
        var dataMod = GetComponent<DataCollection>();
        Debug.Log("add item #: " + count);
        dataMod.AddEnemy(enemyObj);
        //enemyArray[count].SetActive(true);
    }


    [ClientRpc]
    void RpcHide(int count)
    {
        var dataMod = GetComponent<DataCollection>();
        Debug.Log("Hide item #: " + count);
        dataMod.HideEnemy(count);
        //enemyArray[count].SetActive(false);        
    }

    [ClientRpc]
    void RpcShow(int count)
    {
        var dataMod = GetComponent<DataCollection>();
        Debug.Log("show item #: " + count);
        dataMod.ShowEnemy(count);
        //enemyArray[count].SetActive(true);
    }


    [Command]
    void CmdMakeEnemy2()
    {
        float xpos, zpos;// yrot; // probs need floats
        xpos = 0.0f;
        zpos = 0.0f;
        //yrot = 0.0f;

        int randTurn = randomizeTurn(); //synvar? returns 1 or -1

        //var hit = col.gameObject;
        //var enemyCount = GetComponent<DataCollection>().count;
        randomizePosition(ref xpos, ref zpos, enemyCount, randTurn);
        //yrot = Random.Range(0, 360);

        var enemy2 = (GameObject)Instantiate(
            enemy2Prefab,
            new Vector3(xpos * 1.5f, 0, zpos),
             Quaternion.identity);


        var enemyScrip = enemy2.GetComponent<EnemyController>();
        enemyScrip.UpdateTurn(randTurn);

        NetworkServer.Spawn(enemy2);
        //enemyCount += 1;
        Debug.Log("Enemy count: " + enemyCount);
        //this.enemy2 = enemy;

    }




    void randomizePosition(ref float xpos, ref float zpos, int count, int randTurn)
    {
        if (count % 2 == 0) // even nums on +x side
        {
            xpos = Random.Range(-0.5f, -2.0f);
            zpos = Random.Range(-2.0f, 2.0f);
        }
        else // odd nums on -x side
        {
            xpos = Random.Range(0.5f, 2.0f);
            zpos = Random.Range(-2.0f, 2.0f);
        }
        Debug.Log("Turn: " + randTurn);
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




    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            playerInArea = true;
            Debug.Log("INAREA");
            
            tmpEnemy = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            playerInArea = false;
            Debug.Log("OUT");
        }

    }


}
