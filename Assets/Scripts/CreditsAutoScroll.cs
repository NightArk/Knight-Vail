using UnityEngine;
using UnityEngine.UI;

public class CreditsAutoScroll : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float scrollSpeed = 100f;
    public float scrollDelay = 2f; 

    private RectTransform content;
    private bool autoScroll = false;
    private float timer = 0f;
    private bool userPaused = false;

    void Start()
    {
        content = scrollRect.content;
        ResetScrollPosition();
        timer = 0f;
        autoScroll = false;
        userPaused = false;
    }

    void Update()
    {
        
        if (autoScroll && Input.GetMouseButtonDown(0))
        {
            userPaused = true;
            autoScroll = false;
        }

        if (!autoScroll && !userPaused)
        {
            timer += Time.deltaTime;
            if (timer >= scrollDelay)
            {
                autoScroll = true;
            }
        }

        if (autoScroll && content != null)
        {
            float newY = scrollRect.verticalNormalizedPosition - (scrollSpeed * Time.deltaTime / content.rect.height);
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(newY);
        }
    }

    public void StartScrolling()
    {
        ResetScrollPosition();
        timer = 0f;
        userPaused = false;
        autoScroll = false;
    }

    public void StopScrolling()
    {
        autoScroll = false;
        userPaused = true;
    }

    public void ResetScrollPosition()
    {
        scrollRect.verticalNormalizedPosition = 1f;
    }
}
