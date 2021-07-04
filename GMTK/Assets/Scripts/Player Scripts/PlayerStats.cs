using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public bool kingMode;

    [Header("King Stats")]
    public float KmaxHealth;
    public int Kpower;
    public int KlastAttack;
    public float Kspeed;
    public float KjumpForce;

    [Header("Normal Stats")]
    public float NmaxHealth;
    public int Npower;
    public int NlastAttack;
    public float Nspeed;
    public float NjumpForce;

    [Header("Current Stats")]
    public float currentHealth;
    public float maxHealth;
    public int power;
    public int lastAttack;
    public float speed;
    public float jumpForce;
    public float attackWindow, 
    kingDuration, 
    controlLost;

    //Changes stats depending on the mode the player is in
    void Update()
    {
        if (kingMode)
        {
            maxHealth = KmaxHealth;
            power = Kpower;
            speed = Kspeed;
            lastAttack = KlastAttack;
            jumpForce = KjumpForce;
        }
        else
        {
            maxHealth = NmaxHealth;
            power = Npower;
            speed = Nspeed;
            lastAttack = NlastAttack;
            jumpForce = NjumpForce;
        }
    }
}
