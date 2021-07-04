using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class EndGame : MonoBehaviour
{
    private bool inCutscene = true, isKing;
    private AudioClip king, warrior, confirm;

    // End Game Start, set Game complete to true, set cutscene variables, and start cutscene timer
    void Start()
    {
        GlobalController gC = GameObject.FindGameObjectWithTag("GameController").GetComponent<GlobalController>(); 
        gC.gameCompleted = true;
        Animator playerAnimator = GetComponent<Animator>();
        Animator bossAnimator = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Animator>();
        playerAnimator.SetFloat("direction", gC.killerOnLeft ? -1 : 1);
        playerAnimator.SetFloat("king", gC.isKing ? 1 : -1);
        playerAnimator.SetFloat("attackPhase", gC.killerAttackPhase);
        bossAnimator.SetFloat("direction", gC.killerOnLeft ? 1 : -1);
        isKing = gC.isKing;
        StartCoroutine(CutsceneTimer());
    }

    // End Game Update, if out of cutscene when button pressed send back to main menu
    void Update()
    {
        if (Keyboard.current.anyKey.isPressed && !inCutscene)
        {
            GetComponent<AudioSource>().clip = confirm;
            GetComponent<AudioSource>().Play();
            FindObjectOfType<GlobalController>().bossDead = false;
            FindObjectOfType<GlobalController>().playerProgress = 0;
            FindObjectOfType<SoundController>().GetComponent<Animator>().SetBool("bossDead", false);
            FindObjectOfType<SoundController>().GetComponent<Animator>().SetBool("gameStart", false);
            SceneManager.LoadScene("Main Menu");
        }
    }

    //Wait for cutscene to be over before accepting input
    private IEnumerator CutsceneTimer()
    {
        yield return new WaitForSecondsRealtime(15);
        inCutscene = false;
    }

    //play sound
    public void Play()
    {
        GetComponent<AudioSource>().clip = isKing ? king : warrior;
        GetComponent<AudioSource>().Play();
    }
}
