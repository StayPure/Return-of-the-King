using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalController : MonoBehaviour
{
    public bool killerOnLeft, isKing, playerDead, bossDead, gameCompleted;
    public string killerType;
    public int playerProgress = 0;
    public float killerAttackPhase;

    //Allow only one Global Controller at once, and don't destroy on load
    void Awake()
    {
        GlobalController gC = FindObjectOfType<GlobalController>();

        if (this != gC && gC != null)
            Destroy(gameObject);

        DontDestroyOnLoad(this.gameObject);   
    }

    //Check player progress and set position accordingly
    public void GetStartPosition()
    {
        GameObject cpList = GameObject.FindGameObjectWithTag("CheckpointList"),
        player = GameObject.FindGameObjectWithTag("Player");

        player.transform.position = cpList.transform.GetChild(playerProgress).position;
    }
}
