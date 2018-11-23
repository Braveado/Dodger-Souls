using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public float speed = 5f;
    private float speedbuffer;
    private float step;

    private float demonDmg = 0.334f;
    private float pinwheelDmg = 0.167f;
    private float dragonDmg = 0.5f;

    private float soulPUamount = 10f;
    private float boss_soulPUamount = 30f;
    private float estusHeal = 0.334f;

    private Player playerColl;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        step = speed * Time.deltaTime;
        transform.Translate(0, -step, 0);

        if (this.transform.position.y <= -7)
            Destroy(this.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        playerColl = other.gameObject.GetComponent<Player>();
        if (playerColl != null)
        {
            this.GetComponent<AudioSource>().Play();
            if (this.tag == "Demon")
            {
                playerColl.ChangeLife(demonDmg, true);
            }
            else if (this.tag == "Pinwheel")
            {
                playerColl.ChangeLife(pinwheelDmg, true);
            }
            else if (this.tag == "Dragon")
            {
                playerColl.ChangeLife(dragonDmg, true);
            }
            else if (this.tag == "Soul")
            {
                playerColl.AddSouls(soulPUamount);
                pickedUp();
            }
            else if (this.tag == "Boss Soul")
            {
                playerColl.AddSouls(boss_soulPUamount);
                pickedUp();
            }
            else if (this.tag == "Estus")
            {
                playerColl.ChangeLife(estusHeal, false);
                pickedUp();
                StartCoroutine(holdPos(1.5f));                
            }
        }
    }

    void pickedUp()
    {
        this.GetComponent<SpriteRenderer>().enabled = false;
        this.GetComponent<PolygonCollider2D>().enabled = false;
    }

    IEnumerator holdPos(float time)
    {
        speedbuffer = speed;
        speed = 0f;
        yield return new WaitForSeconds(time);
        speed = speedbuffer;
    }
}
