using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private PlayerInputActions inputActions;

    void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.UI.Quit.performed += ctx => QuitMenu();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        GameManager.initialized = false;
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
                QuitMenu();
                break;
        }
    }


    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    void QuitMenu()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

}
