using UnityEngine;

public class MachineButton : MonoBehaviour
{
    public SugarProcessor sugarProcessor;
    public void PressButton()
    {
        sugarProcessor.StartMachine();
    }
}
