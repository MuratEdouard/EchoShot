using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnDroneClicked(Hoverable clickedDrone)
    {
        switch (clickedDrone.name)
        {
            case "PlayDrone":
                SceneManager.LoadScene("MainScene");
                break;

            case "CreditsDrone":
                SceneManager.LoadScene("CreditsScene");
                break;

            case "QuitDrone":
                Application.Quit();
    #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
    #endif
                break;
        }
    }

}
