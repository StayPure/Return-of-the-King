using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patroller : Enemy
{
    protected Vector2 pointL, pointR;
    protected bool movingRight = true;

    //Default start for patrollers 
    void Start()
    {
        enemyBody = GetComponent<Rigidbody2D>();
		enemyAnime = GetComponent<Animator>();
        enemyAnime.SetFloat("direction", startDirection);
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player");
        audioPlayer = GetComponent<AudioSource>();
        soundCont = FindObjectOfType<SoundController>();
        pointL = transform.GetChild(0).transform.position;
        pointR = transform.GetChild(1).transform.position;
    }
    
    //move from point to point till player gets into range
    protected void IdleMovement()
    {
        //pick a direction based on distance between patrol points
        if(transform.localPosition.x >= pointR.x)
            movingRight = false;
        else if (transform.localPosition.x <= pointL.x)
            movingRight = true;

        //Move accordingly to the move direction
        if(movingRight)
            Move(1, HoriDirection(pointL.y, 0.5f));
        else
            Move(-1, HoriDirection(pointL.y, 0.5f));
    }
}