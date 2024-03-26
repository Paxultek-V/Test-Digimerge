using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain : MonoBehaviour
{
    private List<GameObject> m_chainList = new List<GameObject>();


    private void OnEnable()
    {
        PiggyBank.OnPiggyBankFinishedCollectingMoney += OnPiggyBankFinishedCollectingMoney;
    }

    private void OnDisable()
    {
        PiggyBank.OnPiggyBankFinishedCollectingMoney -= OnPiggyBankFinishedCollectingMoney;
    }

    private void Start()
    {
        foreach (Transform child in transform)
        {
            m_chainList.Add(child.gameObject);
        }
    }


    private void OnPiggyBankFinishedCollectingMoney(float amountCollected)
    {
        DestroyChain();
    }

    private void DestroyChain()
    {
        for (int i = 0; i < m_chainList.Count; i++)
        {
            m_chainList[i].SetActive(false);
        }
    }
}
