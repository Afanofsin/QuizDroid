using Quiz;
using UnityEngine;
using UnityEngine.UIElements;

public class ProfileScreen : UIScreen
{
    VisualElement m_BackButton;
    VisualElement m_CloseButton;
    public ProfileScreen(VisualElement parentElement) : base(parentElement)
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
        m_CloseButton = m_RootElement.Q<Button>("profile__close-button");
    }

    private void RegisterCallbacks()
    {
        m_EventRegistry.RegisterCallback<ClickEvent>(m_BackButton, evt => CloseWindow());
        m_EventRegistry.RegisterCallback<ClickEvent>(m_CloseButton, evt => CloseWindow());
    }

    private void CloseWindow()
    {
        UIEvents.ScreenClosed?.Invoke();
    }
}
