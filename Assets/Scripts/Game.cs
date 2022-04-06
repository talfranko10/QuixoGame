using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public GameOverScreen GameOverScreen;

    public void GameOver()
    {
        GameOverScreen.Setup("");
    }


    //public SceneConverter SceneConverter;

    //public void StartGame()
    //{
    //    SceneConverter.Setup();
    //}
    // void Play()
    // {
    //     Board.startGame();
    // }
}
