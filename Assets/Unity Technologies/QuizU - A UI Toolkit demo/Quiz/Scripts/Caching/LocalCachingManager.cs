using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LocalCachingManager : MonoBehaviour
{
    public static LocalCachingManager Instance { get; private set; }
    private Profile _cachedprofile;
    private const string PROFILE_FILE = "user_profile";
    private Dictionary<int, QuizPack> _cachedQuizzes = new();

    public void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    // Get cached profile
    public Profile GetProfile()
    {
        if (_cachedprofile != null)
        {
            return _cachedprofile;
        }

        _cachedprofile = JsonStorage.Load<Profile>(PROFILE_FILE);

        return _cachedprofile;
    }

    // Update profile data
    public void UpdateProfile(Profile profile)
    {
        _cachedprofile = profile;
        JsonStorage.Save<Profile>(PROFILE_FILE, profile);
    }

    public async UniTask<QuizPack> GetQuizPack(int quizId)
    {
        if (_cachedQuizzes.TryGetValue(quizId, out QuizPack cached))
        {
            return cached;
        }

        QuizPack quiz = JsonStorage.Load<QuizPack>($"quiz_{quizId}");
        if (quiz != null)
        {
            _cachedQuizzes[quizId] = quiz;
            return quiz;
        }
        else
        {
            // TODO: ADD SUPABASE FUNCTIONALITY TO HANDLE QUIZ FETCH
            return quiz;
        }
    }

    public void ClearCache()
    {
        _cachedprofile = null;
        _cachedQuizzes.Clear();

        DeleteFile(PROFILE_FILE);
    }

    private void DeleteFile(string filename)
    {
        try
        {
            string path = Path.Combine(Application.persistentDataPath, filename + ".json");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (Exception e)
        {
            Debug.Log($"Error deleting a file: {e.Message}");
        }
    }

    void OnApplicationQuit()
    {
        ClearCache();       
    }
}
