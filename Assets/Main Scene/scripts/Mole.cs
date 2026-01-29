using System.Collections;
using UnityEngine;

public class Mole : MonoBehaviour
{
    [Header("Graphics")]
    public GameObject standardMoleModel;
    public GameObject hardHatModel;
    public GameObject bombModel;


    [Header("GameManager")]
    [SerializeField] private GameManager gameManager;

    // The offset of the objects to hide it.
    private Vector3 startPosition;
    private Vector3 endPosition;
    private Vector3 endPositionForTheBomb;


    // How long it takes to show a mole.
    private float showDuration = 0.5f;
    private float duration = 1f;

    private BoxCollider boxCollider;
    private Vector3 boxOffset;
    private Vector3 boxSize;
    private Vector3 boxOffsetHidden;
    private Vector3 boxSizeHidden;

    // Mole Parameters 
    private bool hittable = true;
    public enum MoleType { Standard, HardHat, Bomb };
    private MoleType moleType;
    private float hardRate = 0.25f;
    private float bombRate = 0f;
    private int lives;
    private int moleIndex = 0;

    private IEnumerator ShowHide(Vector3 start, Vector3 end)
    {
        //// Make sure we start at the start.
        //transform.localPosition = start;

        // Show the mole.
        float elapsed = 0f;
        while (elapsed < showDuration)
        {
            transform.localPosition = Vector3.Lerp(start, end, elapsed / showDuration);
            boxCollider.center = Vector3.Lerp(boxOffsetHidden, boxOffset, elapsed / showDuration);
            boxCollider.size = Vector3.Lerp(boxSizeHidden, boxSize, elapsed / showDuration);
            // Update at max framerate.
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Make sure we're exactly at the end.
        transform.localPosition = end;
        boxCollider.center = boxOffset;
        boxCollider.size = boxSize;

        // Wait for duration to pass.
        yield return new WaitForSeconds(duration);

        // Hide the mole.
        elapsed = 0f;
        while (elapsed < showDuration)
        {
            transform.localPosition = Vector3.Lerp(end, start, elapsed / showDuration);
            boxCollider.center = Vector3.Lerp(boxOffset, boxOffsetHidden, elapsed / showDuration);
            boxCollider.size = Vector3.Lerp(boxSize, boxSizeHidden, elapsed / showDuration);
            // Update at max framerate.
            elapsed += Time.deltaTime;
            yield return null;
        }
        // Make sure we're exactly back at the start position.
        transform.localPosition = start;
        boxCollider.center = boxOffsetHidden;
        boxCollider.size = boxSizeHidden;

        // If we got to the end and it's still hittable then we missed it.
        if (hittable)
        {
            hittable = false;
            // We only give time penalty if it isn't a bomb.
            gameManager.Missed(moleIndex, moleType != MoleType.Bomb);
        }
    }

    public void Hide()
    {
        // Set the appropriate mole parameters to hide it.
        transform.localPosition = startPosition;
        boxCollider.center = boxOffsetHidden;
        boxCollider.size = boxSizeHidden;
    }

    private IEnumerator QuickHide()
    {
        yield return new WaitForSeconds(0.25f);
        /// Whilst we were waiting we may have spawned again here, so just 
        /// check that hasn't happened before hiding it. This will stop it flickering in that case.
        if (!hittable)
        {
            Hide();
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Hammer") && hittable)
        {
            switch (moleType)
            {
                case MoleType.Standard:
                    gameManager.AddScore(moleIndex);
                    // Stop the animation
                    StopAllCoroutines();
                    StartCoroutine(QuickHide());
                    // Turn off hittable so that we can't keep tapping for score.
                    hittable = false;
                    break;
                case MoleType.HardHat:
                    // If lives == 2 reduce
                    if (lives == 2)
                    {
                        lives--;
                    }
                    else
                    {
                        gameManager.AddScore(moleIndex);
                        // Stop the animation
                        StopAllCoroutines();
                        StartCoroutine(QuickHide());
                        // Turn off hittable so that we can't keep tapping for score.
                        hittable = false;
                    }
                    break;
                case MoleType.Bomb:
                    // Game over, 1 for bomb.
                    gameManager.GameOver(1);
                    break;
                default:
                    break;
            }
        }
    }

    private void CreateNext()
    {
        standardMoleModel.SetActive(false);
        hardHatModel.SetActive(false);
        bombModel.SetActive(false);

        float random = Random.Range(0f, 1f);
        if (random < bombRate)
        {
            // Make a bomb.
            moleType = MoleType.Bomb;
            bombModel.SetActive(true);
        }
        else
        {
            bombModel.SetActive(false);
            random = Random.Range(0f, 1f);
            if (random < hardRate)
            {
                // Create a hard one.
                moleType = MoleType.HardHat;
                hardHatModel.SetActive(true);
                lives = 2;
            }
            else
            {
                // Create a standard one.
                moleType = MoleType.Standard;
                standardMoleModel.SetActive(true);
                lives = 1;
            }
        }
        // Mark as hittable so we can register an onclick event.
        hittable = true;
    }

    // As the level progresses the game gets harder.
    private void SetLevel(int level)
    {
        // As level increases increse the bomb rate to 0.25 at level 10.
        bombRate = Mathf.Min(level * 0.025f, 0.25f);

        // Increase the amounts of HardHats until 100% at level 40.
        hardRate = Mathf.Min(level * 0.025f, 1f);

        // Duration bounds get quicker as we progress. No cap on insanity.
        float durationMin = Mathf.Clamp(1 - level * 0.1f, 0.01f, 1f);
        float durationMax = Mathf.Clamp(2 - level * 0.1f, 0.01f, 2f);
        duration = Random.Range(durationMin, durationMax);
    }

    private void Awake()
    {
        startPosition = startPosition = transform.localPosition;
        endPosition = startPosition + new Vector3(0f, 2f, 0f);
        boxCollider = GetComponent<BoxCollider>();
        // Work out collider values.
        boxOffset = boxCollider.center;
        boxSize = boxCollider.size;
        boxOffsetHidden = new Vector3(boxOffset.x, -2f, boxOffset.z);
        boxSizeHidden = new Vector3(boxSize.x, 0f, boxSize.z);
    }

    public void Activate(int level)
    {
        SetLevel(level);
        CreateNext();
        StartCoroutine(ShowHide(startPosition, endPosition));
    }

    // Used by the game manager to uniquely identify moles. 
    public void SetIndex(int index)
    {
        moleIndex = index;
    }

    // Used to freeze the game on finish.
    public void StopGame()
    {
        hittable = false;
        StopAllCoroutines();
    }
}
