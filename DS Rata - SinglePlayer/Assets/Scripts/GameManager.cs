using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {        
        //DontDestroyOnLoad(gameObject);
    }
	
	// Update is called once per frame
	void Update ()
    {

	}

    public void LoadScene (int index)
    {
        StartCoroutine(LoadwSound(index));
    }

    IEnumerator LoadwSound(int index)
    {
        yield return new WaitForSeconds(0.8f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(index);
    }
}
