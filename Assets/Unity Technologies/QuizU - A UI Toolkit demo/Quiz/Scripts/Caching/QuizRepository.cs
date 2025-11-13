using System;
using System.Collections.Generic;
using System.Linq;
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

    public async UniTask<List<QuizPack>> GetQuizPackList(int count)
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

    public async UniTask<List<Question>> GetQuizQuestions(int quizId)
    {
        var questionResponse = await SupabaseService.Instance.client
                        .From<Question>()
                        .Where(q => q.QuizPackId == quizId)
                        .Get();
        var questions = questionResponse.Models;

        List<long> questionIDs = new();

        foreach (var question in questions)
            questionIDs.Add(question.Id);  

        var answersResponse = await SupabaseService.Instance.client
                        .From<Answer>()
                        .Filter("question_id", Postgrest.Constants.Operator.In, questionIDs)
                        .Get();
        
        foreach (var question in questions)
        {
            question.Answers = answersResponse.Models
                            .Where(a => a.QuestionId == question.Id)
                            .ToList();
        }
        return questions;
    }
}
