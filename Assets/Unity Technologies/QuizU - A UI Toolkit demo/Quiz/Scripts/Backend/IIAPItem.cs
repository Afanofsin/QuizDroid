using UnityEngine;
using UnityEngine.Purchasing;

[System.Serializable]
public abstract class IIAPItem
{
    [field : SerializeField] public string Id { get; private set; }
    [field : SerializeField] public string Description { get; private set; }
    [field : SerializeField] public string Price { get; private set; }
    [field : SerializeField] public string Name { get; private set; }

    virtual public void Purchase(StoreController storeController)
    {
    }

    virtual public void Reward()
    {
    }

    virtual public void SetPrice(string price)
    {
    }
}
