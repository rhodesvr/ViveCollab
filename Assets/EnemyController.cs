using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class EnemyController : NetworkBehaviour {


    int direction = 1;
    int turn = 0;
    // Use this for initialization
    void Start () {

	}
    

    public void Update()
    {
        if (isServer)
        {
            //RpcMoveMe();
        }
    }


    [ClientRpc]
    public void RpcMoveMe()
    {
        transform.Translate(direction * 0.01f, 0, 0);

        if (transform.position.x > 3)        
            direction = -1;        
        if (transform.position.x < -3)        
            direction = 1;        
    }

    public void UpdateTurn(int nextTurn)
    {
        turn = nextTurn;
    }

    public void DestroySelf()
    {
        Object.Destroy(this.gameObject);
    }
}
