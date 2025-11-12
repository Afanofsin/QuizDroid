using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace Quiz
{
    /// <summary>
    /// Adds a simple floating animation to a VisualElement, such as the title on the start screen.
    /// Attach this to any GameObject with a UIDocument.
    /// </summary>
    public class TitleFloatEffect : MonoBehaviour
    {
        [Tooltip("The name of the VisualElement to animate (e.g. 'menu__title')")]
        [SerializeField] string m_TitleElementName = "menu__title";

        [Tooltip("Amplitude of the vertical float in pixels.")]
        [SerializeField] float m_FloatOffset = 10f;

        [Tooltip("Time in milliseconds for one half of the float loop.")]
        [SerializeField] int m_Duration = 1000;

        VisualElement m_Title;
        float m_OriginalTop;

        void OnEnable()
        {
            var root = GetComponent<UIDocument>()?.rootVisualElement;
            if (root == null)
            {
                Debug.LogWarning("UIDocument not found.");
                return;
            }

            m_Title = root.Q<VisualElement>(m_TitleElementName);
            if (m_Title == null)
            {
                Debug.LogWarning($"Title element '{m_TitleElementName}' not found.");
                return;
            }

            m_OriginalTop = m_Title.resolvedStyle.top;
            StartFloat();
        }

        void StartFloat()
        {
            AnimateTo(m_OriginalTop - m_FloatOffset);
        }

        void AnimateTo(float targetY)
        {
            m_Title.experimental.animation
                .Position(new Vector2(m_Title.resolvedStyle.left, targetY), m_Duration)
                .Ease(Easing.InOutSine)
                .OnCompleted(() =>
                {
                    float nextY = targetY == m_OriginalTop ? m_OriginalTop - m_FloatOffset : m_OriginalTop;
                    AnimateTo(nextY);
                });
        }
    }
}