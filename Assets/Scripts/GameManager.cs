using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static float gameplaySpeed = 1f;

    private PlayerInputActions inputActions;

    void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.UI.Quit.performed += ctx => QuitGame(); // Add binding in input actions!
    }

    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    void QuitGame()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
