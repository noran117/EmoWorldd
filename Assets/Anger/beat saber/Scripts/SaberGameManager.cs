using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SaberGameManager : MonoBehaviour
{
    public static SaberGameManager Instance;

    [Header("Game State")]
    public bool gameRunning;
    public int score;
    public int winScore = 100;
    public bool gameLocked;

    [Header("UI")]
    public TMP_Text scoreText;
    public GameObject winPanel;
    public GameObject wrongX;

    [Header("Red Screen Overlay")]
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

    public Spawner spawner;

    void Awake()
    {
        Instance = this;
        Debug.Log("GameManager Awake on object: " + gameObject.name);

        score = 0;
        gameRunning = false;
        gameLocked = false;

        RefreshUI();

        if (winPanel) winPanel.SetActive(false);
        if (wrongX) wrongX.SetActive(false);
        if (redOverlay) redOverlay.SetActive(false);
        if (scoreText) scoreText.gameObject.SetActive(false);

        UpdateRedOverlay();
    }

    public void SetRunning(bool running)
    {
        if (gameLocked && running)
            return;

        gameRunning = running;

        if (spawner) spawner.SetSpawnerRunning(running);

        Debug.Log("SetRunning called on: " + gameObject.name + " | gameRunning = " + gameRunning);

        if (running)
        {
            if (redOverlay) redOverlay.SetActive(true);
            if (scoreText) scoreText.gameObject.SetActive(true);

            if (backgroundMusic) backgroundMusic.Stop();
            if (gameMusic && !gameMusic.isPlaying) gameMusic.Play();

            UpdateRedOverlay();
        }
        else
        {
            if (scoreText) scoreText.gameObject.SetActive(false);

            if (gameMusic) gameMusic.Stop();
            if (backgroundMusic && !backgroundMusic.isPlaying) backgroundMusic.Play();
        }
    }
    public void RestartGame()
    {
        score = 0;
        RefreshUI();

        gameLocked = false;

        if (winPanel) winPanel.SetActive(false);
        if (wrongX) wrongX.SetActive(false);

        NoteHitState[] notes = FindObjectsByType<NoteHitState>(FindObjectsSortMode.None);

        foreach (var n in notes)
        {
            Destroy(n.gameObject);
        }

        if (redOverlay) redOverlay.SetActive(false);

        SetRunning(false);
        UpdateRedOverlay();
    }

    public void AddScore(int amount)
    {
        score += amount;
        RefreshUI();
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
        if (sfx && hitCorrect) sfx.PlayOneShot(hitCorrect);
    }

    public void PlayWrong()
    {
        if (sfx && hitWrong) sfx.PlayOneShot(hitWrong);
        if (wrongX) StartCoroutine(ShowX());
    }

    System.Collections.IEnumerator ShowX()
    {
        wrongX.SetActive(true);
        yield return new WaitForSeconds(0.35f);
        wrongX.SetActive(false);
    }

    void Win()
    {
        gameLocked = true;

        SetRunning(false);

        if (gameMusic) gameMusic.Stop();

        if (sfx && winClip) sfx.PlayOneShot(winClip);
        if (winPanel) winPanel.SetActive(true);

        if (redOverlay) redOverlay.SetActive(false);
    }

    void RefreshUI()
    {
        if (scoreText) scoreText.text = $"Score: {score}";
    }
}