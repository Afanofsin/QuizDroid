using Cysharp.Threading.Tasks;
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
        GiveReward().Forget();
    }

    public async UniTaskVoid GiveReward()
    {
        Profile profile = await UserDataRepository.Instance.LoadUserProfile();
        profile.IsAdsActive = false;
        await UserDataRepository.Instance.UpdateUserProfile(profile);
    }
}
