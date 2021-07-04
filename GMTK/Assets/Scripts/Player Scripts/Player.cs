using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Transactions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

/**
*   NOTE: At the end of making this game
*   A bug was found with the input system,
*   the old input system code is still there
*   under comments to show old process
*/

public enum PlayerState
{
    idle,
    moving,
    inAir,
    attacking,
    hit,
    switching,
    inCutscene
}

public class Player : MonoBehaviour
{
    private Rigidbody2D playerBody;
    private Vector3 change = Vector3.zero;
    private PlayerStats stats;
    private Animator anime;
    private IEnumerator crownDrainer;
    private bool startTakeover = false, crownDraining = false, nextAttackReady = false, quiting = false;
    private Vector2 knockback, cutscenePosition;
    private GlobalController gameCont;
    private SoundController soundCont;
    private AudioSource audioPlayer;
    private GameObject health;
    private Coroutine attacking, tryQuitting;
    public GameObject quitText;
    [Header("Player State")]
    public PlayerState currentState;
    public int groundStatus;
    
    [Header("Player Location")]
    public string localLocation = "World", worldLocation;

    [Header("King's Influence Meter")]
    public GameObject crown;

    

    // Player Start, Connects all needed components and sets max health
    void Start()
    {
        playerBody = GetComponent<Rigidbody2D>();
        stats = GetComponent<PlayerStats>();
        anime = GetComponent<Animator>();
        gameCont = FindObjectOfType<GlobalController>();
        audioPlayer = GetComponent<AudioSource>();
        soundCont = FindObjectOfType<SoundController>();
        gameCont.GetStartPosition();
        stats.currentHealth = stats.NmaxHealth;
        health = GameObject.FindGameObjectWithTag("Health");
        AdjustHealth();
    }

    // Player Update, Takes all user inputs and converts it to player actions
    void Update()
    {
        //get user horizontal input
        change.x = Keyboard.current.aKey.isPressed ? -1 : Keyboard.current.dKey.isPressed ? 1 : 0;//Input.GetAxisRaw("Horizontal");

        //if button cancel pressed, try to quit
        if (Keyboard.current.escapeKey.isPressed)//(Input.GetButton("Cancel"))
        {
            if (!quiting)
                tryQuitting = StartCoroutine(quittingGame());
        }
        //else if not held for long enough, stop quitting
        else if(tryQuitting != null)
        {
            StopCoroutine(tryQuitting);
            quiting = false;
            quitText.SetActive(false);
        }

        //if crown is full and isn't in king mode, start takeover animation
        if (crown.GetComponent<Image>().fillAmount == 1 && !stats.kingMode)
        {
            stats.kingMode = true;
            startTakeover = true;
            anime.SetBool("kingMode", true);
        }
        //if crown is empty and in king mode, relenquish power from king
        else if (crown.GetComponent<Image>().fillAmount == 0 && stats.kingMode)
        {
            stats.kingMode = false;
            anime.SetBool("kingMode", false);
        }
        //if king mode and not switchting, starting takeover, and draining crown, start draining the crown
        else if (stats.kingMode && currentState != PlayerState.switching && !startTakeover && !crownDraining)
        {
            crownDrainer = CrownDrain();
            StartCoroutine(crownDrainer);
            crownDraining = true;
        }

        //if player isn't switching, isn't getting hit, isn't in a Cutscene
        if (currentState != PlayerState.switching && currentState != PlayerState.hit && currentState != PlayerState.inCutscene)
        {
            //if in the air, check if going up or down
            if (!(playerBody.velocity.y >= -0.001f && playerBody.velocity.y <= 0.001f) && groundStatus == 0)
            {
                inAirAnimeCheck();
            }
            //if not in the air, set in air animations to false
            else
            {
                anime.SetBool("jumping", false);
                anime.SetBool("falling", false);
                if (currentState != PlayerState.attacking)
                    currentState = PlayerState.idle;
            }

            //if not attacking and not in the air and user input jump command, jump
            if (/*Input.GetButtonDown("Jump") */Keyboard.current.spaceKey.isPressed && currentState != PlayerState.attacking && currentState != PlayerState.inAir)
            {
                CharacterJump();
            }

            //if not attacking and not in the air and user input attack command, attack
            if (/*Input.GetButtonDown("Attack")*/Keyboard.current.qKey.isPressed && currentState != PlayerState.inAir && currentState != PlayerState.attacking)
            {
                Attack();
            }
            //if not attacking and change isn't zero, move character
            else if (change != Vector3.zero && currentState != PlayerState.attacking)
            {
                MoveCharacter();
            }
            //if not attacking, set to idle 
            else if (currentState != PlayerState.attacking)
            {
                playerBody.velocity = new Vector2(0, playerBody.velocity.y);
                anime.SetBool("running", false);
                currentState = PlayerState.idle;
            }
        }
        //if player is hit, get knocked back
        else if (currentState == PlayerState.hit && currentState != PlayerState.inCutscene)
        {
            playerBody.velocity = knockback;
        }
        //if in a Cutscene
        else if (currentState == PlayerState.inCutscene)
        {
            //Move till in position
            if (transform.position.x < cutscenePosition.x)
            {
                playerBody.velocity = new Vector2(1 * stats.speed, playerBody.velocity.y);
                anime.SetFloat("direction", 1);
                anime.SetBool("running", true);
            }
            //Then wait for boss doors to close
            else
            {
                playerBody.velocity = Vector2.zero;
                anime.SetBool("running", false);
                GameObject.FindGameObjectWithTag("BossFightDoor").GetComponent<Animator>().SetBool("closed", true);
            }
        }
    }

