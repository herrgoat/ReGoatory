using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collect : MonoBehaviour
{
    GameController gc;

    //particle effect to be used
    public GameObject reward;

    

    void Start () {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        
    }

    //on trigger enter is a class and will use a 2D collider component 
    private void OnTriggerEnter2D(Collider2D collision) {
        
        //test to make sure it is working
        Debug.Log("something");
        //instantiate / create a copy of the reward particle
        Instantiate(reward, transform.position, transform.rotation);
        //destroy the original gameobject this script is attached to
        Destroy(gameObject);
        gc.collect++;
    }
}
