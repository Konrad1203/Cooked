using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Wybierz poziom do załadowania")]
    [SerializeField] private LevelInfoSO currentLevelInfo;
    private System.Random levelRandomGenerator;

    public float TimeRemaining { get; private set; }
    public int CurrentPoints { get; private set; }
    public bool IsGameActive { get; private set; }
    private float nextOrderTime;
    private float orderMultiplier = 1f;

    public static event Action<int> OnPointsChanged;
    public static event Action<float> OnMultiplierChanged;
    public static event Action OnOrderCreated;
    public static event Action<Item> OnOrderServe;
    public static event Action OnOrderExpired;

    private List<OrderUINote> activeOrders = new();

    void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetCurrentLevelAndInstantiate(LevelInfoSO level)
    {
        currentLevelInfo = level;
        CurrentPoints = 0;
        TimeRemaining = level.levelDuration;
        nextOrderTime = TimeRemaining - level.timeBetweenOrders;

        int seed = currentLevelInfo.levelSeed;
        levelRandomGenerator = new System.Random(seed);

        Instantiate(level.prefab, transform, true);
    }

    void Update()
    {
        if (!IsGameActive) return;
        if (TimeRemaining > 0) {
            TimeRemaining -= Time.deltaTime;
            if (TimeRemaining <= nextOrderTime) {
                StartCoroutine(InstantiateOrdersCoroutine(1));
                nextOrderTime -= currentLevelInfo.timeBetweenOrders;
            }
        } else {
            TimeRemaining = 0;
            EndLevel();
        }
    }

    public void RemoveOrderDueToTimeout(OrderUINote orderToRemove)
    {
        if (activeOrders.Contains(orderToRemove))
        {
            activeOrders.Remove(orderToRemove);
            RegisterOrderExpiration();
        }
    }

    public float GetLevelDuration()
    {
        return currentLevelInfo.levelDuration;
    }

    public float GetOrderDecayTime()
    {
        return currentLevelInfo.orderDecayTime;
    }

    public void StartLevel()
    {
        IsGameActive = true;
        StartCoroutine(SpawnInitialOrdersAfterDelay(currentLevelInfo.initialOrdersCount));
    }

    private IEnumerator SpawnInitialOrdersAfterDelay(int count)
    {
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(InstantiateOrdersCoroutine(count));
    }

    private IEnumerator InstantiateOrdersCoroutine(int count)
    {
        UICanvasManager uiManager = UICanvasManager.Instance;
        OrderSO[] pool = currentLevelInfo.availableOrdersInLevel;
        if (pool.Length == 0) yield break;
        for (int i = 0; i < count; i++)
        {
            OrderUINote note = Instantiate(uiManager.orderNotePrefab, uiManager.orderNotes);
            int randomIndex = levelRandomGenerator.Next(0, pool.Length);
            OrderSO randomOrder = pool[randomIndex];
            note.SetOrder(randomOrder);
            activeOrders.Add(note);
            OnOrderCreated?.Invoke();
            yield return new WaitForSeconds(0.5f);
        }
    }

    public bool RegisterService(Container plate)
    {
        bool served;
        OrderUINote orderMatch = FindOrderMatch(plate);
        if (orderMatch != null) {
            bool wasFirstOrder = activeOrders.Count > 0 && activeOrders[0] == orderMatch;
            activeOrders.Remove(orderMatch);
            orderMatch.InitiateDespawn();
            if (wasFirstOrder) {
                CurrentPoints += Mathf.FloorToInt(50 * orderMultiplier);
                orderMultiplier += currentLevelInfo.orderCombo;
                if (orderMultiplier > currentLevelInfo.maxMultiplier) orderMultiplier = currentLevelInfo.maxMultiplier;
            } else {
                CurrentPoints += Mathf.FloorToInt(50);
                orderMultiplier = 1.0f;
            }
            OnPointsChanged?.Invoke(CurrentPoints);
            OnOrderServe?.Invoke(plate);
            served = true;
        } else {
            orderMultiplier = 1.0f;
            served = false;
        }
        OnMultiplierChanged?.Invoke(orderMultiplier);
        return served;
    }

    private OrderUINote FindOrderMatch(Container plate) {
        foreach (OrderUINote activeOrder in activeOrders) {
            if (IsOrderMatch(activeOrder.Order, plate.itemsOnPlate)) {
                return activeOrder;
            }
        }
        return null;
    }

    public bool IsOrderMatch(OrderSO order, List<ItemSO> itemsOnPlate)
    {
        if (order.requiredItems.Length != itemsOnPlate.Count) return false;
        List<ItemSO> neededItemsCopy = new(order.requiredItems);
        foreach (ItemSO plateItem in itemsOnPlate) {
            if (neededItemsCopy.Contains(plateItem)) neededItemsCopy.Remove(plateItem);
            else return false;
        }
        return neededItemsCopy.Count == 0;
    }

    public void RegisterOrderExpiration()
    {
        CurrentPoints -= 30;
        orderMultiplier = 1f;
        OnPointsChanged?.Invoke(CurrentPoints);
        OnMultiplierChanged?.Invoke(orderMultiplier);
        OnOrderExpired?.Invoke();
    }

    private void EndLevel()
    {
        IsGameActive = false;
        Debug.Log("Koniec Poziomu! Zdobyte punkty: " + CurrentPoints);
        LevelEndMenu.Instance.SetupAndActivate(currentLevelInfo, CurrentPoints);
    }

    private void LogList<T>(string result, List<T> list) {
        foreach (var item in list) result += item.ToString() + ", ";
        Debug.Log(result);
    }
}
