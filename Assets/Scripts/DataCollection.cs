using UnityEngine;
using UnityEngine.Networking;
using System.Collections;


public struct Enemy
{
    // .Count is a built in member
    public GameObject enemy;

}
public class EnemyList : SyncListStruct<Enemy> { }


public class DataCollection : NetworkBehaviour {


    public bool destroyOnDeath;
    public const int maxHealth = 100;
    public int numEnemies = 4;                   // Change this later.
    public const int numRounds = 5;

    private NetworkStartPosition[] spawnPoints;
    //public GameObject[] enemyArray;

    [SyncVar]
    public int currentHealth = maxHealth;
    [SyncVar]
    public int count = 0;
    [SyncVar]
    public int round = 0;
    // Use this for initialization

    public EnemyList enemyList = new EnemyList();






    void Start()
    {
        if (isLocalPlayer)
        {
            spawnPoints = FindObjectsOfType<NetworkStartPosition>();
            //grab number of enemies from EnemySpawner
        }
    }


    //public override void OnStartClient()
    //{
    //    enemyList.Callback = OnIntChanged;
    //}

    //private void OnIntChanged(SyncListInt.Operation op, int index)
    //{
    //    Debug.Log("list changed " + op);
    //}

   
    public void AddEnemy(GameObject enemyObj)
    {
        //if (!isServer)
        //    return;


        var en = new Enemy();
        en.enemy = enemyObj;
        enemyList.Add(en);

        if (enemyList == null)
            Debug.Log("Yer fucked. List is empty on server code");
        if (enemyObj = null)
            Debug.Log("enemyObj is also empty");
    }

    public int GetIndex(GameObject enemyObj)
    {
        var en = new Enemy();
        en.enemy = enemyObj;
        
        return enemyList.IndexOf(en);
    }

    public void HideEnemy(int count)
    {

        enemyList[count].enemy.SetActive(false);

    }
    
    public void ShowEnemy(int count)
    {

        enemyList[count].enemy.SetActive(true);

    }

    public void PickUp(bool success)
    {
        // Add check so that damage will only be applied on the server.
        if (!isServer)
        {
            Debug.Log("PICKUP YALL" + count);
            return;
        }

        if (success)
        {
            count += 1;
            round += 1;
            if (round > numRounds)
                Debug.Log("Game Over.");
        }

       

    }


    public void TakeDamage(int amount)
    {
        if (!isServer)      // add check so that damage will only be applied on ther Server
        {
            Debug.Log("curHealth: " + currentHealth);
            return;
        }

        currentHealth -= amount;
        Debug.Log("cur health: " + currentHealth);
        if (currentHealth <= 0)
        {
            if (destroyOnDeath)
            {
                Destroy(gameObject);
            }
            else
            {
                currentHealth = maxHealth;
                RpcRespawn(); //Called on the server but invoked on the client
            }
        }

    }

    [ClientRpc]
    void RpcSpawnBits()
    {


    }

    // Commands are called on tlhe Client, but executed on the Server.
    // ClientRpcs are called on the Server, but executed on the Client.
    [ClientRpc]
    void RpcRespawn()
    {
        if (isLocalPlayer)
        {

            Vector3 spawnPoint = Vector3.zero;
            //if there is a spwan point array and the array is not empty, pick one at random
            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
            }
            transform.position = spawnPoint; 

        }
    }


}
