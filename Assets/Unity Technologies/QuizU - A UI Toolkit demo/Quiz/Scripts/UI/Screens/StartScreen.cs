using Cysharp.Threading.Tasks;
using Supabase.Gotrue;
using UnityEngine.UIElements;

namespace Quiz
{
    /// <summary>
    /// This fullscreen UI hides the main menu until the user is ready to begin
    /// </summary>

    public class StartScreen : UIScreen
    {
        Button m_StartButton;
        Button m_SignUpButton;
        Button m_SignInButton;
        TextField m_EmailText;
        TextField m_PasswordText;
        Label m_ErrorLabel;


        // Constructor 
        public StartScreen(VisualElement parentElement) : base(parentElement)
        {
            m_RootElement = parentElement;
            SetVisualElements();
            RegisterCallbacks();
            SubscribeToEvents();
        }

        public override void Disable()
        {
            base.Disable();
            UnsubscribeFromEvents();
        }

        public void SetVisualElements()
        {
            m_StartButton = m_RootElement.Q<Button>("start__start-button");
            m_SignUpButton = m_RootElement.Q<Button>("start__sign-up-button");
            m_SignInButton = m_RootElement.Q<Button>("start__sign-in-button");
            m_EmailText = m_RootElement.Q<TextField>("start__email");
            m_PasswordText = m_RootElement.Q<TextField>("start__password");
            m_ErrorLabel = m_RootElement.Q<Label>("start__error-label");
        }

        public void RegisterCallbacks()
        {
            // The custom Event Registry unregisters the callback automatically on disable
            m_EventRegistry.RegisterCallback<ClickEvent>(m_StartButton, evt => UIEvents.MainMenuShown?.Invoke());
            m_EventRegistry.RegisterCallback<ClickEvent>(m_SignUpButton, evt => UniTask.Void (async () =>
            {
                await SendRegistrationRequest();
            }));
            m_EventRegistry.RegisterCallback<ClickEvent>(m_SignInButton, evt => UniTask.Void (async () =>
            {
                await SendLoginRequest();
            }));
        }

        public void SubscribeToEvents()
        {
            SupabaseEvents.OnLoginFail += FailureToAuth;
            SupabaseEvents.OnRegistrationFail += FailureToAuth;
            SupabaseEvents.OnLoginSuccess += SuccesfullAuth;
            SupabaseEvents.OnRegistrationSuccess += SuccesfullAuth;
        }

        public void UnsubscribeFromEvents()
        {
            SupabaseEvents.OnLoginFail -= FailureToAuth;
            SupabaseEvents.OnRegistrationFail -= FailureToAuth;
            SupabaseEvents.OnLoginSuccess -= SuccesfullAuth;
            SupabaseEvents.OnRegistrationSuccess -= SuccesfullAuth;
        }

        public async UniTask SendRegistrationRequest()
        {
            m_ErrorLabel.style.display = DisplayStyle.None;
            await SupabaseService.Instance.RegisterUser(m_EmailText.value, m_PasswordText.value);
        }

        public async UniTask SendLoginRequest()
        {
            m_ErrorLabel.style.display = DisplayStyle.None;
            await SupabaseService.Instance.LoginUser(m_EmailText.value, m_PasswordText.value);
        }

        public void FailureToAuth(string message)
        {
            m_ErrorLabel.style.display = DisplayStyle.Flex;
            m_ErrorLabel.text = message;
        }
        
        public void SuccesfullAuth(Session session)
        {
            // TODO: Save user credentials
            UIEvents.MainMenuShown?.Invoke();
            UniTask.Void(async () =>
            {
                await SupabaseService.Instance.GetProfile();
            });
        }
    }
}
