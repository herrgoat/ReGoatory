﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Bomberfox
{
    public class PlayerControllerOLD : MonoBehaviour
    {
        private enum Direction  // helper for animator to decide which facing to use
        {
            Right,
            Left,
            Up,
            Down,
            None
        }

        [SerializeField]
        private float speed = 10f;

        // How many bombs the player can drop at the same time
        [SerializeField] 
        private int bombCount = 3;

        // The amount of bombs currently in the game
        public int CurrentBombs { get; set; } = 0;

        [SerializeField]
        private GameObject bombPrefab = null;

        private Animator animator;
        private Vector3 moveTarget;
        private Vector3 currentTarget;
        private Direction moveDirection;
        private CollisionHandler collisionHandler;

        void Start()
        {
            animator = GetComponent<Animator>();
            collisionHandler = GetComponent<CollisionHandler>();
        }

        void Update()
        {
            // print(CurrentBombs);
            float dist = Vector3.Distance(moveTarget, transform.position);
            float distance = Mathf.Abs(dist);
            float skipCorners = 0.2f;

            MoveToTarget(currentTarget);

            if (distance <= skipCorners || transform.position == moveTarget)
            {
                // if we are at target position, define new target
                moveTarget = DefineNextPosition();
                currentTarget = moveTarget;
                moveDirection = DefineMoveDirection();
            }

            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
	            animator.SetBool("Running", true);
            }
            else
            {
	            animator.SetBool("Running", false);
            }

            ProcessFire();
            UpdateAnimator();
        }

        /// <summary>
        /// Defines a new position for player to move to according to input. Checks if new position is free.
        /// </summary>
        /// <returns>Current transform plus a direction vector.</returns>
        private Vector3 DefineNextPosition()
        {
            Vector3 direction = new Vector3();
            if (Input.GetAxis("Horizontal") > 0) direction = Vector3.right;
            else if (Input.GetAxis("Horizontal") < 0) direction = Vector3.left;
            else if (Input.GetAxis("Vertical") > 0) direction = Vector3.up;
            else if (Input.GetAxis("Vertical") < 0) direction = Vector3.down;
            else direction = Vector3.zero;
            
            // add new direction to current location
            Vector3 nextPos = moveTarget + direction;

            // if the new position is not own position, doesn't have collider and tag "Block", return it
            if (nextPos != moveTarget && collisionHandler.CheckPosition(nextPos)) return nextPos;
	        
            // otherwise return own position
            return moveTarget;
        }

        /// <summary>
        /// Sets the moveDirection animation helper according to input
        /// </summary>
        /// <returns>the direction which the player should be facing</returns>
        private Direction DefineMoveDirection()
        {
            if (Input.GetAxis("Horizontal") > 0) return Direction.Right;
            else if (Input.GetAxis("Horizontal") < 0) return Direction.Left;
            else if (Input.GetAxis("Vertical") > 0) return Direction.Up;
            else if (Input.GetAxis("Vertical") < 0) return Direction.Down;
            else return Direction.None;
        }

        private void MoveToTarget(Vector3 target)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, 
                moveTarget, 
                speed * Time.deltaTime);
        }
		/// <summary>
        /// Checks input and creates a bomb if bombCount allows.
        /// </summary>
        public void ProcessFire()
        {
            if (Input.GetButtonDown("Fire1") && CurrentBombs < bombCount)
            {
                Vector3Int pos = new Vector3Int(
                    Mathf.RoundToInt(transform.position.x),
                    Mathf.RoundToInt(transform.position.y),
                    0);
                
                if (collisionHandler.CheckPosition(pos))
                {
                    GameObject bomb = Instantiate(bombPrefab, pos, Quaternion.identity);
                    // bomb.GetComponent<Bomb>().SetOwnerAndInit(this);
                }
            }
        }

        /// <summary>
        /// Updates animator component by setting triggers that define which direction player is facing.
        /// </summary>
        private void UpdateAnimator()
        {
            if (moveDirection == Direction.Right) animator.SetTrigger("FacingRight");
            if (moveDirection == Direction.Left) animator.SetTrigger("FacingLeft");
            if (moveDirection == Direction.Up) animator.SetTrigger("FacingUp");
            if (moveDirection == Direction.Down) animator.SetTrigger("FacingDown");
        }

        //Old method for movement. Obsolete but saved here just in case.
        //private void ProcessMovement()
        //{
        //    // Get input values and calculate offsets to previous position
        //    float xOffset = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        //    float yOffset = Input.GetAxis("Vertical") * speed * Time.deltaTime;

        //    // Restrict "player" from moving out of screen, numbers from pixels per unit of graphic. Temporary fix.
        //    float newX = Mathf.Clamp(transform.position.x + xOffset, 0.5f, 15.5f);
        //    float newY = Mathf.Clamp(transform.position.y + yOffset, 0.5f, 8.5f);

        //    // Move "player"
        //    transform.position = new Vector2(newX, newY);
        //}
    }
}
