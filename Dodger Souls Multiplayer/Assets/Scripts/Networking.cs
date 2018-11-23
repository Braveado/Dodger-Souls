using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Networking : NetworkManager
{
    public GameObject[] Players = new GameObject[2];
    string ipAdress;
    private bool End;

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        base.OnServerAddPlayer(conn, playerControllerId);
        if (conn.playerControllers.Count > 0)
        {
            if (End)
                End = false;

            GameObject player = conn.playerControllers[0].gameObject;

            if (Players[0] == null)
                Players[0] = player;
            else if (Players[1] == null)
                Players[1] = player;
        }
    }

    //void Start()
    //{
    //    var ipaddress = Network.player.ipAddress;
    //    GameObject.Find("IPtext").GetComponent<Text>().text = ipaddress;
    //}


    void Update()
    {
        if (!End && Players[0] != null && Players[1] != null)
        {
            if (Players[0].GetComponent<PlayerCharacter>().life <= 0)
            {
                Players[0].GetComponent<PlayerCharacter>().Dead = true;
                Players[1].GetComponent<PlayerCharacter>().Win = true;
                End = true;
            }
            else if (Players[1].GetComponent<PlayerCharacter>().life <= 0)
            {
                Players[1].GetComponent<PlayerCharacter>().Dead = true;
                Players[0].GetComponent<PlayerCharacter>().Win = true;
                End = true;
            }
        }
    }

};
