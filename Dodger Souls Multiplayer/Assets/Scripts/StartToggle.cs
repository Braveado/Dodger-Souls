using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartToggle : MonoBehaviour
{
    public bool ready;
    private float timer;
    private bool sound = true;

    void Start()
    {
        timer = Time.time + 1;
    }

    void Update ()
    {
        if (timer < Time.time)
        {
            if (!ready && GetComponent<Image>().fillAmount < 1)
            {
                GetComponent<Image>().fillAmount += 1 * Time.deltaTime;
                if (GetComponent<Image>().fillAmount >= 0.5 && sound)
                {
                    GetComponent<AudioSource>().Play();
                    sound = false;
                }
            }
            else if (!ready && GetComponent<Image>().fillAmount == 1)
            {                
                ready = true;
            }
        }
    }
}
