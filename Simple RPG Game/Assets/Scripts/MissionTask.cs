using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MissionTask : MonoBehaviour
{
    public int numofEnemies;
    private int enemyCount;

    public UnityEvent missionComplete;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       if(enemyCount == numofEnemies)
       {
            missionComplete.Invoke();
       }  
    }

    public void IsDead()
    {
        enemyCount++;
    }
}
