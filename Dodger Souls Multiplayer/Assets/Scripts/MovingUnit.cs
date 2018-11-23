using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MovingUnit : NetworkBehaviour {

    public float speed = 0f;

    private float demonDmg = 0.334f;
    private float pinwheelDmg = 0.167f;
    private float dragonDmg = 0.667f;

    private float soulPUamount = 10f;
    private float boss_soulPUamount = 30f;
    private float estusHeal = 0.334f;
    private float humanityHeal = 1f;

    private PlayerCharacter playerCollider;

    void Update ()
    {
        transform.Translate(0, -(speed * Time.deltaTime), 0);

        if (this.transform.position.y <= -7)
        {            
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        playerCollider = other.gameObject.GetComponent<PlayerCharacter>();
        if (playerCollider != null)
        {
            GetComponent<AudioSource>().Play();

            if (tag == "Demon")
            {
                playerCollider.ChangeLife(demonDmg, true);
            }
            else if (tag == "Necromancer")
            {
                playerCollider.ChangeLife(pinwheelDmg, true);
            }
            else if (tag == "Dragon")
            {
                playerCollider.ChangeLife(dragonDmg, true);
            }
            else if (tag == "Soul")
            {                
                playerCollider.AddSouls(soulPUamount);
                PickUp();
                RpcPickedUp();
            }
            else if (tag == "BossSoul")
            {
                playerCollider.AddSouls(boss_soulPUamount);
                PickUp();
                RpcPickedUp();
            }
            else if (tag == "Estus")
            {
                playerCollider.ChangeLife(estusHeal, false);
                PickUp();
                RpcPickedUp();
                speed = 1;
            }
            else if (tag == "Humanity")
            {
                playerCollider.ChangeLife(humanityHeal, false);
                PickUp();
                RpcPickedUp();
                speed = 1;
            }
        }
    }

    [ClientRpc]
    void RpcPickedUp()
    {
        PickUp();
    }

    void PickUp()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
    }
}
