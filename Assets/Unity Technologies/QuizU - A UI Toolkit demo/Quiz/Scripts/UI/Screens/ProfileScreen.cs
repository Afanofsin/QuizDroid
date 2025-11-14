using System;
using Cysharp.Threading.Tasks;
using Quiz;
using UnityEngine;
using UnityEngine.UIElements;

public class ProfileScreen : UIScreen
{
    VisualElement m_BackButton;
    VisualElement m_CloseButton;
    Label m_LevelLabel;
    ProgressBar m_XpBar;
    Label m_EmailLabel;
    Label m_UsernameLabel;
    Label m_isPremiumLabel;
    Label m_GoldLabel;

    int baseXP = 100;
    double growthRate = 1.10;
    public ProfileScreen(VisualElement parentElement) : base(parentElement)
    {
        SetVisualElements();
        UpdateProfileData();
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
        SupabaseEvents.OnProfileUpdate += UpdateProfileData;
    }

    private void UnsubscribeFromEvents()
    {
        SupabaseEvents.OnLogout -= HandleLogout;
        SupabaseEvents.OnProfileUpdate -= UpdateProfileData;
    }
    // Find and set references to UI elements
    private void SetVisualElements()
    {
        m_BackButton = m_RootElement.Q<Button>("back-button");
        m_CloseButton = m_RootElement.Q<Button>("profile__close-button");

        m_LevelLabel = m_RootElement.Q<Label>("profile-level-label");
        m_XpBar = m_RootElement.Q<ProgressBar>("profile-bar");
        m_EmailLabel = m_RootElement.Q<Label>("profile-email");
        m_UsernameLabel = m_RootElement.Q<Label>("profile-username");
        m_isPremiumLabel = m_RootElement.Q<Label>("profile-premium");
        m_GoldLabel = m_RootElement.Q<Label>("profile-gold");
    }

    private void RegisterCallbacks()
    {
        m_EventRegistry.RegisterCallback<ClickEvent>(m_BackButton, evt => CloseWindow());
        m_EventRegistry.RegisterCallback<ClickEvent>(m_CloseButton, evt => UniTask.Void (async () =>
        {
            await LogOut();
        }));
    }

    private void UpdateProfileData()
    {
        UpdateProfileDataAsync().Forget();
    }

    private async UniTaskVoid UpdateProfileDataAsync()
    {
        await UniTask.WaitUntil(() => UserDataRepository.Instance != null);
        await UniTask.WaitUntil(() => SupabaseService.Instance.client.Auth.CurrentUser != null);
        Profile profile = await UserDataRepository.Instance.LoadUserProfile();
        
        m_EmailLabel.text = SupabaseService.Instance.client.Auth.CurrentUser.Email;
        m_UsernameLabel.text = profile.Username;
        m_GoldLabel.text = profile.Currency.ToString();

        if(profile.IsPremium && m_isPremiumLabel.ClassListContains("premium-inactive"))
        {
            m_isPremiumLabel.ToggleInClassList("premium-active");
            m_isPremiumLabel.ToggleInClassList("premium-inactive");
        }
        else if (!profile.IsPremium && m_isPremiumLabel.ClassListContains("premium-active"))
        {
            m_isPremiumLabel.ToggleInClassList("premium-active");
            m_isPremiumLabel.ToggleInClassList("premium-inactive");
        }

        int currentLevel = CalculateLevel(profile.Xp);
        int xpForCurrentLevel = GetTotalXPForLevel(currentLevel - 1);
        int xpForNextLevel = GetTotalXPForLevel(currentLevel);
        int xpIntoCurrentLevel = profile.Xp - xpForCurrentLevel;
        int xpNeededForNextLevel = xpForNextLevel - xpForCurrentLevel;

        m_XpBar.lowValue = 0;
        m_XpBar.highValue = xpNeededForNextLevel;
        m_XpBar.value = xpIntoCurrentLevel;
        m_LevelLabel.text = $"Level: {currentLevel}";

    }

    private void CloseWindow()
    {
        UIEvents.ScreenClosed?.Invoke();
    }

    private async UniTask LogOut()
    {
        await SupabaseService.Instance.LogOut();
    }

    private int CalculateLevel(int xp)
    {
        int level = 1;
        int totalXp = 0;
        
        while (true)
        {
            int xpNeeded = (int)(baseXP * Math.Pow(growthRate, level));
            totalXp += xpNeeded;
            if (xp < totalXp)
            {
                return level;
            }
            level++;
        }
    }

    private int GetTotalXPForLevel(int targetLevel)
    {
        int totalXP = 0;
    
        for (int level = 1; level <= targetLevel; level++)
        {
            totalXP += (int)(baseXP * Math.Pow(growthRate, level));
        }
    
        return totalXP;
    }

    private void HandleLogout()
    {
        UIEvents.AuthScreenShown?.Invoke();
    }
}
