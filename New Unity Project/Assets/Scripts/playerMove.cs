using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMove : MonoBehaviour
{
    public Animator myAnimator;
    public float speed = 10f;

    // Update is called once per frame
    void Update() 
    {
            if (Input.GetAxis("Horizontal") != 0)
            {
                myAnimator.SetBool("walking", true);
               
            }

            else  if (Input.GetKey("up"))
            {
                myAnimator.SetBool("jumping", true);
               
            }
            else 
            {
                myAnimator.SetBool("walking", false);
                myAnimator.SetBool("jumping", false);
            }

          

         


            
            
           
    }
}
