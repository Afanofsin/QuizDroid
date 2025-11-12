using UnityEngine;
using System;

public static class ShopEvents
{
    public static Action ShopInitialized;
    public static Action<IIAPItem> BuyInitialized;
    public static Action ItemBought;
    public static Action FailedToBuy;

}
