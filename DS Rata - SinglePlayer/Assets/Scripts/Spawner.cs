using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] enemies;
    private int randEnemy;
    private Vector3 Epos;

    public float EspawnWaitIni = 3f;
    public float EspawnWaitMax = 1f;
    public float EspawnWaitMin = 0.33f;
    private float EspawnWait;

    public GameObject[] items;
    private int randItem;
    private Vector3 Ipos;

    public float IspawnWaitIni = 10f;
    public float IspawnWaitMax = 10f;
    public float IspawnWaitMin = 5f;
    private float IspawnWait;

    public GameObject boss;
    private Vector3 Bpos;

    public float BspawnWaitIni = 20f;
    public float BspawnWaitMax = 20f;
    public float BspawnWaitMin = 10f;
    private float BspawnWait;

    public Player player;
    private bool stop = false;

	// Use this for initialization
	void Start ()
    {
        //Cursor.visible = false;
        StartCoroutine(SpawnEnemies());
        StartCoroutine(SpawnItems());
        StartCoroutine(SpawnBoss());
    }
	
	// Update is called once per frame
	void Update ()
    {
        if ((player.win == true || player.life <= 0) && stop == false)
            stop = true;
    }

    IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(EspawnWaitIni);

        while (stop == false)
        {
            randEnemy = Random.Range(0, enemies.Length);
            Epos.x = Random.Range(-9.0f, 9.0f);

            if(randEnemy == 2)
            {
                if (Random.Range(0, 2) == 1)
                    Instantiate(enemies[randEnemy], transform.position - Epos, transform.rotation);
                Instantiate(enemies[randEnemy], transform.position + Epos, transform.rotation);
            }
            else
                Instantiate(enemies[randEnemy], transform.position + Epos, transform.rotation);

            EspawnWait = Random.Range(EspawnWaitMin, EspawnWaitMax);
            yield return new WaitForSeconds(EspawnWait);
        }
    }

    IEnumerator SpawnItems()
    {
        yield return new WaitForSeconds(IspawnWaitIni);

        while (stop == false)
        {
            randItem = Random.Range(0, items.Length);
            Ipos.x = Random.Range(-9.0f, 9.0f);

            Instantiate(items[randItem], transform.position + Ipos, transform.rotation);

            IspawnWait = Random.Range(IspawnWaitMin, IspawnWaitMax);
            yield return new WaitForSeconds(IspawnWait);
        }
    }

    IEnumerator SpawnBoss()
    {
        yield return new WaitForSeconds(BspawnWaitIni);

        while (stop == false)
        {
            Bpos.x = Random.Range(-7.0f, 7.0f);

            Instantiate(boss, transform.position + Bpos, transform.rotation);

            BspawnWait = Random.Range(BspawnWaitMin, BspawnWaitMax);
            yield return new WaitForSeconds(BspawnWait);
        }
    }
}
