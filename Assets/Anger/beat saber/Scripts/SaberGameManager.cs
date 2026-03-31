using UnityEngine;
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
    public GameObject winPanel;
    public GameObject wrongX;
    public GameObject redOverlay;
    [Range(0f, 1f)] public float startRedAlpha = 0.6f;

    [Header("Win")]
    public ParticleSystem winParticles;
    public float winPanelDuration = 5f;

    [Header("Audio Sources")]
    public AudioSource hitCorrectSource;
    public AudioSource hitWrongSource;
    public AudioSource winSource;

    [Header("Music")]
    public AudioSource gameMusic;
    public AudioSource backgroundMusic;

    [Header("Spawner")]
    public Spawner spawner;

    bool endingTriggered;

    void Awake()
    {
        Instance = this;

        if (spawner == null)
            spawner = FindFirstObjectByType<Spawner>();

        score = 0;
        gameRunning = false;
        gameLocked = false;
        endingTriggered = false;

        if (gameUIRoot) gameUIRoot.SetActive(false);
        if (winPanel) winPanel.SetActive(false);
        if (wrongX) wrongX.SetActive(false);
        if (redOverlay) redOverlay.SetActive(false);

        if (winParticles != null)
            winParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        UpdateRedOverlay();
    }

    public void SetRunning(bool running)
    {
        if (gameLocked && running) return;

        gameRunning = running;

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

            StartCoroutine(WatchMusicEnd());
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
    }

    public void AddScore(int amount)
    {
        if (endingTriggered) return;

        score += amount;

        if (score < 0)
            score = 0;

        Debug.Log("Score = " + score);

        UpdateRedOverlay();

        if (score >= winScore)
            Win();
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
        if (hitCorrectSource != null)
            hitCorrectSource.Play();
    }

    public void PlayWrong()
    {
        if (hitWrongSource != null)
            hitWrongSource.Play();

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
        if (endingTriggered) return;
        endingTriggered = true;

        Debug.Log("WIN CALLED");

        gameLocked = true;
        gameRunning = false;

        if (spawner != null)
            spawner.StopSpawning();

        if (gameMusic && gameMusic.isPlaying)
            gameMusic.Stop();

        if (backgroundMusic && !backgroundMusic.isPlaying)
            backgroundMusic.Play();

        if (redOverlay)
            redOverlay.SetActive(false);

        if (wrongX)
            wrongX.SetActive(false);

        if (winParticles != null)
            winParticles.Play();

        if (winSource != null)
            winSource.Play();

        StartCoroutine(ShowWinPanelThenHide());
    }

    IEnumerator ShowWinPanelThenHide()
    {
        if (winPanel != null)
            winPanel.SetActive(true);

        yield return new WaitForSeconds(winPanelDuration);

        if (winPanel != null)
            winPanel.SetActive(false);
    }

    // Watches the music clip and cleanly stops the game when the song ends
    IEnumerator WatchMusicEnd()
    {
        // Wait until the music finishes playing
        yield return new WaitUntil(() =>
            gameMusic == null ||
            !gameMusic.isPlaying ||
            endingTriggered);

        // Only act if the game is still running (not already won)
        if (!endingTriggered && gameRunning)
        {
            Debug.Log("Music ended — stopping game.");

            gameRunning = false;
            gameLocked  = false;   // allow player to restart by grabbing sabers again

            if (spawner != null)
                spawner.StopSpawning();

            if (gameUIRoot) gameUIRoot.SetActive(false);
            if (redOverlay) redOverlay.SetActive(false);

            if (backgroundMusic && !backgroundMusic.isPlaying)
                backgroundMusic.Play();
        }
    }
}