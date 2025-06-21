using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{

    [Header("Game Logic")]
    public Transform enemiesParent;
    public float checkInterval = 1f;
    public float winDelay = 2f;
    public CanvasGroup winFadePanel;
    public CanvasGroup lossFadePanel;
    public TMP_Text enemiesLeftText;
    public TMP_Text timeLeftText;
    public TMP_Text nbEnemiesDefeatedWinText;
    public TMP_Text nbEnemiesDefeatedLossText;
    public float gameplayTimeMax = 60.0f;

    [Header("Static Logic")]
    public static float gameplaySpeed = 1f;
    public static int initialNbEnemiesSummoned = 4;
    public static int nbEnemiesSummoned;
    public static int nbEnemiesDefeated = 0;
    public static bool initialized = false;
    
    private float checkTimer = 0f;
    private bool winTriggered = false;
    private bool lossTriggered = false;
    private float gameplayTimeLeft;

    private PlayerInputActions inputActions;

    void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.UI.Quit.performed += ctx => QuitGame();

        if (!initialized)
        {
            nbEnemiesSummoned = initialNbEnemiesSummoned;
            initialized = true;
        }
        gameplayTimeLeft = gameplayTimeMax;
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
        if (winTriggered || lossTriggered) return;

        checkTimer += Time.deltaTime;
        if (checkTimer >= checkInterval)
        {
            checkTimer = 0f;

            enemiesLeftText.text = enemiesParent.childCount.ToString() + " Enemies Left";

            if (enemiesParent != null && enemiesParent.childCount == 0)
            {
                winTriggered = true;
                StartCoroutine(WinSequence());
            }
        }

        gameplayTimeLeft -= Time.deltaTime * gameplaySpeed;
        if (gameplayTimeLeft <= 0f)
        {
            gameplayTimeLeft = 0f;

            if (!lossTriggered && enemiesParent != null && enemiesParent.childCount > 0)
            {
                lossTriggered = true;
                StartCoroutine(LossSequence());
                return;
            }
        }

        timeLeftText.text = gameplayTimeLeft.ToString("0.##");
    }

    private IEnumerator WinSequence()
    {
        nbEnemiesDefeatedWinText.text = nbEnemiesDefeated.ToString() + " enemies defeated";

        winFadePanel.blocksRaycasts = true;
        winFadePanel.interactable = false;

        float fadeDuration = 1.5f;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            if (winFadePanel != null)
                winFadePanel.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }

        yield return new WaitForSeconds(3.0f); // short pause after full fade

        nbEnemiesSummoned *= 2;

        SceneManager.LoadScene("MainScene");
        gameplayTimeLeft = gameplayTimeMax;
    }

    private IEnumerator LossSequence()
    {
        lossFadePanel.blocksRaycasts = true;
        lossFadePanel.interactable = false;
        nbEnemiesDefeatedLossText.text = nbEnemiesDefeated.ToString() + " enemies defeated";

        float fadeDuration = 1.5f;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            if (lossFadePanel != null)
                lossFadePanel.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }

        yield return new WaitForSeconds(3.0f); // short pause after full fade

        nbEnemiesSummoned = initialNbEnemiesSummoned;

        SceneManager.LoadScene("MainScene");
        gameplayTimeLeft = gameplayTimeMax;
    }


    void QuitGame()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
