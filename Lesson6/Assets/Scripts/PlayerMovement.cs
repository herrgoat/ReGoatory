using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    //speed we want char to move
    public float speed = 10;
    //how powerful the jump will be
    public float jumpForce = 1;
    //distance to check from the char to the ground
    public float groundCheckDistance = 0.5f;
    //the sprite
    SpriteRenderer sprite = null;
    //rigid body that will allow for physics and we can controll it
    Rigidbody2D rb = null;
    //truee or false if the char is in the air
    bool inAir = false;
    float scaleX = 1;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        scaleX = transform.localScale.x;
    }

    // Update is called once per frame
    void Update() {
        //set the speed for left and right movement but dont mess with up and down
        rb.velocity = Vector2.right * Input.GetAxis("Horizontal") * speed + Vector2.up * rb.velocity.y;

        //flip the sprite to set the direction
        if (Input.GetAxis("Horizontal") != 0) {
            //
            //sprite.flipX = Input.GetAxis("Horizontal") < 0;
            
            int direction = 1;
            
            if (Input.GetAxis("Horizontal") < 0) {
                direction = -1;
            }
            transform.localScale = new Vector3(scaleX * direction, transform.localScale.y, transform.localScale.z);
        }

        

        //Make sure char is on the ground by checking the distance from char downwards
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance);
        //if there is ground close enough you arnt in air
        inAir = hit.collider == null;
        
       
        //If the char is on the ground and Jump so space is pushed the char jumps
        if (!inAir && Input.GetButtonDown("Jump")) {
            
            transform.position += Vector3.up * 0.9f;
            rb.AddForce(Vector2.up * jumpForce);
            
        }

        
    }
}