    private IEnumerator quittingGame()
    {
        quitText.SetActive(true);
        quiting = true;
        yield return new WaitForSecondsRealtime(3);
        soundCont.GetComponent<Animator>().SetBool("gameStart", false);
        SceneManager.LoadScene("Main Menu");
    }

    //Adjust the UI to reflect current health
    void AdjustHealth()
    {
        float currhealth = stats.currentHealth;
        Image[] singleHealth = health.GetComponentsInChildren<Image>();

        //Clear preivous health
        foreach (Image i in singleHealth)
        {
            i.fillAmount = 0;
        }

        //Check if king mode
        if (!stats.kingMode)
        {
            //fills the correct amount of health
            for(int i = 0; i < currhealth/2; i++)
            {
                //if last health isn't a full bar, fill half of it
                if (i + 1 > currhealth / 2)
                    singleHealth[i].fillAmount = .5f;
                //else fill the entire bar
                else
                    singleHealth[i].fillAmount = 1;

                //Set to red, for base character
                singleHealth[i].color = Color.red;
            }
        }
        else
        {
            for(int i = 0; i < currhealth/2; i++)
            {
                if (i + 1 > currhealth / 2)
                    singleHealth[i].fillAmount = .5f;
                else
                    singleHealth[i].fillAmount = 1;

                //Set half to red and other to blue for King
                if (i >= currhealth / 4)
                    singleHealth[i].color = Color.blue;
                else
                    singleHealth[i].color = Color.red;
            }
        }
    }

    //Drain the crown while king for kingDuration
    IEnumerator CrownDrain()
    {
        for (int i = 0; i < stats.kingDuration + 1; i++)
        {
            crown.GetComponent<Image>().fillAmount -= 1 / stats.kingDuration;
            yield return new WaitForSecondsRealtime(1);
        }
        crownDraining = false;
    }

    //Timer to count how long you have till the next combo attack
    IEnumerator AttackTimer()
    {
        nextAttackReady = true;
        yield return new WaitForSecondsRealtime(stats.attackWindow);
        nextAttackReady = false;
        anime.SetInteger("attackPhase", 0);
    }

    //Attack function
    void Attack()
    {
        anime.SetBool("attacking", true);
        playerBody.velocity = Vector2.zero;
        currentState = PlayerState.attacking;
        if (!stats.kingMode)
            soundCont.PlaySelectedSound(audioPlayer, Random.Range(1, 3) == 1 ? soundCont.playerMelee1 : soundCont.playerMelee2);
        else 
            soundCont.PlaySelectedSound(audioPlayer, soundCont.kingMelee);

        //if not in king mode, each attack gives king more control
        if (!stats.kingMode)
            crown.GetComponent<Image>().fillAmount += 1 / stats.controlLost;
    }

    //After attacking, go to the next attack phase for the combo
    void DoneAttacking()
    {
        //if done the last attack, reset
        if (anime.GetInteger("attackPhase") == stats.lastAttack)
        {
            anime.SetInteger("attackPhase", 0);
        }
        //else go to next attack phase and reset attack timer
        else
        {
            anime.SetInteger("attackPhase", anime.GetInteger("attackPhase") + 1);
            if (nextAttackReady) StopCoroutine(attacking);
            attacking = StartCoroutine(AttackTimer());
        }
        currentState = PlayerState.idle;
        anime.SetBool("attacking", false);
    }

    //Move player
    void MoveCharacter()
    {
        playerBody.velocity = new Vector2(change.x * stats.speed, playerBody.velocity.y);
        anime.SetFloat("direction", change.x);
        anime.SetBool("running", true);

        //if not in the air, currentState is moving
        if (currentState != PlayerState.inAir)
            currentState = PlayerState.moving;

        //if not in king mode, every vertical movement gives king more control
        if (!stats.kingMode)
           crown.GetComponent<Image>().fillAmount += (float).005 / stats.controlLost;
    }

    //Player Jumps
    void CharacterJump()
    {
        playerBody.velocity = Vector2.up * stats.jumpForce;
        currentState = PlayerState.inAir;

        //if not in king mode, every horizontal movement gives king more control
        if (!stats.kingMode)
           crown.GetComponent<Image>().fillAmount += (float).01 / stats.controlLost;
    }

    //Checks if player is going up or coming down
    void inAirAnimeCheck()
    {
        currentState = PlayerState.inAir;

        //if velocity is positive/going up
        if (playerBody.velocity.y > 0)
        {
            anime.SetBool("jumping", true);
            anime.SetBool("falling", false);
        }
        //if velocity is negative/going down
        else
        {
            anime.SetBool("jumping", false);
            anime.SetBool("falling", true);
        }
    }

