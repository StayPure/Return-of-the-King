using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class CodeHandler : MonoBehaviour
{
    public TMP_InputField userText;
    private GlobalController gameCont;
    private SoundController soundCont;

    // Code Start, if on a Code, set code to correct code, 
    //             else get sound controller for the other functions 
    void Start()
    {
        gameCont = FindObjectOfType<GlobalController>();
        if (this.name.Equals("CODE"))
        {
            if (gameCont.playerProgress == 1)
                GetComponent<TextMeshProUGUI>().text = "KFall";
            else if (gameCont.playerProgress == 2)
                GetComponent<TextMeshProUGUI>().text = "LifeWC";
            else if (gameCont.playerProgress == 3)
                GetComponent<TextMeshProUGUI>().text = "SLogic";
            else 
                GetComponent<TextMeshProUGUI>().text = "";
        }
        else
        {
            soundCont = FindObjectOfType<SoundController>();
        }
    }

    //Check Code giving a error sound if incorrect
    public void CheckCode()
    {
        //set to lower case and trim, so it isn't case sensitive
        string code = userText.text.ToLower().Trim();

        if (code.Equals("kfall"))
            CorrectCode(1);
        else if (code.Equals("lifewc"))
            CorrectCode(2);
        else if (code.Equals("slogic"))
            CorrectCode(3);
        else
        {
            GetComponent<AudioSource>().clip = soundCont.error;
            GetComponent<AudioSource>().Play();
        }
    }

    //if code is correct, get progress for code and send player to location
    private void CorrectCode(int progress)
    {
        gameCont.playerProgress = progress;
        GetComponent<AudioSource>().clip = soundCont.confirm;
        GetComponent<AudioSource>().Play();
        soundCont.GetComponent<Animator>().SetBool("gameStart", true);
        SceneManager.LoadScene("Game Scene");
    }
}
