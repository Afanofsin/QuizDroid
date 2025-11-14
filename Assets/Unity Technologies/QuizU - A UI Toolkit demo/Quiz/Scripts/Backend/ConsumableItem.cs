using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;

public class ConsumableItem : IIAPItem
{
    [SerializeField] int amount = 100; 
    override public void Purchase(StoreController storeController)
    {
        Debug.Log($"{Description}");
        storeController.PurchaseProduct(Id);
    }

    public override void Reward()
    {
        GiveReward().Forget();
    }

    public async UniTaskVoid GiveReward()
    {
        Profile profile = await UserDataRepository.Instance.LoadUserProfile();
        profile.Currency += amount;
        await UserDataRepository.Instance.UpdateUserProfile(profile);
    }
}
