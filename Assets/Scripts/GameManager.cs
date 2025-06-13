using UnityEngine;

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
        Debug.Log("Quit game pressed");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
