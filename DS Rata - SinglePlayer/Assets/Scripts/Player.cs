using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public Sprite[] avatars;    
    public Image avatarHUD;
    private int randAvatar;

    public Image lifeBar;
    public float life = 1f;
    private bool vulnerable = true;

    public Text soulsText;
    private float souls = 0;
    public int soulsXsecond = 1;

    public float speed = 5f;
    private float step;

    public GameObject Death;
    public GameObject Victory;

    public bool win = false;

	// Use this for initialization
	void Start ()
    {
        randAvatar = Random.Range(0, avatars.Length);
        gameObject.GetComponent<SpriteRenderer>().sprite = avatars[randAvatar];
        avatarHUD.sprite = avatars[randAvatar];

        lifeBar.fillAmount = life;
        soulsText.text = souls.ToString("0");
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (life > 0)
        {
            if(win == false)
                AddSouls(soulsXsecond * Time.deltaTime);

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                step = speed * Time.deltaTime;
                transform.Translate(-step, 0, 0);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                step = speed * Time.deltaTime;
                transform.Translate(step, 0, 0);
            }
        }

        if (souls >= 100 && win == false)
        {
            win = true;
            StartCoroutine(Win());
        }
    }

    public void ChangeLife(float amount, bool attack)
    {
        if (attack == true && vulnerable == true)
        {
            life -= amount;
            lifeBar.fillAmount = life;
            StartCoroutine(Damaged());
        }
        else if (attack == false)
        {
            life += amount;
            lifeBar.fillAmount = life;
        }
    }

    public void AddSouls(float amount)
    {
        souls += amount;
        if (souls > 999999)
            souls = 999999;
        soulsText.text = souls.ToString("0");
    }

    IEnumerator Damaged()
    {
        vulnerable = false;

        for (int i = 0; i < 3; i++)
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(0.15f);

            gameObject.GetComponent<SpriteRenderer>().enabled = true;
            yield return new WaitForSeconds(0.15f);
        }

        if (life <= 0)
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;

            Death.SetActive(true);
            DeathFx();
            yield return new WaitForSeconds(8f);

            StartCoroutine(Fade(Death.transform.Find("BlackMask").GetComponent<RawImage>().color, 16, 1.0f, 1.75f));
            yield return new WaitForSeconds(2f);

            //Cursor.visible = true;
            SceneManager.LoadScene(0);
        }
        else
            vulnerable = true;
    }

    void DeathFx()
    {
        float compensationSouls = souls * 0.2f;
        Death.transform.Find("Souls").Find("TextS").GetComponent<Text>().text = compensationSouls.ToString("0");

        StartCoroutine(Fade(Death.transform.Find("GrayMask").GetComponent<RawImage>().color, 11, 0.5f, 5f));
        StartCoroutine(Fade(Death.transform.Find("YouDied").GetComponent<Image>().color, 12, 0.85f, 3f));
        StartCoroutine(Fade(Death.transform.Find("YouDied").Find("TextYD").GetComponent<Text>().color, 13, 0.65f, 5f));
        StartCoroutine(Fade(Death.transform.Find("Souls").GetComponent<Image>().color, 14, 1.0f, 3f));
        StartCoroutine(Fade(Death.transform.Find("Souls").Find("TextS").GetComponent<Text>().color, 15, 1.0f, 3f));
    }

    IEnumerator Fade(Color targetImage, int returnIndex, float wantedAlpha, float seconds)
    {
        float initialAlpha = targetImage.a;
        for (float percent = 0.0f; percent < 1.0f; percent += Time.deltaTime / seconds)
        {
            targetImage.a = Mathf.Lerp(initialAlpha, wantedAlpha, percent);

            switch(returnIndex)
            {
                case 11:
                    Death.transform.Find("GrayMask").GetComponent<RawImage>().color = targetImage;
                    break;
                case 12:
                    Death.transform.Find("YouDied").GetComponent<Image>().color = targetImage;
                    break;
                case 13:
                    Death.transform.Find("YouDied").Find("TextYD").GetComponent<Text>().color = targetImage;
                    break;
                case 14:
                    Death.transform.Find("Souls").GetComponent<Image>().color = targetImage;
                    break;
                case 15:
                    Death.transform.Find("Souls").Find("TextS").GetComponent<Text>().color = targetImage;
                    break;
                case 16:
                    Death.transform.Find("BlackMask").GetComponent<RawImage>().color = targetImage;
                    break;
                case 21:
                    Victory.transform.Find("Victory").GetComponent<Image>().color = targetImage;
                    break;
                case 22:
                    Victory.transform.Find("Victory").Find("TextV").GetComponent<Text>().color = targetImage;
                    break;
                case 23:
                    Victory.transform.Find("Souls").GetComponent<Image>().color = targetImage;
                    break;
                case 24:
                    Victory.transform.Find("Souls").Find("TextS").GetComponent<Text>().color = targetImage;
                    break;
                case 25:
                    Victory.transform.Find("BlackMask").GetComponent<RawImage>().color = targetImage;
                    break;
            }

            yield return null;
        }
    }

    void VictoryFx()
    {
        Victory.transform.Find("Souls").Find("TextS").GetComponent<Text>().text = souls.ToString("0");

        StartCoroutine(Fade(Victory.transform.Find("Victory").GetComponent<Image>().color, 21, 0.85f, 3f));
        StartCoroutine(Fade(Victory.transform.Find("Victory").Find("TextV").GetComponent<Text>().color, 22, 0.65f, 5f));
        StartCoroutine(Fade(Victory.transform.Find("Souls").GetComponent<Image>().color, 23, 1.0f, 3f));
        StartCoroutine(Fade(Victory.transform.Find("Souls").Find("TextS").GetComponent<Text>().color, 24, 1.0f, 3f));
    }

    IEnumerator Win()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionY;

        Victory.SetActive(true);
        VictoryFx();
        yield return new WaitForSeconds(8f);

        StartCoroutine(Fade(Victory.transform.Find("BlackMask").GetComponent<RawImage>().color, 25, 1.0f, 1.75f));
        yield return new WaitForSeconds(2f);

        //Cursor.visible = true;
        SceneManager.LoadScene(0);
    }
}
