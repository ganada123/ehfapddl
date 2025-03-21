using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{

    public void ChangeMainScene()
    {
        SceneManager.LoadScene("Main");
    }

    public void ChangeGoListScene()
    {
        SceneManager.LoadScene("GoList");
    }
}
