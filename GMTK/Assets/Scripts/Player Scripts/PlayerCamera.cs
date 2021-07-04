using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private GameObject player;
    private string location;
    private Camera mainCamera;
    private Vector3 vel = Vector3.zero;
    private float floatVel = 0.0f;
    
    // Camera Start, Connects to player and Camera, and moves to player position
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        mainCamera = GetComponent<Camera>();
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 0.5f, -1);
    }

    // Camera Update, get player local location and adjust camera accordingly
    void Update()
    {
        location = player.GetComponent<Player>().localLocation;
        
        if (location.Equals("Struct 1"))
        {
            TransitionToLocation(new Vector3(-30, 7, -1), 6.5f);
        }
        else if (location.Equals("Struct 2"))
        {
            TransitionToLocation(new Vector3(17.5f, 4, -1), 6.5f);
        }
        else if (location.Equals("Struct 3"))
        {
            TransitionToLocation(new Vector3(76f, 7, -1), 9f);
        }
        else if (location.Equals("King's Walk"))
        {
            FollowPlayer(new Vector3(player.transform.position.x, player.transform.position.y + 4f, -1), 8.5f);
        }
        else if (location.Equals("Main Hall"))
        {
            FollowPlayer(new Vector3(player.transform.position.x, player.transform.position.y + 3f, -1), 7f);
        }
        else if(location.Equals("Top Path - Kitchen"))
        {
            TransitionToLocation(new Vector3(299.5f, 25.5f, -1), 6f);
        }
        else if (location.Equals("Top Path - Dining Room"))
        {
            FollowPlayer(new Vector3(player.transform.position.x, player.transform.position.y + 3f, -1), 6f);
        }
        else if(location.Equals("Top Path - Library"))
        {
            TransitionToLocation(new Vector3(410.5f, 25.5f, -1), 6f);
        }
        else if(location.Equals("King's Throne"))
        {
            TransitionToLocation(new Vector3(492.7f, 6.5f, -1), 5.5f);
        }
        else if(location.Equals("Bottom Path - Tunnels"))
        {
            FollowPlayer(new Vector3(player.transform.position.x, player.transform.position.y + 1.5f, -1), 4.5f);
        }
        else if(location.Equals("Bottom Path - Blacksmith"))
        {
            TransitionToLocation(new Vector3(410.5f, -13f, -1), 6f);
        }
        else if (location.Equals("World"))
        {
            FollowPlayer(new Vector3(player.transform.position.x, player.transform.position.y + 1.5f, -1), 5f);
        }
    }

    //Follows player with desired camera size
    void FollowPlayer(Vector3 position, float cameraSize)
    {
        transform.position = Vector3.MoveTowards(transform.position, position, .125f);
        mainCamera.orthographicSize = Mathf.SmoothDamp(mainCamera.orthographicSize, cameraSize, ref floatVel, .5f);
    }

    //Moves to location position and locks camera there
    void TransitionToLocation(Vector3 position, float cameraSize)
    {
        transform.position = Vector3.SmoothDamp(transform.position, position, ref vel, .5f);
        mainCamera.orthographicSize = Mathf.SmoothDamp(mainCamera.orthographicSize, cameraSize, ref floatVel, .5f);
    }
}
