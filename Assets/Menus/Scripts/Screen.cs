using DG.Tweening;
using UnityEngine;

public class Screen : MonoBehaviour
{
    [SerializeField] private ScreenManager.ScreenType m_screenType;
    [SerializeField] private RectTransform m_content;
    [SerializeField] private CanvasGroup m_backPanel;

    [SerializeField] private bool m_isOpen = false;
    [SerializeField] private float m_animateSpeed = 0.5f;

    private void Awake()
    {
        m_backPanel.DOFade(0.0f, 0f);
        m_content.DOScale(0.0f, 0f);
    }

    public void Open()
    {
        if (m_isOpen)
        {
            return;
        }

        m_isOpen = true;
        m_backPanel.DOFade(1.0f, m_animateSpeed);
        m_content.gameObject.SetActive(true);
        m_content.DOScale(1.0f, m_animateSpeed);
    }

    public void Close()
    {
        if (!m_isOpen)
        {
            return;
        }

        m_isOpen = false;
        m_backPanel.DOFade(0.0f, m_animateSpeed);
        m_content.DOScale(0.0f, m_animateSpeed).OnComplete(() =>
        {
            m_content.gameObject.SetActive(false);
        });
    }
}
