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
    public GameObject gameUIRoot;   // الأب تبع UI كله
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

    [Header("Spawner")]
    public Spawner spawner;
   
    void Awake()
    {
        Instance = this;

        if (spawner == null)
            spawner = FindFirstObjectByType<Spawner>();

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
        if (gameLocked && running)
            return;

        gameRunning = running;
        Debug.Log("SetRunning called | gameRunning = " + gameRunning);

        if (spawner != null)
        {
            if (running) spawner.StartSpawning();
            else spawner.StopSpawning();
        }

        if (running)
        {
            ShowGameplayUI();

            if (backgroundMusic) backgroundMusic.Stop();
            if (gameMusic && !gameMusic.isPlaying) gameMusic.Play();
        }
        else
        {
            HideGameplayUI();

            if (gameMusic) gameMusic.Stop();
            if (backgroundMusic && !backgroundMusic.isPlaying) backgroundMusic.Play();
        }
    }

    void ShowGameplayUI()
    {
        if (gameUIRoot) gameUIRoot.SetActive(true);

        if (scoreText)
        {
            scoreText.gameObject.SetActive(true);
            scoreText.alpha = 1f;
            scoreText.text = $"Score: {score}";
        }

        if (redOverlay)
        {
            redOverlay.SetActive(true);

            Image img = redOverlay.GetComponent<Image>();
            if (img)
            {
                Color c = img.color;
                c.a = startRedAlpha;
                img.color = c;
            }
        }
    }

    void HideGameplayUI()
    {
        if (gameUIRoot) gameUIRoot.SetActive(false);
    }

    public void RestartGame()
    {
        score = 0;
        gameLocked = false;

        RefreshUI();

        if (winPanel) winPanel.SetActive(false);
        if (wrongX) wrongX.SetActive(false);

        if (spawner != null)
            spawner.ClearAllNotes();
        else
        {
            NoteHitState[] notes = FindObjectsByType<NoteHitState>(FindObjectsSortMode.None);
            foreach (var n in notes)
                Destroy(n.gameObject);
        }

        SetRunning(false);

        if (GameStartGate.Instance != null)
            GameStartGate.Instance.Check();
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

        if (sfx && winClip) sfx.PlayOneShot(winClip);
        if (winPanel) winPanel.SetActive(true);
    }

    void RefreshUI()
    {
        if (scoreText)
            scoreText.text = $"Score: {score}";
    }
}