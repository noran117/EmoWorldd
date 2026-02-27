using UnityEngine;
using TMPro;

public class PickupObjectUI : MonoBehaviour
{
    public Canvas worldCanvas;
    public TMP_Text instructionText;

    private enum ObjectState
    {
        OnGround,
        PickedUp,
        Thrown
    }

    private ObjectState currentState = ObjectState.OnGround;

    void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        // 1 ? «· ﬁÿÂ«
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentState = ObjectState.OnGround;
            UpdateUI();
        }

        // 2 ? «—„ˆÂ«
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentState = ObjectState.PickedUp;
            UpdateUI();
        }

        // 3 ? —ﬂ¯»Â«
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentState = ObjectState.Thrown;
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        switch (currentState)
        {
            case ObjectState.OnGround:
                instructionText.text = "«· ﬁÿÂ«";
                worldCanvas.enabled = true;
                break;

            case ObjectState.PickedUp:
                instructionText.text = "«—„ˆÂ«";
                worldCanvas.enabled = true;
                break;

            case ObjectState.Thrown:
                instructionText.text = "—ﬂ¯»Â«";
                worldCanvas.enabled = true;
                break;
        }
    }
}
