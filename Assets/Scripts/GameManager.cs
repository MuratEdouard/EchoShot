using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [Header("Game Logic")]
    public Transform enemiesParent;
    public float checkInterval = 1f; // Check every second
    public static float gameplaySpeed = 1f;

    private float checkTimer = 0f;
    private PlayerInputActions inputActions;

    void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.UI.Quit.performed += ctx => QuitGame();
    }

    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    void Update()
    {
        checkTimer += Time.deltaTime;
        if (checkTimer >= checkInterval)
        {
            checkTimer = 0f;

            if (enemiesParent != null && enemiesParent.childCount == 0)
            {
                Debug.Log("All enemies defeated. Restarting scene...");
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void QuitGame()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
