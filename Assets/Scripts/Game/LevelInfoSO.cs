using UnityEngine;

[CreateAssetMenu(fileName = "LevelInfoSO", menuName = "Game/Level")]
public class LevelInfoSO : ScriptableObject
{
    public int levelSeed;
    
    [Header("Konfiguracja Czasu i Punktów")]
    public float levelDuration = 120f;
    public int pointsRequiredFor1Star = 100;
    public int pointsRequiredFor2Stars = 250;
    public int pointsRequiredFor3Stars = 500;

    [Header("Konfiguracja Zamówień")]
    public OrderSO[] availableOrdersInLevel;
    public int initialOrdersCount = 2;
    public float orderDecayTime = 30f;
    public float timeBetweenOrders = 10f;
    public float orderCombo = 0.1f;
    public float maxMultiplier = 2.0f;

    public string levelName;
    public GameObject prefab;
}
