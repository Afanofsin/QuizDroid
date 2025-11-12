using Cysharp.Threading.Tasks;
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
        SupabaseEvents.OnLogout += HandleLogout;
    }

    private void UnsubscribeFromEvents()
    {
        SupabaseEvents.OnLogout -= HandleLogout;
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
        m_EventRegistry.RegisterCallback<ClickEvent>(m_CloseButton, evt => UniTask.Void (async () =>
        {
            await LogOut();
        }));
    }

    private void CloseWindow()
    {
        UIEvents.ScreenClosed?.Invoke();
    }

    private async UniTask LogOut()
    {
        await SupabaseService.Instance.LogOut();
    }

    private void HandleLogout()
    {
        UIEvents.AuthScreenShown?.Invoke();
    }
}
