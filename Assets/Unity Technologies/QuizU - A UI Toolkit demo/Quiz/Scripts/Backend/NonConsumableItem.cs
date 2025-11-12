using UnityEngine;
using UnityEngine.Purchasing;

[System.Serializable]
public class NonConsumableItem : IIAPItem
{
    override public void Purchase(StoreController storeController)
    {
        Debug.Log($"{Description}");
        storeController.PurchaseProduct(Id);
    }

    public override void Reward()
    {
        Debug.Log($"Yay!");
    }
}
