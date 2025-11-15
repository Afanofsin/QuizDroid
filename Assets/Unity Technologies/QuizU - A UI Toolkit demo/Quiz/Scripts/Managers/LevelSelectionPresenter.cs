
using UnityEngine;
using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.CompilerServices;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;

namespace Quiz
{
    /// <summary>
    /// This is an example of Model-View-Presenter design pattern.
    ///
    /// The LevelSelector is the Presenter, a layer of management that facilitates communication
    /// between the QuizSO ScriptableObject data (Model) and the LevelSelectionScreen user-interface (View).
    /// This separates the UI from the data to improve testing and maintenance.
    /// </summary>
    /// 
    [RequireComponent(typeof(LevelSelectionScreen))]
    public class LevelSelectionPresenter
    {
        [Tooltip("Available quizzes in project; the data (Model)")]
        [SerializeField] QuizSO[] m_Quizzes;

        LevelSelectionScreen m_LevelSelectionScreen;

        public LevelSelectionPresenter(LevelSelectionScreen levelSelectionScreen)
        {
            m_LevelSelectionScreen = levelSelectionScreen ?? throw new ArgumentNullException(nameof(levelSelectionScreen));

            InitializeAsync().Forget();
        }

        // Event unsubscriptions
        public void Disable()
        {
            LevelSelectionEvents.ButtonSelected -= LevelSelectionEvents_ButtonSelected;
        }

        // Event-handling methods

        // Load the Quiz ScriptableObject data to update the LevelSelectionScreen and then highlight
        // the selected button in the NavigationBar
        private void LevelSelectionEvents_ButtonSelected(int selectedIndex)
        {
            if (selectedIndex >= 0 && selectedIndex < m_Quizzes.Length)
            {
                ButtonSelectedAsync(selectedIndex).Forget();
            }
        }

        private async UniTaskVoid ButtonSelectedAsync(int selectedIndex)
        {
            var cache = LocalCachingManager.Instance.GetQuizPackSO(selectedIndex);
            if (cache == null || cache.TotalQuestions == 0)
            {
                List<Question> questions = await QuizRepository.Instance.GetQuizQuestions(m_Quizzes[selectedIndex].Id);
                List<QuestionSO> questionSOs = ConvertQuestionToSO(questions);
                m_Quizzes[selectedIndex].InitializeQuestions(questionSOs);
                LocalCachingManager.Instance.CacheQuizPackSO(selectedIndex, m_Quizzes[selectedIndex]);
                Debug.Log($"I am fetching Quiz from server");
            }

            LevelSelectionEvents.QuizDataLoaded?.Invoke(m_Quizzes[selectedIndex]);
            m_LevelSelectionScreen.NavigationBar.HighlightButton(selectedIndex);
        }

        // Methods

        // Sets up the LevelSelectionScreen and NavigationBar. Assigns actions to each
        // button n the NavigationBar.
        private async UniTaskVoid InitializeAsync()
        {
            await UniTask.WaitUntil(() => LocalCachingManager.Instance != null);

            List<QuizPack> quizzes = LocalCachingManager.Instance.GetQuizPackList();
            Sprite icon = Resources.Load<Sprite>("QuizIcon");
            if (quizzes != null)
            {
                var quizSOs = ConvertQuizToSO(quizzes, icon);
                Initialize(quizSOs);
                LevelSelectionEvents.ButtonSelected += LevelSelectionEvents_ButtonSelected;
            }
            else
            {
                List<QuizPack> packs = await QuizRepository.Instance.GetQuizPackList(5);

                var quizSOs = ConvertQuizToSO(packs, icon);
                Initialize(quizSOs);
                LevelSelectionEvents.ButtonSelected += LevelSelectionEvents_ButtonSelected;
            }

        }
        private void Initialize(List<QuizSO> quizSOs)
        {
            m_Quizzes = quizSOs.ToArray();

            m_LevelSelectionScreen.SetupNavigationBar(m_Quizzes.Length);

            // Assign tooltips from quiz titles
            string[] quizTitles = m_Quizzes.Select(quiz => quiz.Title).ToArray();
            m_LevelSelectionScreen.SetupTooltips(quizTitles);

            RegisterCallbacks();

            OnLoadFirstQuiz();
        }

        private void RegisterCallbacks()
        {
            // An alternate way to register callbacks, uses the Clickable Manipulator's clicked property for convenience.
            // You can also use the m_LevelSelectionScreen's EventRegistry.

            m_LevelSelectionScreen.BackButton.clicked += () => UIEvents.ScreenClosed?.Invoke();
            m_LevelSelectionScreen.StartButton.clicked += () => GameEvents.GameStarted?.Invoke();

            // Assign ClickEvent handlers for each button in the navigation bar
            for (int i = 0; i < m_Quizzes.Length; i++)
            {
                int index = i;

                // Raises the static LevelSelectionEvents with the button index

                m_LevelSelectionScreen.NavigationBar.Buttons[index].clicked += () => LevelSelectionEvents.ButtonSelected(index);
            }
        }

        // Load the first quiz and highlight first button
        private void OnLoadFirstQuiz()
        {
            if (m_Quizzes[0] != null)
            {
                LevelSelectionEvents.QuizDataLoaded?.Invoke(m_Quizzes[0]);
                m_LevelSelectionScreen.NavigationBar.HighlightButton(0);
            }
        }

        private List<QuizSO> ConvertQuizToSO(List<QuizPack> quizPacks, Sprite icon)
        {
            List<QuizSO> quizSOs = new();
            foreach (var pack in quizPacks)
            {
                var quizSO = ScriptableObject.CreateInstance<QuizSO>();
                quizSO.Initialize(pack.Id, pack.Title, pack.Summary,
                                pack.DifficultyLevel, pack.EstimatedTime, icon,
                                null);
                quizSOs.Add(quizSO);
            }
            return quizSOs;
        }

        private List<QuestionSO> ConvertQuestionToSO(List<Question> questions)
        {
            List<QuestionSO> questionSOs = new();
            foreach (var question in questions)
            {
                var questionSO = ScriptableObject.CreateInstance<QuestionSO>();
                List<Answer> answers = new();
                foreach (var answer in question.Answers)
                {
                    Answer ans;
                    ans.IsCorrect = answer.IsCorrect;
                    ans.Text = answer.AnswerText;
                    answers.Add(ans);
                }
                QuestionText questionText = new QuestionText { FontSize = QuestionFontSize.Medium, Text = question.QuestionText };
                questionSO.Initialize(new() { questionText }, "", true, answers, question.CorrectFeedback, question.IncorrectFeedback);
                questionSOs.Add(questionSO);
            }
            return questionSOs;
        }
    }
}
