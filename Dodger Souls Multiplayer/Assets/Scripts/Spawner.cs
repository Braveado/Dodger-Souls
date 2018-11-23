using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Spawner : NetworkBehaviour
{
    [Header("Enemies")]
    public GameObject[] enemies;
    private int randEnemy;
    private Vector3 Epos;
    private GameObject ns_enemy;
    public float EspawnWaitIni = 3f;
    public float EspawnWaitMax = 1f;
    public float EspawnWaitMin = 0.33f;
    private float EspawnWait;
    [Space]
    [Header("Boss")]
    public GameObject boss;
    private Vector3 Bpos;
    private GameObject ns_boss;
    public float BspawnWaitIni = 20f;
    public float BspawnWaitMax = 20f;
    public float BspawnWaitMin = 10f;
    private float BspawnWait;
    [Space]
    [Header("Items")]
    public GameObject[] items;
    private int randItem;
    private Vector3 Ipos;
    private GameObject ns_item;
    public float IspawnWaitIni = 10f;
    public float IspawnWaitMax = 10f;
    public float IspawnWaitMin = 5f;
    private float IspawnWait;
    [Space]
    private bool active;

    void Update()
    {
        if (GameObject.Find("NetworkManager").GetComponent<Networking>().Players[0] != null &&
            GameObject.Find("NetworkManager").GetComponent<Networking>().Players[1] != null)
        {
            if (!active)
            {
                active = true;
                GetComponent<AudioSource>().Play();
                EspawnWait = Time.time + EspawnWaitIni;
                BspawnWait = Time.time + BspawnWaitIni;
                IspawnWait = Time.time + IspawnWaitIni;
            }
        }
        else
        {
            if(active)
            {
                active = false;
                GetComponent<AudioSource>().Stop();
                foreach (Transform spawned in transform)
                    GameObject.Destroy(spawned.gameObject);
            }
        }


        if (active)
        {
            if(EspawnWait <= Time.time)
            {
                randEnemy = Random.Range(0, enemies.Length);
                Epos.x = Random.Range(-9.0f, 9.0f);

                if (randEnemy == 2)
                {
                    ns_enemy = Instantiate(enemies[randEnemy], transform.position + Epos, transform.rotation);
                    ns_enemy.transform.parent = transform;
                    NetworkServer.Spawn(ns_enemy);
                    if (Random.Range(0, 3) != 2)
                    {
                        ns_enemy = Instantiate(enemies[randEnemy], transform.position - Epos, transform.rotation);
                        ns_enemy.transform.parent = transform;
                        NetworkServer.Spawn(ns_enemy);
                    }
                }
                else
                {
                    ns_enemy = Instantiate(enemies[randEnemy], transform.position + Epos, transform.rotation);
                    ns_enemy.transform.parent = transform;
                    NetworkServer.Spawn(ns_enemy);
                }

                EspawnWait = Time.time + Random.Range(EspawnWaitMin, EspawnWaitMax);
            }

            if (BspawnWait <= Time.time)
            {
                Bpos.x = Random.Range(-7.5f, 7.5f);

                ns_boss = Instantiate(boss, transform.position + Bpos, transform.rotation);
                ns_boss.transform.parent = transform;
                NetworkServer.Spawn(ns_boss);

                BspawnWait = Time.time + Random.Range(BspawnWaitMin, BspawnWaitMax);
            }

            if (IspawnWait <= Time.time)
            {
                randItem = Random.Range(0, items.Length);
                Ipos.x = Random.Range(-9.0f, 9.0f);

                ns_item = Instantiate(items[randItem], transform.position + Ipos, transform.rotation);
                ns_item.transform.parent = transform;
                NetworkServer.Spawn(ns_item);

                IspawnWait = Time.time + Random.Range(IspawnWaitMin, IspawnWaitMax);
            }
        }
    }
}
