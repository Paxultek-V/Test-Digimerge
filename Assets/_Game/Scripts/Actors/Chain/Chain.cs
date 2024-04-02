using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain : MonoBehaviour
{
    private List<ChainElement> m_chainList = new List<ChainElement>();

    private ChainElement m_chainElementBuffer;

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
            m_chainElementBuffer = child.gameObject.GetComponent<ChainElement>();
            
            if (m_chainElementBuffer != null)
                m_chainList.Add(m_chainElementBuffer);
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
            m_chainList[i].DestroyChainElement();
        }
    }
}