using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public void endGame()
    {
        print("game ended");
        Application.Quit();
    }
}
