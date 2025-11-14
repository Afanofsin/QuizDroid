using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Postgrest.Responses;
using Supabase.Gotrue;
using Supabase.Gotrue.Exceptions;
using Client = Supabase.Client;
using Supabase;
using JetBrains.Annotations;
using System.Linq;

public class SupabaseService : MonoBehaviour
{
    public static SupabaseService Instance { get; private set; }
    public UnityAuthListener Listener = null!;
    public const string SUPABASE_URL = "https://ogyyytbeikgroamiuvvj.supabase.co";
    public const string PUBLIC_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Im9neXl5dGJlaWtncm9hbWl1dnZqIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjAyOTg5MDEsImV4cCI6MjA3NTg3NDkwMX0.8O68a24RomFxX_OSlTqa2UCQPWkQYmMTgIYFRWsL0Yw";
    public Client client { get; private set; }
    private readonly NetworkStatus _networkStatus = new ();

    void Awake()
    {
        // Implement Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }
    public async void Start()
    {
        if (client == null)
        {
            SupabaseOptions options = new();
			options.AutoRefreshToken = true;

            client = new Client(SUPABASE_URL, PUBLIC_KEY, options);

            _networkStatus.Client = (Supabase.Gotrue.Client)client.Auth;

            client.Auth.SetPersistence(new UnitySession());
            client.Auth.AddStateChangedListener(Listener.SessionListener);

            await client.InitializeAsync();
        }
    }

    public async UniTask RegisterUser(string email, string password)
    {
        Debug.Log($"Registration start");
        
        try
        {
            Session signUp = await client.Auth.SignUp(email, password).AsUniTask();

            if (signUp == null || signUp.User == null)
            {
                SupabaseEvents.OnRegistrationFail?.Invoke("Registration Failed.");
                return;
            }
            
            await UserDataRepository.Instance.CreateUser(signUp.User);
            Debug.Log($"Registration success. User ID: {signUp.User.Id}");
        }
        catch (Exception exception)
        {
            Debug.LogError($"Registration Error: {exception.Message}");
            SupabaseEvents.OnRegistrationFail?.Invoke(exception.Message);
            return;
        }
    }

    public async UniTask LoginUser(string email, string password)
    {
        Debug.Log($"Login start");

        try
        {
            Session signIn = await client.Auth.SignInWithPassword(email, password).AsUniTask();

            if (signIn == null || signIn.User == null)
            {
                SupabaseEvents.OnLoginFail?.Invoke("Login Failed.");
                return;
            }

            await UserDataRepository.Instance.LoadUserProfile();
            Debug.Log($"Login success. User ID: {signIn.User.Id}");
        }
        catch (Exception exception)
        {
            Debug.LogError($"Registration Error: {exception.Message}");
            SupabaseEvents.OnLoginFail?.Invoke(exception.Message);
            return;
        }
    }

    public async UniTask<Profile> GetProfile()
    {
        var user = client.Auth.CurrentUser;
        Debug.Log($"{user.Id}");
        var result = await client.From<Profile>()
                    .Where(p => p.Id == user.Id)
                    .Get();
        Profile profile = result.Models.FirstOrDefault();
        return profile;
    }
    
    public async UniTask<bool> UpdateProfile(Profile profile)
    {
        try
        {
            await client.From<Profile>()
                .Update(profile);
            return true;
        }
        catch(Exception e)
        {
            Debug.LogError($"Failed to update profile: {e.Message}");
            return false;
        }
    }

    public async UniTask LogOut()
    {
        await client.Auth.SignOut().AsUniTask();
        Debug.Log($"User Signed Out");
    }

    private void OnApplicationQuit()
    {       
	    client?.Auth.Shutdown();
	    client = null;
    }
}
