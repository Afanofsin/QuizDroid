using UnityEngine;
using UnityEngine.Purchasing;

[System.Serializable]
public class SubscriptionItem : IIAPItem
{
    [field: SerializeField] public int Duration { get; private set; }

    override public void Purchase(StoreController storeController)
    {
        Debug.Log($"{Description} : {Duration}");
        storeController.PurchaseProduct(Id);
    }

    public override void Reward()
    {
        Debug.Log($"Yay!");
    }
}
