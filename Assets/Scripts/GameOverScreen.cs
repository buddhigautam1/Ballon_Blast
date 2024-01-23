using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameOverScreen : MonoBehaviour
{
    public void restartGame()
    {
        SceneManager.LoadScene("Game");
    }
    public void menuGame()
    {
        SceneManager.LoadScene("Menu");
    }

}
