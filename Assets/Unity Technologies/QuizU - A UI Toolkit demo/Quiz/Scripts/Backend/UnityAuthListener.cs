using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;
using UnityEngine;

public class UnityAuthListener : MonoBehaviour
{
    public void SessionListener(IGotrueClient<User, Session> sender, Constants.AuthState newState)
    {
		if (sender.CurrentUser?.Email == null)
			Debug.Log($"No user logged in");
		else
		{
			Debug.Log($"Logged in as {sender.CurrentUser.Email}");
		}

		switch (newState)
		{
			case Constants.AuthState.SignedIn:
				SupabaseEvents.OnLoginSuccess?.Invoke(sender.CurrentSession);
				break;
			case Constants.AuthState.SignedOut:
				SupabaseEvents.OnLogout?.Invoke();
				Debug.Log("Signed Out");
				break;
			case Constants.AuthState.UserUpdated:
				Debug.Log("Signed In");
				break;
			case Constants.AuthState.PasswordRecovery:
				Debug.Log("Password Recovery");
				break;
			case Constants.AuthState.TokenRefreshed:
				Debug.Log("Token Refreshed");
				break;
			case Constants.AuthState.Shutdown:
				Debug.Log("Shutdown");
				break;
			default:
				Debug.LogError("Unknown Auth State Update");
				break;
			}
    }
}
