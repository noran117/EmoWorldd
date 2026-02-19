using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    [SerializeField] int totalCoins = 8;
    private int collectedCoins = 0;

    public ChestController chest;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void CollectCoin()
    {
        collectedCoins++;
        Debug.Log("Coin collected! Total: " + collectedCoins);

        if (collectedCoins >= totalCoins)
        {
            chest.OpenChest();
        }
    }
}
