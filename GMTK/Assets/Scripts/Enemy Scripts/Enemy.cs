using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Statemachine for all enmemies in the game
public enum enemyState
{
    idle,
    inAir,
    attacking,
    hit,
    interrupted,
    blocking,
    dead
}

public class Enemy : MonoBehaviour
{
    public string enemyType;

    [Header("Enemy Stats")]
    public enemyState currentState;
    public int maxHealth, power;
    public float speed, attackSpeed, startDirection;
    public bool flyer;
    
    protected int currentHealth;
    protected GameObject player;
    protected Vector2 playerPosition;
    protected SoundController soundCont;
    protected AudioSource audioPlayer;
    protected bool attackOnCooldown = false;
    protected Rigidbody2D enemyBody;
    protected Animator enemyAnime;
    

    // Enemy Start, Connect all the needed things and set max health
    void Start()
    {
        enemyBody = GetComponent<Rigidbody2D>();
		enemyAnime = GetComponent<Animator>();
        enemyAnime.SetFloat("direction", startDirection);
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player");
        audioPlayer = GetComponent<AudioSource>();
        soundCont = FindObjectOfType<SoundController>();
    }

    void Death()
    {
        Destroy(gameObject);
    }

    //Trigger Check
	private void OnTriggerEnter2D(Collider2D collision)
	{
        //if hit by player attack and isn't hit, interrupted, and blocking
        if (collision.CompareTag("PlayerAttack") && currentState != enemyState.hit && currentState != enemyState.interrupted && currentState != enemyState.blocking)
        {
            int playerPower = collision.GetComponentInParent<PlayerStats>().power;
            currentHealth -= playerPower;
            soundCont.PlaySelectedSound(audioPlayer, soundCont.enemyHit);

            //if dead
            if (currentHealth <= 0)
            {
                currentState = enemyState.dead;
                enemyAnime.SetBool("dead", true);
            }
            //if enemy isn't attacking
            else if (currentState != enemyState.attacking)
                currentState = enemyState.hit;
            //if enemy is attacking
            else 
                currentState = enemyState.interrupted;
        }
        //if blocked, play blocked sound
        else if (currentState == enemyState.blocking)
        {
            soundCont.PlaySelectedSound(audioPlayer, soundCont.enemyBlock);
        }
	}

    //Recover from getting hit
    void Recovered()
    {
        enemyAnime.SetBool("hit", false);
        currentState = enemyState.idle;
    }

    //Default Attack function
    protected void Attack(int direction)
    {
        enemyAnime.SetFloat("direction", direction);
        enemyAnime.SetBool("attacking", true);
        enemyBody.velocity = Vector2.zero;
        //Check if already attacking
        if (currentState != enemyState.attacking)
        {
            //play sound for Boss or Goblin
            soundCont.PlaySelectedSound(audioPlayer, enemyType.Equals("Evil Warrior") ? soundCont.bossAttack : soundCont.goblinSlash);
        }
        currentState = enemyState.attacking;
    }

    //Attack cooldown timer
    protected IEnumerator AttackTimer()
    {
        yield return new WaitForSecondsRealtime(1/attackSpeed);
        attackOnCooldown = false;
    }

    //Finish attacking and put it on cooldown
    void DoneAttacking()
    {
        enemyAnime.SetBool("attacking", false);
        currentState = enemyState.idle;
        attackOnCooldown = true;
        StartCoroutine(AttackTimer());
    }

    //Choosing horizontal direction of movement
    protected int HoriDirection(float goalY, float difference)
    {
        //Defaults to Zero for non flying units
        int horizontalMovement = 0;

        //Checks if Enemy is a flyer and isn't in the accpetable distance of the Y its going toward
        if (flyer && !(transform.localPosition.y >= goalY - difference && transform.localPosition.y <= goalY + difference))
        {
            //if flyer is under the target Y
            if (transform.localPosition.y < goalY)
                horizontalMovement = 1;
            //if flyer is above the target Y
            else if (transform.localPosition.y > goalY)
                horizontalMovement = -1;
        }

        //Return the selected movement direction
        return horizontalMovement;
    }

    //Chase player character
    protected void Chase()
    {
        //playerPosition.y has 0.72 added so flyers chase to the head and not center mass

        //if Enemy is to the left of the player
        if(transform.localPosition.x < playerPosition.x)
            Move(1, HoriDirection(playerPosition.y + 0.72f, 0.001f));
        //if Enemy is to the right of the player
        else
            Move(-1, HoriDirection(playerPosition.y + 0.72f, 0.001f));
    }

    //Chase player character, but stay a certain distance away
    protected void Chase(float maintainDistance)
    {
        int backupDigit = 1;
        float distance = Mathf.Abs(transform.localPosition.x - playerPosition.x); 

        //If enemy is took close, set the backupDigit to -1 so the enemy moves away from the player
        if (distance < maintainDistance - .5)
            backupDigit = -1;
        //If enemy is at an acceptable distance, set the backupDigit to 0 so the enemy doesn't move
        else if (distance >= maintainDistance - 0.5 && distance <= maintainDistance + 0.5)
            backupDigit = 0;
        
        //See Chase() comments*
        if(transform.localPosition.x < playerPosition.x)
            Move(1 * backupDigit, HoriDirection(playerPosition.y + 0.72f, 0.1f));
        else
            Move(-1 * backupDigit, HoriDirection(playerPosition.y + 0.72f, 0.1f));
    }

    //Moves enemy in a certain direction
    protected void Move (int directionX, int directionY) 
    {
        enemyBody.velocity = new Vector2(directionX * speed, flyer ? directionY * speed : (directionY == 0 ? enemyBody.velocity.y : directionY * speed));

        //if not moving on the X-axis, stop moving animation
        if (directionX == 0) 
        {
            //Checks what side of the player the enemy is on and set direction to face player
            enemyAnime.SetFloat("direction", transform.localPosition.x < playerPosition.x ? 1 : -1);
            enemyAnime.SetBool("moving", false);
        }
        //else moving on the X-axis, start moving animation
        else
        {
            enemyAnime.SetFloat("direction", directionX);
            enemyAnime.SetBool("moving", true);
        }
    }
}