using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class Collection : MonoBehaviour {

    public int numCollectibles;
    public int speed;
    public GameObject collectible;
    public GameObject[] collectArray;
    public Vector3[] spawnGrid; //2D array for pos 

    private int roundCount;
    private int count;
    private int numErr;
    bool nextItemFlag;
    private bool turn;
    //private bool click;
    private bool playerInArea; 

    private float distance; 
    private Vector3 previousPosition; 

    private bool startTime;
    private float curTime; 
    private float endTime;

    //private AudioClip clickSound;
    //private AudioSource source;

    public float sensX = 100.0f;
    float rotationX = 0.0f;

    // Use this for initialization
    void Start()
    {

        startTime = true; // set this to false after trigger is set 
        //click = false;
        playerInArea = false;   //might want to put in Awake
        curTime = 0;
        endTime = 999.0f;

        distance = 0f; 


        float xpos, zpos, yrot; // probs need floats
        int randTurn = randomizeTurn();

        xpos = 0.0f;
        zpos = 0.0f;
        count = 0;
        numErr = 0;
        collectArray = new GameObject[numCollectibles];
        spawnGrid = new Vector3[numCollectibles];
        //source = GetComponent<AudioSource>();


        for (int i = 0; i < numCollectibles; i++)
        {
            // make sure they don't overlap and that they aren't in the same spots?
            randomizePosition(ref xpos, ref zpos, i, randTurn);
            yrot = Random.Range(0, 360);

            GameObject spawn = Instantiate(collectible, new Vector3(xpos * 1.5f, 0, zpos), Quaternion.Euler(270, yrot, 0)) as GameObject; // cast as gameObject        
            collectArray[i] = spawn;
            spawnGrid[i] = new Vector3(xpos, 0f, zpos);
            collectArray[i].SetActive(false);
        }
        collectArray[0].SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {

        distance += Vector3.Distance(transform.position, previousPosition);
        previousPosition = transform.position;

        if (startTime) //trigger this!
        {
            curTime += Time.deltaTime;

            if (count >= numCollectibles)
            {
                //Finish round! put out data
                //reset game
                // start again
                //until 5 trials completed (1 training) 
                Debug.Log("All items collected!");
                Debug.Log("It took " + curTime + "seconds to collect them.");
                Debug.Log("Distance Travelled  = " + distance);

                startTime = false; 
            }
            //count game
            else if ( curTime >= endTime)
            {
                Debug.Log(curTime + " seconds have passed: GAME OVER");
                Debug.Log("Distance Travelled  = " + distance);
                startTime = false; 
                //end the round
            }
         
        }


        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(new Vector3(0, 0, -speed * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));
        }



        if (Input.GetMouseButton(1))
        {
            rotationX += Input.GetAxis("Mouse X") * sensX * Time.deltaTime;
            transform.localEulerAngles = new Vector3(0, rotationX, 0);
        }

        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("CLICKED");            
            nextItemFlag = clickAction(); 
           
        }
       


    }

    void randomizePosition(ref float xpos, ref float zpos, int i, int randTurn)
    {
        if( i%2 == 0  ) // even nums on +x side
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
        {
             return randTurn = 1;
        }
        else
        {
            return randTurn = -1;
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Collectible"))
        {
            playerInArea = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Collectible"))
        {
            playerInArea = false;
        }
    }
    
    bool clickAction()
    {
        if (count < numCollectibles)
        {
            if (playerInArea == true)
            {

                collectArray[count].SetActive(false);
                //collectArray[count].GetComponent<BoxCollider>().enabled = false;
                count += 1;
                playerInArea = false;
                if (count < numCollectibles)
                    collectArray[count].SetActive(true);
                Debug.Log("count = " + count);
                return true;
            }
            else
            {
                // bloop! err
                Vector3 collectPos = collectArray[count].transform.position;
                float errDist = Vector3.Distance(transform.position, collectPos);
                numErr += 1;
                Debug.Log("err #" + numErr + "; dist = " + errDist);
                return false;
            }
        }

        else
        {
            Debug.Log("Round over!");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // or .name
            return false;

        }

    }



}
