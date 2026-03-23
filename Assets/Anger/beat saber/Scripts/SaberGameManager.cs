using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class SaberGameManager : MonoBehaviour
{
    public static SaberGameManager Instance;

    [Header("Game State")]
    public bool gameRunning;
    public bool gameLocked;
    public int score;
    public int winScore = 1000;

    [Header("UI")]
    public GameObject gameUIRoot;
    public TMP_Text scoreText;
    public GameObject winPanel;
    public GameObject wrongX;
    public GameObject redOverlay;
    [Range(0f, 1f)] public float startRedAlpha = 0.6f;

    [Header("Audio")]
    public AudioSource sfx;
    public AudioClip hitCorrect;
    public AudioClip hitWrong;
    public AudioClip winClip;

    [Header("Music")]
    public AudioSource gameMusic;
    public AudioSource backgroundMusic;

    [Header("Spawner")]
    public Spawner spawner;

    void Awake()
    {
        Instance = this;
        Debug.Log("SaberGameManager Awake on: " + gameObject.name);

        if (spawner == null)
            spawner = FindFirstObjectByType<Spawner>();

        Debug.Log("Spawner ref = " + (spawner ? spawner.name : "NULL"));
        Debug.Log("gameUIRoot ref = " + (gameUIRoot ? gameUIRoot.name : "NULL"));
        Debug.Log("redOverlay ref = " + (redOverlay ? redOverlay.name : "NULL"));
        Debug.Log("gameMusic ref = " + (gameMusic ? gameMusic.name : "NULL"));
        Debug.Log("backgroundMusic ref = " + (backgroundMusic ? backgroundMusic.name : "NULL"));

        score = 0;
        gameRunning = false;
        gameLocked = false;

        if (gameUIRoot) gameUIRoot.SetActive(false);
        if (winPanel) winPanel.SetActive(false);
        if (wrongX) wrongX.SetActive(false);
        if (redOverlay) redOverlay.SetActive(false);

        RefreshUI();
        UpdateRedOverlay();
    }

    public void SetRunning(bool running)
    {
        if (gameLocked && running) return;

        gameRunning = running;
        Debug.Log("SetRunning called | gameRunning = " + gameRunning);

        if (spawner != null)
        {
            if (running) spawner.StartSpawning();
            else spawner.StopSpawning();
        }

        if (running)
        {
            if (gameUIRoot) gameUIRoot.SetActive(true);
            if (redOverlay) redOverlay.SetActive(true);

            if (backgroundMusic && backgroundMusic.isPlaying)
                backgroundMusic.Stop();

            if (gameMusic && !gameMusic.isPlaying)
                gameMusic.Play();
        }
        else
        {
            if (gameUIRoot) gameUIRoot.SetActive(false);
            if (redOverlay) redOverlay.SetActive(false);

            if (gameMusic && gameMusic.isPlaying)
                gameMusic.Stop();

            if (backgroundMusic && !backgroundMusic.isPlaying)
                backgroundMusic.Play();
        }

        RefreshUI();
    }


    public void AddScore(int amount)
    {
        score += amount;

        if (score < 0)
            score = 0;

        Debug.Log("Score = " + score);

        RefreshUI();
        UpdateRedOverlay();

        if (score >= winScore)
            Win();
    }

    void RefreshUI()
    {
        if (scoreText)
            scoreText.text = "Score: " + score;
    }

    void UpdateRedOverlay()
    {
        if (!redOverlay) return;

        Image img = redOverlay.GetComponent<Image>();
        if (!img) return;

        float t = Mathf.Clamp01((float)score / winScore);
        float alpha = Mathf.Lerp(startRedAlpha, 0f, t);

        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    public void PlayCorrect()
    {
        if (sfx && hitCorrect) sfx.PlayOneShot(hitCorrect);
    }

    public void PlayWrong()
    {
        if (sfx && hitWrong) sfx.PlayOneShot(hitWrong);

        if (wrongX)
            StartCoroutine(ShowWrongX());
    }

    IEnumerator ShowWrongX()
    {
        wrongX.SetActive(true);
        yield return new WaitForSeconds(0.35f);
        wrongX.SetActive(false);
    }

    void Win()
    {
        Debug.Log("WIN CALLED");

        gameLocked = true;

        if (spawner != null)
            spawner.StopSpawning();

        gameRunning = false;

        if (gameMusic && gameMusic.isPlaying)
            gameMusic.Stop();

        if (backgroundMusic && !backgroundMusic.isPlaying)
            backgroundMusic.Play();

        if (gameUIRoot)
            gameUIRoot.SetActive(true);

        if (sfx && winClip)
            sfx.PlayOneShot(winClip);

        if (winPanel != null)
        {
            Debug.Log("Activating winPanel: " + winPanel.name);
            winPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("winPanel is NULL");
        }
    }
}