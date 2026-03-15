/*using UnityEngine;
using BNG;

public class grabcheerscript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public CompanionFollow companion;
    private Grabbable grabbable;
    private bool wasGrabbed = false;

    void Start()
    {
        grabbable = GetComponent<Grabbable>();
    }

    void Update()
    {
        if (grabbable.BeingHeld && !wasGrabbed)
        {
            companion.Cheer();

            if (!grabbable.IsGrabbed)
        {
            wasGrabbed = false;
        }
    }













}*/
