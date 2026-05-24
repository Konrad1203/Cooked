using UnityEngine;
using System.Collections.Generic;

public class CustomerLineManager : MonoBehaviour
{
    public List<Transform> slots;
    private List<CustomerAI> customersInLine = new();
    public CustomerAI customerPrefab;
    public Transform spawnPoint;
    public Transform exitPoint;

    private void OnEnable()
    {
        GameManager.OnOrderCreated += OnOrderCreated;
        GameManager.OnOrderServe += OnOrderServe;
        GameManager.OnOrderExpired += OnOrderExpired;
    }

    private void OnDisable()
    {
        GameManager.OnOrderCreated -= OnOrderCreated;
        GameManager.OnOrderServe -= OnOrderServe;
        GameManager.OnOrderExpired -= OnOrderExpired;
    }

    public void OnOrderCreated()
    {
        if (customersInLine.Count < slots.Count)
        {
            CustomerAI newCustomer = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity, transform);
            customersInLine.Add(newCustomer);
            var idx = customersInLine.Count - 1;
            newCustomer.SetTarget(slots[idx], idx - 1 >= 0 ? slots[idx-1] : transform);
        }
    }

    public void OnOrderServe(Item plate)
    {
        if (customersInLine.Count > 0)
        {
            RemoveAndMoveQueue(0, plate);
        }
    }

    public void OnOrderExpired()
    {
        if (customersInLine.Count > 0)
        {
            RemoveAndMoveQueue(0); 
        }
    }

    private void RemoveAndMoveQueue(int index, Item plate = null)
    {
        var customer = customersInLine[index];
        customersInLine.RemoveAt(index);
        customer.ServePlateSetTargetAndDestroy(plate, exitPoint);
        UpdateCustomersPositions();
    }

    private void UpdateCustomersPositions()
    {
        for (int i = 0; i < customersInLine.Count; i++)
            customersInLine[i].SetTarget(slots[i], i - 1 >= 0 ? slots[i-1] : transform);
    }
}
