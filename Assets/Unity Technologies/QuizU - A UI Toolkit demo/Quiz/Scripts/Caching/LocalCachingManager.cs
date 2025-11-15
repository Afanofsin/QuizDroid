using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Quiz;
using UnityEngine;

public class LocalCachingManager : MonoBehaviour
{
    public static LocalCachingManager Instance { get; private set; }
    private Profile _cachedprofile;
    private const string PROFILE_FILE = "user_profile";
    private Dictionary<int, QuizPack> _cachedQuizzes = new();
    private Dictionary<int, QuizSO> _cachedQuizzesSO = new();

    public void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    public void CreateLocalProfile(Profile profile)
    {
        JsonStorage.Save<Profile>(PROFILE_FILE, profile);
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

    public QuizSO GetQuizPackSO(int quizId)
    {
        if (_cachedQuizzesSO.TryGetValue(quizId, out QuizSO cached))
        {
            return cached;
        }

        QuizSO quiz = JsonStorage.Load<QuizSO>($"quiz_{quizId}_so");
        if (quiz != null)
        {
            _cachedQuizzesSO[quizId] = quiz;
            return quiz;
        }
        return null;
    }

    public List<QuizPack> GetQuizPackList()
    {
        List<QuizPack> packs = _cachedQuizzes.Values.ToList();
        if (packs.Count != 0)
        {
            return packs;
        }
        else
        {
            return null;
        }

    }

    public void CacheQuizPack(int quizId, QuizPack quiz)
    {
        _cachedQuizzes[quizId] = quiz;
    }

    public void CacheQuizPackSO(int quizId, QuizSO quiz)
    {
        _cachedQuizzesSO[quizId] = quiz;
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
