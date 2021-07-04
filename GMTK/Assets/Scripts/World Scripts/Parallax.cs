using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length, startpos;
    public GameObject mainCamera;
    public float parallaxAmmount;
    // Start is called before the first frame update
    void Start()
    {
        startpos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        float temp = (mainCamera.transform.position.x * (1 - parallaxAmmount));
        float distance = mainCamera.transform.position.x * parallaxAmmount;
        transform.position = new Vector2 (startpos + distance, transform.position.y);

        if (temp > startpos + length) 
            startpos += length;
        else if (temp < startpos - length) 
            startpos -= length;
    }
}
