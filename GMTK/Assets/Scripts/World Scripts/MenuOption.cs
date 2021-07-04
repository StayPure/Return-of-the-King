using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuOption : MonoBehaviour, ISelectHandler, IDeselectHandler 
{
    private GameObject cursor;
    private SoundController soundCont;
    private GlobalController gameCont;
    private AudioSource audioPlayer;

    // Start, get needed components
    void Start()
    {
        cursor = this.gameObject.transform.GetChild(0).gameObject;
        soundCont = FindObjectOfType<SoundController>();
        gameCont = FindObjectOfType<GlobalController>();
        audioPlayer = GetComponent<AudioSource>();
    }

    //Move Cursor and play hover, if selected
    public void OnSelect(BaseEventData eventData)
    {
        cursor.SetActive(true);
        soundCont.PlaySelectedSound(audioPlayer, soundCont.hover);
    }

    //play confirm when clicked
    public void OnClick()
    {
        soundCont.PlaySelectedSound(audioPlayer, soundCont.confirm);
    }

    //When deselected turn cursor off
    public void OnDeselect(BaseEventData eventData)
    {
        cursor.SetActive(false);
    }

    //Load specifed scene
    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void StartGame()
    {
        AudioSource scPlayer = soundCont.GetComponent<AudioSource>();
        scPlayer.loop = false;
        soundCont.PlaySelectedSound(scPlayer, soundCont.mainThemeOutro);
        soundCont.GetComponent<Animator>().SetBool("gameStart", true);
        SceneManager.LoadScene("Game Scene");
    }

    //revive player and start game
    public void Ressurect()
    {
        gameCont.playerDead = false;
        SceneManager.LoadScene("Game Scene");
    }

    //revive player and return to the main menu
    public void ReturnToMain()
    {
        gameCont.playerDead = false;
        soundCont.GetComponent<Animator>().SetBool("gameStart", false);
        SceneManager.LoadScene("Main Menu");
    }

    //Quit the game fully
    public void EndGame()
    {
        Application.Quit();
    }
}
