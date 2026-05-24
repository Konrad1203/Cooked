using UnityEngine;

public class PlateDispenser : CounterTop
{
    [SerializeField] private Container platePrefab;
    
    void Start()
    {
        if (itemOnCounter == null)
        {
            SpawnPlate();
        }
    }

    private void SpawnPlate()
    {
        Container plate = Instantiate(platePrefab);
        base.PlaceItem(plate);
    }

    protected override bool PlaceItem(Item newItem)
    {
        return false;
    }

    protected override Item TakeItem(Transform playerHoldPoint) {
        var item = base.TakeItem(playerHoldPoint);
        SpawnPlate();
        return item;
    }
}
