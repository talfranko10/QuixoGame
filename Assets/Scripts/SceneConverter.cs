using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneConverter : MonoBehaviour
{
    public void Setup()
    {
        gameObject.SetActive(true);

    }

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }


}