    //Going from one form to the other
    void Switching(int starting)
    {
        //if switch is starting
        if (starting == 1)
        {
            currentState = PlayerState.switching;
            playerBody.velocity = Vector2.zero;
        }
        //if switch is ending
        else
        {
            currentState = PlayerState.idle;
            startTakeover = false;

            //if King Mode adjust the health up
            if (stats.kingMode)
                stats.currentHealth = (stats.currentHealth / stats.NmaxHealth) * stats.KmaxHealth;
            //if not, adjust the health down
            else
                stats.currentHealth = (stats.currentHealth / stats.KmaxHealth) * stats.NmaxHealth;
            AdjustHealth();
        }
    }

    //Player enter Trigger check    
    private void OnTriggerEnter2D(Collider2D collision)
    {   
        //if collided with a enemy attack, enemy flyer, and player isn't hit and isn't switching
        if ((collision.CompareTag("EnemyAttack") || collision.CompareTag("EnemyFlyer")) && currentState != PlayerState.hit && currentState != PlayerState.switching)
        {
            float distanceDifference = transform.localPosition.x - collision.transform.position.x;
            int enemyPower;
            string enemyType;
            knockback = Vector2.zero;

            //if is an enemy attack, get info from parent
            if (collision.CompareTag("EnemyAttack"))
            {
                enemyPower = collision.GetComponentInParent<Enemy>().power;
                enemyType = collision.GetComponentInParent<Enemy>().enemyType;
            }
            //if not, get info directly
            else
            {
                enemyPower = collision.GetComponent<Enemy>().power;
                enemyType = collision.GetComponent<Enemy>().enemyType;
            }

            //Take Damage and play sound
            stats.currentHealth -= enemyPower;
            soundCont.PlaySelectedSound(audioPlayer, enemyType.Equals("Evil Warrior") ? soundCont.hitByBoss : soundCont.playerHit);
            AdjustHealth();

            //if health goes below 0, go to death scene
            if (stats.currentHealth <= 0)
            {
                //check what side the killer is on, if flyer spawn on the opposite
                gameCont.killerOnLeft = collision.CompareTag("EnemyFlyer") ? !(distanceDifference > 0) : distanceDifference > 0;
                gameCont.isKing = stats.kingMode;
                gameCont.killerType = enemyType;

                //if the boss, also get attack phase
                if (enemyType.Equals("Evil Warrior"))
                    gameCont.killerAttackPhase = collision.GetComponentInParent<Animator>().GetFloat("Attack Phase");
                
                //Set player to dead and load death scene
                gameCont.playerDead = true;
                SceneManager.LoadScene("Player Death");
            }

            //if player is to the left of attack
            if(distanceDifference < 0)
            {
                //if its a land attack
                if(collision.CompareTag("EnemyAttack"))
                {
                    knockback = new Vector2(stats.kingMode ? enemyPower * -1.5f : enemyPower * -3f, 0);
                    anime.SetFloat("direction", 1);
                }
                //if its a flyer attack
                else
                {
                    anime.SetFloat("direction", 1);
                }
                
            }
            //if player is to the right of the attack
            else 
            {
                //if its a land attack
                if(collision.CompareTag("EnemyAttack"))
                {
                    knockback = new Vector2(stats.kingMode ? enemyPower * 1.5f : enemyPower * 3f, 0);
                    anime.SetFloat("direction", -1);
                }
                //if its a flyer attack
                else
                {
                    anime.SetFloat("direction", -1);
                }
                
            } 
            anime.SetBool("hurt", true);
            currentState = PlayerState.hit;
        }
        //if local location set local location name
        else if (collision.CompareTag("Location"))
        {
            localLocation = collision.name;
        }
        //if world location set world location name
        else if (collision.CompareTag("World Location"))
        {
            worldLocation = collision.name;
        }
        //if entering a boss room, go into cutscene
        else if (collision.name.Equals("Boss Room Entrance"))
        {
            currentState = PlayerState.inCutscene;
            cutscenePosition = collision.transform.gameObject.transform.GetChild(0).transform.position;
        }
    }

    //Player exit Trigger check
    private void OnTriggerExit2D(Collider2D collision)
    {
        //Set location to default if leaving a location
        if (collision.CompareTag("Location"))
        {
            localLocation = "World";
        }
    }

    //Player enter Collision check
    private void OnCollisionEnter2D(Collision2D collision) 
    {
        //Check what ground player is on
        if (collision.gameObject.CompareTag("Ground"))
            groundStatus = 1;
        else if (collision.gameObject.CompareTag("Platform"))
            groundStatus = 2;
    }

    //Player exit collision check
    private void OnCollisionExit2D(Collision2D collision) 
    {
        //if not on either ground types, in air 
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Platform"))
        {
            groundStatus = 0;
        }
    }

    //Recover from being hit
    void Recovered()
    {
        anime.SetBool("hurt", false);
        currentState = PlayerState.idle;
    }
}