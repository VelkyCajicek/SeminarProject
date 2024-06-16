using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void Playgame()
    {
        // For changing the scene from main menu to main scene
        SceneManager.LoadSceneAsync(2); // "1" is the index of the scene with the game
    }
}
