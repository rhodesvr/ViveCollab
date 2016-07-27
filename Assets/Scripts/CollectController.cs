using UnityEngine;
using System.Collections;

public class CollectController : MonoBehaviour {

    public AudioClip clickSound;
    private AudioSource source;

	// Use this for initialization
	void Awake () {
        source = GetComponent<AudioSource>(); 
	}
	

	void OnCollisionEnter (Collision col) { //neato builtin unity col


        
        source.PlayOneShot(clickSound, 1F);
        //Debug.Log("sound off");

        var hit = col.gameObject;
        var hitPlayer = hit.GetComponent<PlayerMove>();
        var dataMod = hit.GetComponent<DataCollection>();   //modify DataCollection Scrip. Call TakeDamage()

        if (hitPlayer != null)
        {
            dataMod.TakeDamage(25);
        }
        Destroy(gameObject);
    }



    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
        //    source.PlayOneShot(clickSound, 1F);
            //source.Play();
        //    Debug.Log("sound off");
        }
    }

}

