using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EvilWarrior : Enemy
{
    public bool battleStarted = false;
    private bool nearWall, jumpOverOnCooldown, jumpOverStarted;
    private int jumpOverDirection;
    private Vector2 knockback;
    private GameObject Health;
    private Collider2D nearestWall;
    public float jumpForce;

    // Evil Warrior Start, Connect needed components and set start direction and max health
    void Start()
    {
        enemyBody = GetComponent<Rigidbody2D>();
        enemyAnime = GetComponent<Animator>();
        enemyAnime.SetFloat("direction", startDirection);
        currentHealth = maxHealth;
        audioPlayer = GetComponent<AudioSource>();
        soundCont = FindObjectOfType<SoundController>();
        player = GameObject.FindGameObjectWithTag("Player");
        Health = GameObject.FindGameObjectWithTag("BossHealth");
    }

    // Evil Warrior Update, Once Battle has started fight player
    void Update()
    {
        //Wait for battle to start
        if (battleStarted)
        {
            //if hit by player get knocked back
            if (currentState == enemyState.hit && currentState != enemyState.dead)
            {
                enemyAnime.SetFloat("direction", playerPosition.x.CompareTo(transform.position.x));
                enemyAnime.SetBool("hit", true);
                enemyBody.velocity = knockback;
                return;
            }

            //Reset Wall jump when going for an attack
            if (!attackOnCooldown)
                nearestWall = null;
            //Check if in then air
            if (!(enemyBody.velocity.y >= -0.001f && enemyBody.velocity.y <= 0.001f))
            {
                inAirAnimeCheck();
            }
            //if not in the air, set in air animations to false
            else
            {
                enemyAnime.SetBool("in-air", false);
                if (currentState != enemyState.attacking)
                    currentState = enemyState.idle;
            }

            //Get player posisiont and distance from player
            playerPosition = player.transform.position;
            float distance = Mathf.Abs(playerPosition.x - transform.localPosition.x);

            //if in range, attack isn't on cooldown, in the air, or doing and different action, then attack
            if (distance <= 2 && !attackOnCooldown && currentState != enemyState.inAir && !inAction())
            {
                Attack(playerPosition.x.CompareTo(transform.position.x));
            }
            //if not in range but can attack then get into range
            else if (!attackOnCooldown && !inAction() && currentState != enemyState.inAir)
            {
                Chase();
            }
            //if waiting for cooldowns, run and dodge around
            else if (attackOnCooldown && !inAction())
            {
                //if near a wall and not trying to attack
                if (nearWall && nearestWall != null)
                {
                    //Jump if not done already and run from wall
                    if (currentState != enemyState.inAir) Jump();
                    Run(nearestWall.transform.position.x.CompareTo(transform.position.x));
                }
                //if near player and already did a wall jump, jump over player
                else if (distance <= 3 && !jumpOverOnCooldown && nearestWall != null)
                {
                    //if haven't jumped yet, jump over player and put it on cooldown
                    if (!jumpOverStarted)
                    {
                        jumpOverDirection = playerPosition.x.CompareTo(transform.position.x);
                        jumpOverStarted = true;
                        Jump();
                        StartCoroutine(JumpOverTimer());
                    }

                    //if jumped, keep moving in the same direction
                    if (currentState == enemyState.inAir)
                        Move(jumpOverDirection, 0);
                }
                //if can't jumpover and wall not near, run from player
                else if (currentState != enemyState.inAir)
                    Run(playerPosition.x.CompareTo(transform.position.x));
            }
        }
    }

    //Adjust the UI to reflect current health
    void AdjustHealth()
    {
        float currHealth = currentHealth;
        Image[] singleHealth = Health.GetComponentsInChildren<Image>();

        //Clear previous health
        foreach (Image i in singleHealth)
        {
            i.fillAmount = 0;
        }

        //fills the correct amount of health
        for (int i = 0; i < currHealth / 2; i++)
        {
            //if last health isn't a full bar, fill half of it
            if (i + 1 > currHealth / 2)
                singleHealth[i].fillAmount = .5f;
            //else fill the entire bar
            else
                singleHealth[i].fillAmount = 1;
        }
    }

    //Run from direction
    void Run(int direction)
    {
        Move(direction * -1, 0);
    }

    //Send Evil Warrior up 
    void Jump()
    {
        enemyBody.velocity = Vector2.up * jumpForce;
        currentState = enemyState.inAir;
    }

    //Set In Air Animation
    void inAirAnimeCheck()
    {
        currentState = enemyState.inAir;
        enemyAnime.SetBool("in-air", true);
        enemyAnime.SetFloat("In-Air Velocity", enemyBody.velocity.y < 0 ? -1 : 1);
    }

    //Finish attacking and increment Attack Phase if has another attack
    void DoneAttacking()
    {
        enemyAnime.SetBool("attacking", false);
        currentState = enemyState.idle;
        attackOnCooldown = true;
        if (enemyAnime.GetFloat("Attack Phase") == 2)
            enemyAnime.SetFloat("Attack Phase", 0);
        else
            enemyAnime.SetFloat("Attack Phase", enemyAnime.GetFloat("Attack Phase") + 1);
        StartCoroutine(AttackTimer());
    }

    //Enter Trigger Check
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if nearwall, set to nearest wall
        if (collision.CompareTag("Walls"))
        {
            nearWall = true;
            nearestWall = collision;
        }
        //if hit by player attack and isn't hit, interrupted, and blocking, take damage or die
        else if (collision.CompareTag("PlayerAttack") && currentState != enemyState.hit && currentState != enemyState.interrupted && currentState != enemyState.blocking)
        {
            int playerPower = collision.GetComponentInParent<PlayerStats>().power;

            //don't get hit if attacking
            if (currentState != enemyState.attacking)
            {
                currentState = enemyState.hit;
                //check is if king mode or not, and knockback accordingly
                knockback = new Vector2((player.GetComponent<PlayerStats>().kingMode ? 7 : 5) * transform.position.x.CompareTo(playerPosition.x), enemyBody.velocity.y);
                currentHealth -= playerPower;
                AdjustHealth();
                soundCont.PlaySelectedSound(audioPlayer, soundCont.enemyHit);
            }

            //if dead, send current status and start boss kill scene
            if (currentHealth <= 0)
            {
                GlobalController gameCont = GameObject.FindGameObjectWithTag("GameController").GetComponent<GlobalController>();
                gameCont.isKing = player.GetComponent<PlayerStats>().kingMode;
                gameCont.killerAttackPhase = player.GetComponent<Animator>().GetInteger("attackPhase");
                gameCont.killerOnLeft = playerPosition.x - transform.position.x < 0;
                gameCont.bossDead = true;
                SceneManager.LoadScene("Boss Kill");
            }
        }
    }

    //Exit Trigger Check
    private void OnTriggerExit2D(Collider2D collision)
    {
        //if not near wall, keep it near for a second then set to false
        if (collision.CompareTag("Walls"))
        {
            StartCoroutine(StillNearWall());
        }
    }

    //Jump over timer for cooldown and started
    private IEnumerator JumpOverTimer()
    {
        //wait a second, put on cooldown, wait another 4 seconds then take off cooldown
        yield return new WaitForSecondsRealtime(1f);
        jumpOverOnCooldown = true;
        jumpOverStarted = false;
        yield return new WaitForSecondsRealtime(4f);
        jumpOverOnCooldown = false;

    }

    //Wait a second then set near wall to false
    private IEnumerator StillNearWall()
    {
        yield return new WaitForSecondsRealtime(1f);
        nearWall = false;
    }

    //checks if hit or attacking
    bool inAction()
    {
        return currentState == enemyState.hit || currentState == enemyState.attacking;
    }
}
