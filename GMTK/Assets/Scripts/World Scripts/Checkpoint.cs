using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private GlobalController gameCont;
    public int checkpointNum;

    //Get Game Controller at the start
    void Start()
    {
        gameCont = FindObjectOfType<GlobalController>();
    }

    //Player gets checkpoint
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (gameCont.playerProgress < checkpointNum)
                gameCont.playerProgress = checkpointNum;
        }
    }
}
