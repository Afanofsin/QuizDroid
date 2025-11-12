using UnityEngine;
using UnityEngine.Purchasing;

public class ConsumableItem : IIAPItem
{
    override public void Purchase(StoreController storeController)
    {
        Debug.Log($"{Description}");
        storeController.PurchaseProduct(Id);
    }

    public override void Reward()
    {
        Debug.Log($"Yay!");

        // TODO: Server Request update profile
    }

}
