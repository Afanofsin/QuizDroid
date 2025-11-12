using Quiz;
using UnityEngine;
using UnityEngine.UIElements;

public class ShopScreen : UIScreen
{
    VisualElement m_BackButton;
    VisualElement m_CloseButton;
    VisualElement m_BuyConsumableButton;
    VisualElement m_BuyNonConsumableButton;
    VisualElement m_BuySubscriptionButton;

    public ShopScreen(VisualElement parentElement) : base(parentElement)
    {
        SetVisualElements();
        RegisterCallbacks();
        SubscribeToEvents();
    }

    public override void Disable()
    {
        base.Disable();
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        
    }

    private void UnsubscribeFromEvents()
    {
        
    }
    // Find and set references to UI elements
    private void SetVisualElements()
    {
        m_BackButton = m_RootElement.Q<Button>("back-button");
        m_CloseButton = m_RootElement.Q<Button>("shop__close-button");

        m_BuyConsumableButton = m_RootElement.Q<Button>("cons-button");
        m_BuyNonConsumableButton = m_RootElement.Q<Button>("noncons-button");
        m_BuySubscriptionButton = m_RootElement.Q<Button>("sub-button");
    }

    private void RegisterCallbacks()
    {
        m_EventRegistry.RegisterCallback<ClickEvent>(m_BackButton, evt => CloseWindow());
        m_EventRegistry.RegisterCallback<ClickEvent>(m_CloseButton, evt => CloseWindow());

        m_EventRegistry.RegisterCallback<ClickEvent>(m_BuyConsumableButton, evt => InitiateBuy(IAPManager.Instance.AvailableItems[0]));
        m_EventRegistry.RegisterCallback<ClickEvent>(m_BuyNonConsumableButton, evt => InitiateBuy(IAPManager.Instance.AvailableItems[1]));
        m_EventRegistry.RegisterCallback<ClickEvent>(m_BuySubscriptionButton, evt => InitiateBuy(IAPManager.Instance.AvailableItems[2]));
    }

    private void InitiateBuy(IIAPItem item)
    {
        ShopEvents.BuyInitialized?.Invoke(item);
    }
        
    private void CloseWindow()
    {
        UIEvents.ScreenClosed?.Invoke();
    }

}
