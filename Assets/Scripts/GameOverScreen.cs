using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public Text winnerText;
    private GameOverScreen GameOverScreenn;

    void Start()
    {
        
    }
    public void Setup(string winner)
    {
        GameOverScreenn = GameObject.Find("").GetComponent<GameOverScreen>();
        if (GameOverScreenn)
        {
            gameObject.SetActive(true);
            winnerText.text = winner.ToString() + " WIN!";
        }
    }



public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
