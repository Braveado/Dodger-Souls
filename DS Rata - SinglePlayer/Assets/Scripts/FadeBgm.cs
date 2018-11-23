using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeBgm : MonoBehaviour
{

    public Player player;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (player.win == true || player.life <= 0)
            GetComponent<AudioSource>().volume -= 0.06f * Time.deltaTime;
	}
}
