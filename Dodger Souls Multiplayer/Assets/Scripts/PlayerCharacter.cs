using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class PlayerCharacter : NetworkBehaviour
{
    public Sprite[] avatars;
    public Image avatarHUD;
    [SyncVar]
    public int playerAvatar;

    public Text nameText;
    [SyncVar]
    public string playerName;

    [DllImport("__Internal")]
    private static extern void SendName1();
    [DllImport("__Internal")]
    private static extern void SendAvatar1();
    [DllImport("__Internal")]
    private static extern void SendName2();
    [DllImport("__Internal")]
    private static extern void SendAvatar2();
    [DllImport("__Internal")]
    private static extern void GetScore(string str);

    private bool infoSafe = true;

    public Image lifeBar;
    [SyncVar(hook = "OnChangeLife")]
    public float life = 1f;
    [SyncVar(hook = "OnDamaged")]
    private bool vulnerable = true;
    [SyncVar]
    private float damaged;
    private Color transparent;

    public Text soulsText;
    [SyncVar(hook = "OnAddSouls")]
    public float souls;
    private int soulsXtimer = 1;
    private float soulsTimer;
    private float soulsTimerTick = 1f;

    [SyncVar]
    public float Speed = 5f;

    public GameObject DeathFx;
    public GameObject VictoryFx;
    private Color fade;
    private float blackoutTimer;
    private bool animationEnd;

    [SyncVar(hook = "OnDeath")]
    public bool Dead;
    [SyncVar(hook = "OnVictory")]
    public bool Win;

    void Start ()
    {
        if (isClient)
            GameObject.Find("NetworkManager").GetComponent<NetworkManagerHUD>().showGUI = false;

        transform.parent = GameObject.Find("Players").transform;

        lifeBar.fillAmount = life;        
        soulsText.text = souls.ToString("0");

        if (name == "PlayerCharacter(Clone)")
        {
            if (GameObject.Find("P1") == null)
                gameObject.name = "P1";
            else
                gameObject.name = "P2";
        }

        if (isServer)
        {
            CheckHUD();
            RpcCheckHUD();
        }

        if (isLocalPlayer)
            GetProfile();
    }

    public void GetProfile()
    {
        //playerName = name;
        if (name == "P1")
        {
            SendName1();
            SendAvatar1();
            //playerAvatar = 1;
        }
        else if (name == "P2")
        {
            SendName2();
            SendAvatar2();
            //playerAvatar = 2;
        }
        CmdSendProfile(playerName, playerAvatar);
    }

    public void RecieveName(string data)
    {
        playerName = data;
    }

    public void RecieveAvatar(string data)
    {
        playerAvatar = int.Parse(data);
    }

    [Command]
    void CmdSendProfile(string nameP, int avatarP)
    {
        playerName = nameP;
        playerAvatar = avatarP;
    }

    public override void OnStartLocalPlayer()
    {
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        avatarHUD.GetComponent<Image>().color = new Color(1, 1, 1);
    }

    void SetPlayerInfo()
    {
        gameObject.GetComponent<PlayerCharacter>().nameText.text = playerName;
        gameObject.GetComponent<SpriteRenderer>().sprite = avatars[playerAvatar];
        avatarHUD.sprite = avatars[playerAvatar];
    }

    void Update ()
    {        
        SetPlayerInfo();

        if (life > 0f)
        {
            if (GameObject.Find("NetworkManager").GetComponent<Networking>().Players[0] != null &&
                GameObject.Find("NetworkManager").GetComponent<Networking>().Players[1] != null)
            {

                if (infoSafe)
                    infoSafe = false;
            }

            if (isServer)
            {
                if (GameObject.Find("NetworkManager").GetComponent<Networking>().Players[0] != null &&
                    GameObject.Find("NetworkManager").GetComponent<Networking>().Players[1] != null)                
                {
                    if (soulsTimer <= Time.time && !Win)
                    {
                        AddSouls(soulsXtimer);
                        soulsTimer = Time.time + soulsTimerTick;
                    }
                }

                if (!vulnerable && damaged <= Time.time)
                {
                    vulnerable = true;

                    transparent = gameObject.GetComponent<SpriteRenderer>().color;
                    transparent.a = 1f;
                    gameObject.GetComponent<SpriteRenderer>().color = transparent;
                }
            }

            if (isLocalPlayer)
            {
                if (Input.GetKey(KeyCode.LeftArrow))
                    transform.Translate(-(Speed * Time.deltaTime), 0, 0);
                if (Input.GetKey(KeyCode.RightArrow))
                    transform.Translate(Speed * Time.deltaTime, 0, 0);
            }
        }

        if (isClient)
        {
            if (DeathFx.activeInHierarchy && !animationEnd)
            {
                DeathFxAnimation();
            }
            if (VictoryFx.activeInHierarchy && !animationEnd)
            {
                VictoryFxAnimation();
            }
            if ((DeathFx.activeInHierarchy || VictoryFx.activeInHierarchy) && animationEnd)
            {                
                GameObject.Find("NetworkManager").GetComponent<NetworkManagerHUD>().showGUI = true;
                GameObject.Find("NetworkManager").GetComponent<Networking>().StopClient();
            }
        }
    }

    [ClientRpc]
    void RpcCheckHUD()
    {
        CheckHUD();
    }

    void CheckHUD()
    {
        if (transform.position.x < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;

            Vector2 pos = new Vector2(0, 0);

            pos = transform.Find("PlayerHUD").Find("PlayerInfo").GetComponent<RectTransform>().anchoredPosition;
            pos.x = -pos.x;
            transform.Find("PlayerHUD").Find("PlayerInfo").GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
            transform.Find("PlayerHUD").Find("PlayerInfo").GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
            transform.Find("PlayerHUD").Find("PlayerInfo").GetComponent<RectTransform>().anchoredPosition = pos;

            transform.Find("PlayerHUD").Find("PlayerInfo").Find("PlayerAvatar").localScale = new Vector3(-1, 1, 1);

            pos = transform.Find("PlayerHUD").Find("PlayerInfo").Find("PlayerName").GetComponent<RectTransform>().anchoredPosition;
            pos.x = -pos.x;
            transform.Find("PlayerHUD").Find("PlayerInfo").Find("PlayerName").GetComponent<RectTransform>().anchorMin = new Vector2(1, 1);
            transform.Find("PlayerHUD").Find("PlayerInfo").Find("PlayerName").GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            transform.Find("PlayerHUD").Find("PlayerInfo").Find("PlayerName").GetComponent<RectTransform>().anchoredPosition = pos;

            pos = transform.Find("PlayerHUD").Find("PlayerInfo").Find("LifeBar").GetComponent<RectTransform>().anchoredPosition;
            pos.x = -pos.x;
            transform.Find("PlayerHUD").Find("PlayerInfo").Find("LifeBar").GetComponent<RectTransform>().anchorMin = new Vector2(1, 0.5f);
            transform.Find("PlayerHUD").Find("PlayerInfo").Find("LifeBar").GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.5f);
            transform.Find("PlayerHUD").Find("PlayerInfo").Find("LifeBar").GetComponent<RectTransform>().anchoredPosition = pos;
            transform.Find("PlayerHUD").Find("PlayerInfo").Find("LifeBar").GetComponent<Image>().fillOrigin = 0;

            pos = transform.Find("PlayerHUD").Find("PlayerInfo").Find("Souls").GetComponent<RectTransform>().anchoredPosition;
            pos.x = -pos.x;
            transform.Find("PlayerHUD").Find("PlayerInfo").Find("Souls").GetComponent<RectTransform>().anchorMin = new Vector2(1, 0);
            transform.Find("PlayerHUD").Find("PlayerInfo").Find("Souls").GetComponent<RectTransform>().anchorMax = new Vector2(1, 0);
            transform.Find("PlayerHUD").Find("PlayerInfo").Find("Souls").GetComponent<RectTransform>().anchoredPosition = pos;
        }
    }

    public void ChangeLife(float amount, bool attack)
    {
        if (!isServer)
            return;

        if (!attack)
        {
            life += amount;
            if (life > 1f)
                life = 1f;
            lifeBar.fillAmount = life;
        }
        else if (attack && vulnerable)
        {
            life -= amount;
            lifeBar.fillAmount = life;

            vulnerable = false;
            damaged = Time.time + 1;
            transparent = gameObject.GetComponent<SpriteRenderer>().color;
            transparent.a = 0.66f;
            gameObject.GetComponent<SpriteRenderer>().color = transparent;
        }
    }

    void OnChangeLife(float amount)
    {
        life = amount;
        lifeBar.fillAmount = life;
    }

    void OnDamaged(bool vulnerable)
    {
        if (!vulnerable)
        {
            transparent = gameObject.GetComponent<SpriteRenderer>().color;
            transparent.a = 0.66f;
            gameObject.GetComponent<SpriteRenderer>().color = transparent;
            Speed = 3f;
        }
        else if (vulnerable)
        {
            transparent = gameObject.GetComponent<SpriteRenderer>().color;
            transparent.a = 1f;
            gameObject.GetComponent<SpriteRenderer>().color = transparent;
            Speed = 5f;
        }
    }

    void OnDeath(bool Dead)
    {
        if(Dead)
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 0.01f;

            if (isLocalPlayer)
            {
                DeathFx.SetActive(true);
                var finalSouls = souls * 0.2f;
                DeathFx.transform.Find("Souls").Find("TextS").GetComponent<Text>().text = finalSouls.ToString("0");
                GetScore(DeathFx.transform.Find("Souls").Find("TextS").GetComponent<Text>().text);
                blackoutTimer = Time.time + 8;

                CmdPlayerDeath();
            }
        }
    }

    [Command]
    void CmdPlayerDeath()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 0.01f;
    }

    void OnVictory(bool Win)
    {
        if (Win)
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionY;

            if (isLocalPlayer)
            {
                VictoryFx.SetActive(true);
                var finalSouls = souls;
                VictoryFx.transform.Find("Souls").Find("TextS").GetComponent<Text>().text = finalSouls.ToString("0");
                GetScore(VictoryFx.transform.Find("Souls").Find("TextS").GetComponent<Text>().text);
                blackoutTimer = Time.time + 8;

                CmdPlayerVictory();
            }
        }
    }

    [Command]
    void CmdPlayerVictory()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionY;
    }

    public void AddSouls(float amount)
    {
        if (!isServer)
            return;

        souls += amount;
        soulsText.text = souls.ToString("0");
    }

    void OnAddSouls(float amount)
    {
        souls = amount;
        soulsText.text = souls.ToString("0");
    }

    void DeathFxAnimation()
    {
        if (DeathFx.transform.Find("Souls").Find("TextS").GetComponent<Text>().color.a < 1)
        {
            fade = DeathFx.transform.Find("GrayMask").GetComponent<RawImage>().color;
            if (fade.a < 0.5f)
            {
                fade.a += 0.083f * Time.deltaTime;
                DeathFx.transform.Find("GrayMask").GetComponent<RawImage>().color = fade;
            }
            fade = DeathFx.transform.Find("YouDied").GetComponent<Image>().color;
            if (fade.a < 0.855f)
            {
                fade.a += 0.285f * Time.deltaTime;
                DeathFx.transform.Find("YouDied").GetComponent<Image>().color = fade;
            }
            fade = DeathFx.transform.Find("YouDied").Find("TextYD").GetComponent<Text>().color;
            if (fade.a < 0.666f)
            {
                fade.a += 0.111f * Time.deltaTime;
                DeathFx.transform.Find("YouDied").Find("TextYD").GetComponent<Text>().color = fade;
            }
            fade = DeathFx.transform.Find("Souls").GetComponent<Image>().color;
            if (fade.a < 1f)
            {
                fade.a += 0.333f * Time.deltaTime;
                DeathFx.transform.Find("Souls").GetComponent<Image>().color = fade;
            }
            fade = DeathFx.transform.Find("Souls").Find("TextS").GetComponent<Text>().color;
            if (fade.a < 1f)
            {
                fade.a += 0.166f * Time.deltaTime;
                DeathFx.transform.Find("Souls").Find("TextS").GetComponent<Text>().color = fade;
            }
        }
        else if (blackoutTimer < Time.time && GameObject.Find("EndScreen").transform.Find("BlackMask").GetComponent<RawImage>().color.a < 1)
        {
            fade = GameObject.Find("EndScreen").transform.Find("BlackMask").GetComponent<RawImage>().color;
            if (fade.a < 1f)
            {
                fade.a += 0.5f * Time.deltaTime;
                GameObject.Find("EndScreen").transform.Find("BlackMask").GetComponent<RawImage>().color = fade;
            }
        }
        else if (GameObject.Find("EndScreen").transform.Find("BlackMask").GetComponent<RawImage>().color.a >= 1)
            animationEnd = true;
    }

    void VictoryFxAnimation()
    {
        if (VictoryFx.transform.Find("Souls").Find("TextS").GetComponent<Text>().color.a < 1)
        {
            fade = VictoryFx.transform.Find("Victory").GetComponent<Image>().color;
            if (fade.a < 0.855f)
            {
                fade.a += 0.285f * Time.deltaTime;
                VictoryFx.transform.Find("Victory").GetComponent<Image>().color = fade;
            }
            fade = VictoryFx.transform.Find("Victory").Find("TextV").GetComponent<Text>().color;
            if (fade.a < 0.666f)
            {
                fade.a += 0.111f * Time.deltaTime;
                VictoryFx.transform.Find("Victory").Find("TextV").GetComponent<Text>().color = fade;
            }
            fade = VictoryFx.transform.Find("Souls").GetComponent<Image>().color;
            if (fade.a < 1f)
            {
                fade.a += 0.333f * Time.deltaTime;
                VictoryFx.transform.Find("Souls").GetComponent<Image>().color = fade;
            }
            fade = VictoryFx.transform.Find("Souls").Find("TextS").GetComponent<Text>().color;
            if (fade.a < 1f)
            {
                fade.a += 0.166f * Time.deltaTime;
                VictoryFx.transform.Find("Souls").Find("TextS").GetComponent<Text>().color = fade;
            }
        }
        else if (blackoutTimer < Time.time && GameObject.Find("EndScreen").transform.Find("BlackMask").GetComponent<RawImage>().color.a < 1)
        {
            fade = GameObject.Find("EndScreen").transform.Find("BlackMask").GetComponent<RawImage>().color;
            if (fade.a < 1f)
            {
                fade.a += 0.5f * Time.deltaTime;
                GameObject.Find("EndScreen").transform.Find("BlackMask").GetComponent<RawImage>().color = fade;
            }
        }
        else if (GameObject.Find("EndScreen").transform.Find("BlackMask").GetComponent<RawImage>().color.a >= 1)
            animationEnd = true;
    }
}
