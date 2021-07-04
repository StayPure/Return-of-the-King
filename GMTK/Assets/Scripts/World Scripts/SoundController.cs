using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundController : MonoBehaviour
{
    [Header("Menu Sounds")]
    public AudioClip hover; 
    public AudioClip confirm; 
    public AudioClip error;

    [Header("Player Sounds")]
    public AudioClip playerMelee1; 
    public AudioClip playerMelee2; 
    public AudioClip playerHit; 
    public AudioClip kingMelee; 
    public AudioClip hitByBoss;

    [Header("Enemies' Sounds")]
    public AudioClip goblinSlash; 
    public AudioClip skeletonSlash; 
    public AudioClip enemyBlock; 
    public AudioClip flyingEyeChomp; 
    public AudioClip enemyHit; 
    public AudioClip bossAttack; 
     
    [Header("Music")]
    public AudioClip mainTheme; 
    public AudioClip mainThemeOutro; 
    public AudioClip overWorldIntro; 
    public AudioClip overWorldTheme; 
    public AudioClip overWorldOutro; 
    public AudioClip castleMainTheme; 
    public AudioClip castleMainThemeOutro;
    public AudioClip armoryTheme; 
    public AudioClip bossTheme; 
    public AudioClip bossThemeOutro; 
    public AudioClip playerDeathTheme; 
    public AudioClip credits;

    private AudioSource audioPlayer, childAudioPlayer;
    private Player player;
    private string location = "";
    private Animator musicTransitioner;
    private GlobalController gameCont;
    private int previousProgress;

    //Allow only one Sound Controller at once, and don't destroy on load
    void Awake()
    {
        SoundController[] sCs = FindObjectsOfType<SoundController>();

        if (sCs.GetLength(0) != 1)
            Destroy(gameObject);
        DontDestroyOnLoad(this.gameObject);   
    }

    //Connect needed Componenets and check player location
    void Start()
    {
        audioPlayer = GetComponent<AudioSource>();
        childAudioPlayer = GetComponentsInChildren<AudioSource>()[1];
        musicTransitioner = GetComponent<Animator>();
        gameCont = FindObjectOfType<GlobalController>();
        player = FindObjectOfType<Player>();
        CheckProgress();
        previousProgress = gameCont.playerProgress;
    }

    //Checks location and player and boss status for music changes
    void Update()
    {
        musicTransitioner.SetBool("playerDead", gameCont.playerDead);
        musicTransitioner.SetBool("bossDead", gameCont.bossDead);

        //if player is Null and in game, find player
        if (player == null && SceneManager.GetActiveScene().name.Equals("Game Scene"))
            player = FindObjectOfType<Player>();
        //else if player isn't null then get location
        
        if (player != null) 
            location = player.worldLocation;

        //if progress is different then previous progress, check it
        if(gameCont.playerProgress != previousProgress)
        { 
            CheckProgress();
            previousProgress = gameCont.playerProgress;
        }

        //Play correct song for location
        if (location.Equals("Castle"))
        {
            musicTransitioner.SetInteger("location", 1);
        }
        else if (location.Equals("Overworld") && musicTransitioner.GetInteger("location") != 0)
        {
            musicTransitioner.SetInteger("location", 2);
        }
        else if (location.Equals("Armory"))
        {  
            musicTransitioner.SetInteger("location", 3);
        }
        else if (location.Equals("King's Throne"))
        {
            musicTransitioner.SetInteger("location", 4);
        }
    }

    //Check where player is, to set correct start song
    private void CheckProgress()
    {
        if (gameCont.playerProgress == 0 || gameCont.playerProgress == 1)
            musicTransitioner.SetFloat("startLocation", 2);
        else if (gameCont.playerProgress == 2 || gameCont.playerProgress == 3)
            musicTransitioner.SetFloat("startLocation", 1);
    }

    //play title end when start is selected
    public void TitleEndPlay()
    {
        Debug.Log("play hit");
        audioPlayer.clip = mainThemeOutro;
        audioPlayer.loop = false;
        audioPlayer.Play();
        musicTransitioner.SetBool("gameStart", true);
    }

    //picks a track from the list and plays it
    public void SetTrack(string title)
    {
        if(title.Equals("OWI"))
            audioPlayer.clip = overWorldIntro;
        else if(title.Equals("OWT"))
            audioPlayer.clip = overWorldTheme;
        else if(title.Equals("CT"))
            audioPlayer.clip = castleMainTheme;
        else if(title.Equals("AT"))
            audioPlayer.clip = armoryTheme;
        else if(title.Equals("CTO"))
            audioPlayer.clip = castleMainThemeOutro; 
        else if(title.Equals("BT"))
            audioPlayer.clip = bossTheme;
        else if(title.Equals("BTO"))
            audioPlayer.clip = bossThemeOutro;
        else if(title.Equals("Credits"))
            audioPlayer.clip = credits;
        else if(title.Equals("PD"))
            audioPlayer.clip = playerDeathTheme;
        else if (title.Equals("MT"))
            audioPlayer.clip = mainTheme;

        audioPlayer.loop = true;
        audioPlayer.Play();
    }

    //picks a track from the list and plays it from the child
    public void SetChildTrack (string title)
    {
        if(title.Equals("overWO"))
            childAudioPlayer.clip = overWorldOutro;
        else if(title.Equals("CTO"))
            childAudioPlayer.clip = castleMainThemeOutro;

        childAudioPlayer.Play();
    }

    //play the sound from the requested player
    public void PlaySelectedSound(AudioSource otherPlayer, AudioClip sound)
    {
        otherPlayer.clip = sound;
        otherPlayer.Play();
    }

}
