using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManagerState : MonoBehaviour
{
    public static QuestManagerState Instance;

    public enum TildaState
    {
        Initial,
        Second,
        VisitedMuffin,
        Final
    }


    public TildaState tildaDialogueState = TildaState.Initial;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // persists across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }
}