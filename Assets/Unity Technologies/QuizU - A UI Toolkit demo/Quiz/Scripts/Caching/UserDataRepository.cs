using Cysharp.Threading.Tasks;
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

    public async UniTask<Profile> LoadUserProfile()
    {
        var profile = LocalCachingManager.Instance.GetProfile();
        if (profile == null)
        {
            profile = await SupabaseService.Instance.GetProfile();
            return profile;
        }
        return profile;
    }

    public async void UpdateUserProfile(Profile profile)
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
