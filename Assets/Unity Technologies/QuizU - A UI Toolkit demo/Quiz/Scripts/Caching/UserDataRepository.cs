using Cysharp.Threading.Tasks;
using Supabase.Gotrue;
using UnityEngine;

public class UserDataRepository : MonoBehaviour
{
    public static UserDataRepository Instance { get; private set; }
    public void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    public async UniTask CreateUser(User user)
    {
        Profile profile = new Profile
        {
            Id = user.Id,
            Username = $"username{user.Id.Substring(0, 5)}",
            Xp = 0,
            Currency = 0,
            IsPremium = false
        };

        await SupabaseService.Instance.client
                        .From<Profile>()
                        .Insert(profile);

        LocalCachingManager.Instance.CreateLocalProfile(profile);
    }

    public async UniTask<Profile> LoadUserProfile()
    {
        var profile = LocalCachingManager.Instance.GetProfile();
        if (profile == null)
        {
            profile = await SupabaseService.Instance.GetProfile();
            LocalCachingManager.Instance.CreateLocalProfile(profile);
            return profile;
        }
        return profile;
    }

    public async UniTask UpdateUserProfile(Profile profile)
    {
        var update = await SupabaseService.Instance.UpdateProfile(profile);
        if (update == false)
            return;
        else
        {
            LocalCachingManager.Instance.UpdateProfile(profile);
        }
    }
}
