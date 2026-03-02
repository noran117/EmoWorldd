using UnityEngine;

public class BrotherController : MonoBehaviour
{
    public static BrotherController Instance;
    public Animator animator;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void StartDance()
    {
        animator.SetTrigger("Dance");
    }

        public void OnDanceFinished()
    {
        BasketManager.Instance.ShowBasket();
    }
}
