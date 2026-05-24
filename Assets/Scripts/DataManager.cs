using UnityEngine;

class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public float itemMinFlightTime = 0.2f;
    public float playerDetectionRadius = 0.8f;
    public float playerThrowForce = 5.0f;
    public float playerThrowTorque = 1.5f;
    public float playerMoveSpeed = 5f;
    public float playerMovePushForce = 1f;
    public float playerMovementSmoothing = 10f;
    public Color highlightColor = new(0.4f, 0.4f, 0.1f);
    public float stationCuttingTime = 3.0f;
    public float stationTrashShrinkingTime = 0.5f;

    void Awake() { Instance = this; }
}
