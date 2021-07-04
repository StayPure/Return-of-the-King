using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killer : MonoBehaviour
{
    public AudioClip skeleton, goblin, flyingEye, evilWarrior;

    // Killer Start, set all the cutscene variables
    void Start()
    {
        GlobalController gameCont =  GameObject.FindGameObjectWithTag("GameController").GetComponent<GlobalController>();
        GetComponent<Animator>().SetFloat("direction", gameCont.killerOnLeft ? 1 : -1);
        GetComponent<Animator>().SetFloat("attackPhase", gameCont.killerAttackPhase);
        int enemyType = 0;

        if(gameCont.killerType.Equals("Skeleton"))
        {
            enemyType = 1;
            GetComponent<AudioSource>().clip = skeleton;
        }
        else if(gameCont.killerType.Equals("Goblin"))
        {
            enemyType = 2;
            GetComponent<AudioSource>().clip = goblin;
        }
        else if(gameCont.killerType.Equals("FlyingEye"))
        {
            enemyType = 3;
            GetComponent<AudioSource>().clip = flyingEye;
        }
        else if(gameCont.killerType.Equals("Evil Warrior"))
        {
            enemyType = 4;
            GetComponent<AudioSource>().clip = evilWarrior;
        }
        GetComponent<Animator>().SetFloat("enemyType", enemyType); 
    }

    //play sound
    public void Play()
    {
        GetComponent<AudioSource>().Play();
    }
}
