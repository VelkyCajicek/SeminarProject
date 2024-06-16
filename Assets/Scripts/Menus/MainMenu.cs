using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    FadeInOut fade;
    private void Start()
    {
        fade = FindObjectOfType<FadeInOut>();
    }
    public IEnumerator ChangeScene()
    {
        fade.FadeIn();
        yield return new WaitForSeconds(1);
        SceneManager.LoadSceneAsync(1);
    }
    public void Playgame()
    {
        // For changing the scene from main menu to main scene
        //SceneManager.LoadSceneAsync(1); // "1" is the index of the scene with the game
        StartCoroutine(ChangeScene());
    }
    public void QuitGame()
    {
        // For the quit button
        Application.Quit();
    }
}
