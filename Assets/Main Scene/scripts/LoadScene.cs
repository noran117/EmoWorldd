using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void LoadHappinessScene()
    {
        SceneManager.LoadScene("Happiness");

    }
    public void LoadSadnessScene()
    {
        SceneManager.LoadScene("Sadness");

    }
    public void LoadAngerScene()
    {
        SceneManager.LoadScene("Anger");

    }
    public void LoadFearScene()
    {
        SceneManager.LoadScene("Fear");

    }
    /// <summary>
    /// Restart the Current level, maybe we can use it in sadness scene 
    /// </summary>
    public void RestartCurrentLevel()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        SceneManager.LoadScene(currentSceneName);
    }

    public void QuitTheApp()
    {
        Application.Quit();
    }

}
/// <summary>
/// We need a UI Widget that contains a Quit button, Sound Control Panel, Mute All Sounds button, and a Score counter if needed  
/// </summary>


