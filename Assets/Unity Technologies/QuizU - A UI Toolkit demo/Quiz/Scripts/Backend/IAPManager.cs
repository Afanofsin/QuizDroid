using UnityEngine;
using UnityEngine.Purchasing;
using Cysharp.Threading.Tasks;
using System;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using System.Collections.Generic;
using SerializeReferenceEditor;
using UnityEditor.Media;
using System.Linq;

public class IAPManager : MonoBehaviour
{
    public static IAPManager Instance { get; private set; }
    public static bool IsInitialized { get; private set; } = false;
    private static StoreController storeController;

    [field: SerializeReference, SR] private List<IIAPItem> shopitems;
    public List<IIAPItem> AvailableItems => shopitems;

    // TODO: Add Ui reference and interactions

    async void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        UnityIAPServices.Store("fake");
        await InitIAP();
    }

    private void OnDestroy()
    {
        UnSubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        ShopEvents.BuyInitialized += BuyProduct;
    }
    
    private void UnSubscribeToEvents()
    {
        ShopEvents.BuyInitialized -= BuyProduct;
    }

    private async UniTask InitIAP()
    {
        try
        {
            var env = new InitializationOptions().SetEnvironmentName("Production");
            storeController = UnityIAPServices.StoreController();

            storeController.OnStoreDisconnected += OnStoreDisconnected;
            storeController.OnProductsFetched += OnProductsFetched;
            storeController.OnProductsFetchFailed += OnProductsFetchedFailed;
            storeController.OnPurchasesFetched += OnPurchasesFetched;
            storeController.OnPurchasesFetchFailed += OnPurchasesFetchFailed;
            storeController.OnPurchasePending += OnPurchasePending;
            storeController.OnPurchaseConfirmed += OnPurchaseConfirmed;
            storeController.OnPurchaseFailed += OnPurchaseFailed;
            storeController.OnPurchaseDeferred += OnPurchaseDeferred;

            await storeController.Connect();

            var initialFetch = BuildProductDefinition();

            storeController.FetchProducts(initialFetch);
            SubscribeToEvents();
        }
        catch (Exception e)
        {
            Debug.Log($"Initialization failed, reason: {e.StackTrace}");
        }
    }

    private List<ProductDefinition> BuildProductDefinition()
    {
        var initialFetch = new List<ProductDefinition>();

        initialFetch?.Add(new ProductDefinition(AvailableItems[0].Id, ProductType.Consumable));
        initialFetch?.Add(new ProductDefinition(AvailableItems[1].Id, ProductType.NonConsumable));
        initialFetch?.Add(new ProductDefinition(AvailableItems[2].Id, ProductType.Subscription));

        return initialFetch;
    }

    private void OnProductsFetched(List<Product> products)
    {
        // Products are loaded, fetching purchases now.
        storeController.FetchPurchases();

        foreach (var product in products)
        {
            string price = product.metadata.localizedPrice + " " + product.metadata.isoCurrencyCode;
            // TODO: UPDATE UI BUTTONS WITH NEW PRICES;
        }
    }

    private void OnProductsFetchedFailed(ProductFetchFailed error)
    {
        Debug.Log($"Product fetch failed: {error}");
    }

    private void OnPurchasesFetched(Orders orders)
    {
        IsInitialized = true;
    }

    private void OnPurchasesFetchFailed(PurchasesFetchFailureDescription error)
    {
        Debug.Log($"Purchases fetch failed: {error}");
    }

    private void OnStoreDisconnected(StoreConnectionFailureDescription error)
    {
        Debug.Log($"Init/Connect failed: {error}");
    }

    private void OnPurchasePending(PendingOrder order)
    {
        Debug.Log($"Pending order: {order}");
        storeController.ConfirmPurchase(order);
    }

    private void OnPurchaseConfirmed(Order order)
    {
        Debug.Log($"Purchase confirmed: {order}");
        // TODO: Reward 
        var item = shopitems.FirstOrDefault(i => i.Id == order.Info.PurchasedProductInfo[0].productId);
        if (item != null)
        {
            item.Reward();
        }
        else
        {
            Debug.Log($"No matching item for {order} found");
        }

    }
    
    private void OnPurchaseFailed(FailedOrder order)
    {
        if (order?.Info?.PurchasedProductInfo == null ||
        order.Info.PurchasedProductInfo.Count == 0)
        {
            Debug.Log($"Purchase failed, no product info is available");
            return;
        }
        var productId = order.Info.PurchasedProductInfo[0].productId;
        var reason = order.FailureReason;
        var message = order.Details;

        Debug.Log($"Purchase failed. Product is {productId}.\n Reason is {reason}.\n Details: {message}");
    }

    private void OnPurchaseDeferred(DeferredOrder order)
    {
        Debug.Log($"Purchase deffered for product: {order?.Info}");
        // TODO: Show UI "Purchase pending..."
    }

    public void BuyProduct(IIAPItem item)
    {
        if (IsInitialized)
        {
            item.Purchase(storeController);
        }
        else
        {
            Debug.Log($"IAP is not yet initialized");
        }
    }
}
