using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

void OnCollisionEnter(Collision col)
    {
        var hit = col.gameObject;
        var hitPlayer = hit.GetComponent<PlayerMove>();
        var dataMod = hit.GetComponent<DataCollection>();

        if (hitPlayer != null)
        {
            //dataMod.TakeDamage(25);
            //dataMod.
        }
        Destroy(gameObject);

    }
}
