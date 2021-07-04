using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Enemy
{
    public bool original;
    public float chaseDistance;
    public int groundStatus;
    public GameObject deathSprite;
    private bool chasing = false, directionSet = false;

    //Goblin update fuction
    void Update()
    {
        //Check if this is original goblin or the death sprite
        if (!original)
        {
            //sets direction for death animation if not already set
            if (!directionSet)
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
            int playerGroundStatus = player.GetComponent<Player>().groundStatus;

            //if goblin is hit but not dead
            if (currentState == enemyState.hit && currentState != enemyState.dead)
            {
                enemyAnime.SetBool("attacking", false);
                enemyAnime.SetBool("hit", true);
                enemyBody.velocity = Vector2.zero;
            }
            //if player is within goblin chase distance and is on the same platform as player, or if chasing already started
            else if ((Mathf.Abs(playerPosition.x - transform.position.x) <= chaseDistance || chasing)
            && groundStatus == playerGroundStatus &&
            ((groundStatus == 1 && playerPosition.y <= transform.localPosition.y + 10 && playerPosition.y >= transform.localPosition.y - 10) ||
                groundStatus == 2 && playerPosition.y <= transform.localPosition.y + 0.5 && playerPosition.y >= transform.localPosition.y - 0.5))
            {
                chasing = true;
                float distance = Mathf.Abs(playerPosition.x - transform.localPosition.x);

                //if attack isn't on cooldown and player is within in attack range
                if (!attackOnCooldown && distance <= 1.5)
                {
                    Attack(playerPosition.x.CompareTo(transform.localPosition.x));
                }
                //if not attacking
                else if (currentState != enemyState.attacking)
                {
                    //not in attack range
                    if (distance > 1)
                        Chase(1f);
                    //in attack range but waiting for cooldowns
                    else
                        enemyAnime.SetBool("moving", false);
                }
            }
            //if idle, just idle animation
            else if (currentState == enemyState.idle)
            {
                enemyAnime.SetBool("moving", false);
                chasing = false;
            }
        }
        //if dead, spawn death sprite and destroy original
        else
        {
            Instantiate(deathSprite, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    //Enter Collision Check, For Ground Status
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            groundStatus = 1;
        else if (collision.gameObject.CompareTag("Platform"))
            groundStatus = 2;
    }

    //Exit Collision Check, For Ground Status
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Platform"))
            groundStatus = 0;
    }
}
