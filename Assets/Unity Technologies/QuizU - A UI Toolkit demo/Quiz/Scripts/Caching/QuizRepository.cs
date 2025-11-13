using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Quiz;
using UnityEngine;

public class QuizRepository : MonoBehaviour
{
    public static QuizRepository Instance { get; private set; }
    public void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    public async UniTask<List<QuizPack>> FetchQuizPackList(int count)
    {
        try
        {
            var response = await SupabaseService.Instance.client.From<QuizPack>()
                        .Limit(count)
                        .Get();
            List<QuizPack> quizdata = new();
            foreach (var model in response.Models)
            {
                quizdata.Add(model);
            }
            return quizdata;
        }
        catch (Exception e)
        {
             Debug.LogError($"FetchQuizPackList Exception: {e.Message}\n{e.StackTrace}");
        }
        return null;
    }
}
