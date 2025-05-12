using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public GameObject[] coins;

    public void SetCoinsActive(bool state)
    {
        foreach (GameObject coin in coins)
        {
            coin.SetActive(state);
        }
    }
}