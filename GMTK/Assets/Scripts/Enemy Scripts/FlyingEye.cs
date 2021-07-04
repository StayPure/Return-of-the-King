using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEye : Patroller
{

    public float attackDistance;
    private bool firstAttackWait = true;

    //Flying Eye update function
    void Update()
    {
        //Flying Eye is currently not dead
        if (currentState != enemyState.dead)
        {
            playerPosition = player.transform.localPosition;

            //if Flying Eye is hit
            if (currentState == enemyState.hit)
            {
                enemyAnime.SetBool("hit", true);
                enemyBody.velocity = Vector2.zero;
            }
            //if player is within chase distance of Flying Eye
            else if (playerPosition.x <= transform.localPosition.x + 6 && playerPosition.x >= transform.localPosition.x - 6)
            {
                float xDistance = Mathf.Abs(playerPosition.x - transform.localPosition.x),
                yDistance = Mathf.Abs(playerPosition.y - transform.localPosition.y);
                
                //if player is in the accepteable distance for an attack and isn't currently attacking and attack isn't on cooldown
                if (xDistance <= 3 && yDistance <= 1 && !attackOnCooldown && currentState != enemyState.attacking)
                {
                    //wait to do first attack instead of immediately attacking 
                    if (firstAttackWait)
                    {
                        attackOnCooldown = true;
                        firstAttackWait = false;
                        StartCoroutine(AttackTimer());
                    }
                    else
                    {
                        Attack(playerPosition.x.CompareTo(transform.localPosition.x));
                    }
                }
                //if not attacking
                else if (currentState != enemyState.attacking)
                {
                    //chase if too far to attack
                    if (xDistance > 2 || yDistance > 0.5)
                        Chase(2f);
                    //else wait for cooldowns
                    else
                        enemyBody.velocity = Vector2.zero;
                }
            }
            //if idle, do idle patrols 
            else if (currentState == enemyState.idle)
            {
                IdleMovement();
                firstAttackWait = true;
            }
        }
        //if dead, FALL
        else if (currentState == enemyState.dead)
        {
            Move(0, -1);
        }
    }

    //Moving Attack to attack with the enemy body
    new void Attack(int direction)
    {
        enemyBody.velocity = Vector2.zero;
        enemyBody.velocity = new Vector2(attackDistance * direction, 0);
        enemyAnime.SetFloat("direction", direction);
        enemyAnime.SetBool("attacking", true);
        currentState = enemyState.attacking;
        soundCont.PlaySelectedSound(audioPlayer, soundCont.flyingEyeChomp);
    }
}
