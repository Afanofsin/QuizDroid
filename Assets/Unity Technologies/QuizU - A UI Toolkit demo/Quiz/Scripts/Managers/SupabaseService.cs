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

public class SupabaseService : MonoBehaviour
{
    public static SupabaseService Instance { get; private set; }
    public UnityAuthListener Listener = null!;
    public const string SUPABASE_URL = "https://ogyyytbeikgroamiuvvj.supabase.co";
    public const string PUBLIC_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Im9neXl5dGJlaWtncm9hbWl1dnZqIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjAyOTg5MDEsImV4cCI6MjA3NTg3NDkwMX0.8O68a24RomFxX_OSlTqa2UCQPWkQYmMTgIYFRWsL0Yw";
    private Client _supabase;
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
        if (_supabase == null)
        {
            SupabaseOptions options = new();
			options.AutoRefreshToken = true;

            _supabase = new Client(SUPABASE_URL, PUBLIC_KEY, options);

            _networkStatus.Client = (Supabase.Gotrue.Client)_supabase.Auth;

            _supabase.Auth.SetPersistence(new UnitySession());

            await _supabase.InitializeAsync();
        }
    }

    public async UniTask RegisterUser(string email, string password)
    {
        Debug.Log($"Registration start");
        
        try
        {
            Session signUp = await _supabase.Auth.SignUp(email, password).AsUniTask();

            if (signUp == null || signUp.User == null)
            {
                SupabaseEvents.OnRegistrationFail?.Invoke("Registration Failed.");
                return;
            }

            Debug.Log($"Registration success. User ID: {signUp.User.Id}");
            SupabaseEvents.OnRegistrationSuccess?.Invoke(signUp);
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
            Session signIn = await _supabase.Auth.SignInWithPassword(email, password).AsUniTask();

            if (signIn == null || signIn.User == null)
            {
                SupabaseEvents.OnLoginFail?.Invoke("Login Failed.");
                return;
            }

            Debug.Log($"Login success. User ID: {signIn.User.Id}");
            SupabaseEvents.OnLoginSuccess?.Invoke(signIn);
        }
        catch (Exception exception)
        {
            Debug.LogError($"Registration Error: {exception.Message}");
            SupabaseEvents.OnLoginFail?.Invoke(exception.Message);
            return;
        }
    }
    

    private void OnApplicationQuit()
    {       
	    _supabase?.Auth.Shutdown();
	    _supabase = null;
    }
}
