using System;
using Supabase.Gotrue;
using UnityEngine;

public static class SupabaseEvents 
{
    public static Action<Session> OnLoginSuccess;
    public static Action<Session> OnRegistrationSuccess;
    public static Action<string> OnLoginFail;
    public static Action<string> OnRegistrationFail;
    public static Action OnLogout;
    public static Action OnProfileUpdate;
}
