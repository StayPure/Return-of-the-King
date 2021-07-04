using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Patroller
{
    public float blockSpeed;
    public bool original;
    public GameObject deathSprite;
    private bool directionSet = false, blockOnCooldown = false;

    //Skeleton Update Function
    void Update()
    {
        //Check if this is original skeleton or the death sprite
        if (!original)
        {
            //sets direction for death animation if not already set
            if(!directionSet)
            {
                if (transform.localPosition.x < playerPosition.x)
                    enemyAnime.SetFloat("direction", 1);
                else
                    enemyAnime.SetFloat("direction", -1);
                directionSet = true;
                soundCont.PlaySelectedSound(audioPlayer, soundCont.enemyHit);
            }
        }
        //check if goblin is dead
        else if (currentState != enemyState.dead)
        {
            playerPosition = player.transform.localPosition;

            //if skeleton is hit but not dead
            if ((currentState == enemyState.hit || currentState == enemyState.interrupted) && currentState != enemyState.dead)
            {
                //if skeleton's attack is interrupted put attack on cooldown
                if (currentState == enemyState.interrupted && !attackOnCooldown)
                {
                    attackOnCooldown = true;
                    StartCoroutine(Timer(attackSpeed, true));
                }
                enemyAnime.SetBool("attacking", false);
                enemyAnime.SetBool("hit", true);
                enemyBody.velocity = Vector2.zero;
            }
            //if player enters Skeleton's patrol area
            else if (playerPosition.x <= pointR.x && playerPosition.x >= pointL.x
            && playerPosition.y <= pointR.y && playerPosition.y >= pointR.y - 3)
            {
                float distance = Mathf.Abs(playerPosition.x - transform.localPosition.x);

                //if player is in attack range and attack isn't on cooldown and not blocking
                if (distance <= 2 && !attackOnCooldown && currentState != enemyState.blocking)
                {
                    Action("attacking", playerPosition.x.CompareTo(transform.localPosition.x));
                }
                //if player is in block range and block isn't on cooldown and not attacking
                else if (distance <= 3 && !blockOnCooldown && currentState != enemyState.attacking)
                {
                    Action("blocking", playerPosition.x.CompareTo(transform.localPosition.x));
                }
                //if not blocking and not attacking
                else if (currentState != enemyState.attacking && currentState != enemyState.blocking)
                {
                    //if not in range to attack, get in range
                    if (distance > 2)
                        Chase();
                    //in range to attack but waiting for cooldowns
                    else
                        enemyAnime.SetBool("moving", false);
                }
            }
            //if idle, idly patrol area
            else if (currentState == enemyState.idle)
            {
                IdleMovement();
            }
        }
        //if dead, spawn death sprite and destroy original
        else 
        {
            Instantiate(deathSprite, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    //Preform action, attack or block
    void Action(string action, int direction)
    {
        enemyAnime.SetFloat("direction", direction);
        enemyAnime.SetBool(action, true);
        enemyBody.velocity = Vector2.zero;
        if (action.Equals("attacking") && currentState != enemyState.attacking)
        {
            soundCont.PlaySelectedSound(audioPlayer, soundCont.skeletonSlash);
        }
        
        //check the action and set the currentState accordingly 
        currentState = action.Equals("attacking") ? enemyState.attacking : enemyState.blocking;
    }

    //finish action and put it on cooldown
    public void DoneAction(string action)
    {
        bool forAttack = action.Equals("attacking");
        enemyAnime.SetBool(action, false);
        currentState = enemyState.idle;
        if (forAttack)
            attackOnCooldown = true;
        else 
            blockOnCooldown = true;

        //start cooldown timer for specified action
        StartCoroutine(Timer(forAttack ? attackSpeed : blockSpeed, forAttack));
    }

    //start a cooldown timer for action specified
    private IEnumerator Timer(float speed, bool forAttack)
    {
        yield return new WaitForSecondsRealtime(1/speed);
        if (forAttack)
            attackOnCooldown = false;
        else 
            blockOnCooldown = false;
    }
}