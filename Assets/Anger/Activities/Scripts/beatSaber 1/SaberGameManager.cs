using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SaberGameManager : MonoBehaviour
{
    public static SaberGameManager Instance;

    [Header("Game State")]
    public bool gameRunning;
    public int score;
    public int winScore = 20;
    public bool gameLocked;
    [Header("UI")]
    public TMP_Text scoreText;
    public GameObject winPanel;
    public GameObject wrongX;

    [Header("Red Screen Overlay")]
    public Image redOverlay;          
    [Range(0f, 1f)] public float startRedAlpha = 0.6f;  

    [Header("Audio")]
    public AudioSource sfx;
    public AudioClip hitCorrect;
    public AudioClip hitWrong;
    public AudioClip winClip;

    void Awake()
    {
        Instance = this;
        SetRunning(false);
        RefreshUI();
        if (winPanel) winPanel.SetActive(false);
        if (wrongX) wrongX.SetActive(false);
        UpdateRedOverlay();
    }

    public void SetRunning(bool running)
    {
        gameRunning = running;
        // UpdateRedOverlay();
    }
    public void RestartGame()
    {
        score = 0;
        RefreshUI();

        gameLocked = false;

        if (winPanel) winPanel.SetActive(false);

        NoteHitState[] notes = FindObjectsByType<NoteHitState>(
            FindObjectsSortMode.None
        );

        foreach (var n in notes)
        {
            Destroy(n.gameObject);
        }

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

        float t = (winScore <= 0) ? 1f : Mathf.Clamp01((float)score / winScore);
        float alpha = Mathf.Lerp(startRedAlpha, 0f, t);

        Color c = redOverlay.color;
        c.a = alpha;
        redOverlay.color = c;
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

        if (sfx && winClip) sfx.PlayOneShot(winClip);
        if (winPanel) winPanel.SetActive(true);

        UpdateRedOverlay();
    }

    void RefreshUI()
    {
        if (scoreText) scoreText.text = $"Score: {score}";
    }

    public void ResetGame()
    {
        score = 0;
        RefreshUI();
        if (winPanel) winPanel.SetActive(false);
        SetRunning(false);
        UpdateRedOverlay();
    }
}