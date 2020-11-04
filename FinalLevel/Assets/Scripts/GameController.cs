using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{



    public int collect = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        if (collect == 5) {
            Debug.Log("ending");
            SceneManager.LoadScene("Complete");
        }
    }

}
