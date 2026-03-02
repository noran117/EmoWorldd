using UnityEngine;
using BNG;

public class PuzzleManager : MonoBehaviour
{
    [Header("Sound")]
    public AudioSource snapSound;

    public SnapZone[] sockets;
    public Light localRoomLight;

    public GameObject celebrationEffect;
    public Transform celebrationPoint;

    int filledCount = 0;
    bool finished = false;

    bool[] socketFilled;

    void Start()
    {
        if (celebrationEffect != null)
            celebrationEffect.SetActive(false);

        if (sockets == null) sockets = new SnapZone[0];
        socketFilled = new bool[sockets.Length];

        for (int i = 0; i < sockets.Length; i++)
        {
            var socket = sockets[i];
            if (socket == null) continue;

            int index = i; 
            socket.OnSnapEvent.AddListener((Grabbable g) => OnPiecePlaced(index, g));
        }
    }

    public void EnablePuzzle()
    {
        filledCount = 0;
        finished = false;

        if (celebrationEffect != null)
            celebrationEffect.SetActive(false);

        if (socketFilled != null)
        {
            for (int i = 0; i < socketFilled.Length; i++)
                socketFilled[i] = false;
        }
    }

    void OnPiecePlaced(int socketIndex, Grabbable g)
    {
        snapSound?.Play();
        if (GameStateManager.Instance.currentState != GameState.Bargaining) return;
        if (finished) return;

        if (socketFilled != null && socketIndex >= 0 && socketIndex < socketFilled.Length)
        {
            if (socketFilled[socketIndex]) return;
            socketFilled[socketIndex] = true;
        }

        filledCount++;

        float t = (sockets != null && sockets.Length > 0) ? (float)filledCount / sockets.Length : 1f;

        if (localRoomLight != null)
        {
            localRoomLight.color = Color.Lerp(Color.red, new Color(0.5f, 0.5f, 0.7f), t);
            localRoomLight.intensity = Mathf.Lerp(1.8f, 0.8f, t);
        }

        if (sockets != null && filledCount >= sockets.Length)
        {
            finished = true;

            if (celebrationEffect != null)
            {
                if (celebrationPoint != null)
                {
                    celebrationEffect.transform.SetParent(celebrationPoint);
                    celebrationEffect.transform.localPosition = Vector3.zero;
                    celebrationEffect.transform.localRotation = Quaternion.identity;
                }

                celebrationEffect.SetActive(true);
            }

            BargainingManager.Instance.OnPuzzleFinished();
        }
    }
}