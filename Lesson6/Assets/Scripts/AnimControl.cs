using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimControl : MonoBehaviour
{
    public Animator myAnimator;
    public float speed = 10f;

    // Update is called once per frame
    void Update() 
    {
            if (Input.GetAxis("Horizontal") != 0)
            {
                myAnimator.SetBool("walk", true);
               
                //if (Input.GetAxis("Horizontal") < 0)
               // {
                //    transform.rotation = Quaternion.Euler(0, 180, 0);
                //}

               // if (Input.GetAxis("Horizontal") > 0)
                //{
                //    transform.rotation = Quaternion.Euler(0, 0, 0);
                //}

            

            //else  if (Input.GetKey("up"))
            //{
              //  myAnimator.SetBool("jump", true);
               
            //}

            //else  if (Input.GetKey("down"))
            //{
             //   myAnimator.SetBool("crouch", true);

               
            }

        

            else 
            {
                myAnimator.SetBool("walk", false);
                myAnimator.SetBool("jump", false);
                myAnimator.SetBool("crouch", false);
            }

          

         


            
            
           
    }
}
