using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [Space(10)]
    [Header("Ui Manager")]
    public float fadeDuration = 3f;
    private bool isDead = false;
    private bool loadMainMenuScene = false;

    [Space(10)]
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject player;
    [SerializeField] private EnemySpawner enemySpawnerManager;

    [Space(10)]
    [Header("WinScreenManager")]
    public float blinkInterval = 1f;
    [SerializeField]private bool isBlinking = false;
    public TMP_Text pressStartText;

    private void Start()
    {
        pressStartText.enabled = false;
        canvasGroup.alpha = 1.0f;
        StartCoroutine(FadeOut());
        SetCursorVisible(false);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!loadMainMenuScene && canvasGroup.alpha < 0.1)
            {
                TurnOffPlayerComponents();
                StartCoroutine(FadeInAndLoadScene("Main_Menu"));
                loadMainMenuScene = true;
            }
        }
        if (enemySpawnerManager.enemyCount == 0)
        {
            if (canvasGroup.alpha <= 0)
            {
                StartCoroutine(FadeIn());
                TurnOffPlayerComponents();
            }

            if (canvasGroup.alpha == 1 && !isBlinking)
            {
                StartCoroutine(BlinkText());
                isBlinking = true;

            }

            if (isBlinking && Input.GetKeyDown(KeyCode.R))
            {
                Scene currentScene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(currentScene.buildIndex);
            }
        }
    }

    public void SetCursorVisible(bool state)
    {
        Cursor.visible = state;
        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void LoadSceneByName(string sceneName)
    {
            SceneManager.LoadScene(sceneName);
    }

    private void TurnOffPlayerComponents()
    {
        player.GetComponent<ThirdPersonMovement>().enabled = false;
        player.GetComponent<PlayerController>().enabled = false;
        player.GetComponent<PickUpDrop>().enabled = false;
        player.GetComponent<ThirdPersonShooterController>().enabled = false;
        player.GetComponent<Animator>().enabled = false;
    }

    private IEnumerator BlinkText()
    {
        while (true)
        {
            pressStartText.enabled = !pressStartText.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    public IEnumerator FadeIn()
    {
        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - startTime < fadeDuration)
        {
            float t = (Time.realtimeSinceStartup - startTime) / fadeDuration;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
        canvasGroup.alpha = 1f;
    }

    public IEnumerator FadeOut()
    {
        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - startTime < fadeDuration)
        {
            float t = (Time.realtimeSinceStartup - startTime) / fadeDuration;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
        canvasGroup.alpha = 0f;
    }

    public IEnumerator FadeInAndLoadCurrentScene()
    {
        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - startTime < fadeDuration)
        {
            float t = (Time.realtimeSinceStartup - startTime) / fadeDuration;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
        canvasGroup.alpha = 1f;
        yield return new WaitForSecondsRealtime(0.1f);

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
        yield return null;
    }

    public IEnumerator FadeOutAndLoadCurrentScene()
    {
        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - startTime < fadeDuration)
        {
            float t = (Time.realtimeSinceStartup - startTime) / fadeDuration;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
        canvasGroup.alpha = 0f;
        yield return new WaitForSecondsRealtime(0.1f);

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
        yield return null;
    }

    public IEnumerator FadeInAndLoadScene(string sceneName)
    {
        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - startTime < fadeDuration)
        {
            float t = (Time.realtimeSinceStartup - startTime) / fadeDuration;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
        canvasGroup.alpha = 1f;
        yield return new WaitForSecondsRealtime(0.1f);

        LoadSceneByName(sceneName);
        yield return null;
    }

    public IEnumerator FadeOutAndLoadScene(string sceneName)
    {
        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - startTime < fadeDuration)
        {
            float t = (Time.realtimeSinceStartup - startTime) / fadeDuration;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
        canvasGroup.alpha = 0f;
        yield return new WaitForSecondsRealtime(0.1f);

        LoadSceneByName(sceneName);
        yield return null;
    }
}
