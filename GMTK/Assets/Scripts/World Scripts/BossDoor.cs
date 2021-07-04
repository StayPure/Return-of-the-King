using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossDoor : MonoBehaviour
{
    public GameObject boss, player;
    
    //Start Boss Fight
    void StartFight()
    {
        boss.GetComponent<EvilWarrior>().battleStarted = true;
        player.GetComponent<Player>().currentState = PlayerState.idle;
    }

    //Play Sound
    void Play()
    {
        GetComponent<AudioSource>().Play();
    }

    //Show Boss Health One Point at a time
    IEnumerator ShowingBossHealth()
    {
        Image[] singleHealth = GameObject.FindGameObjectWithTag("BossHealth").GetComponentsInChildren<Image>();
        foreach (Image i in singleHealth)
        {
            i.fillAmount = 1;
            yield return new WaitForSecondsRealtime(.15f);
        }
    }
}
