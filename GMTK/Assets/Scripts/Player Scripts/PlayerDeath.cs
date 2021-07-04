using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    // Death Start, get killer location and king status
    void Start()
    {
        GlobalController gC =  GameObject.FindGameObjectWithTag("GameController").GetComponent<GlobalController>();
        GetComponent<Animator>().SetBool("killerOnLeft", gC.killerOnLeft);
        GetComponent<Animator>().SetBool("isKing", gC.isKing);
    }

    //Plays Sound
    public void Play()
    {
        GetComponent<AudioSource>().Play();
    }
}
