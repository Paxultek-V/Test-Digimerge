using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner_FxController : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_spawnFx = null;

    private void OnEnable()
    {
        Spawner_ValueActor.OnSpawnValueFromCanon += OnSpawnValueFromCanon;
    }

    private void OnDisable()
    {
        Spawner_ValueActor.OnSpawnValueFromCanon -= OnSpawnValueFromCanon;
    }

    private void OnSpawnValueFromCanon()
    {
        if (m_spawnFx.isPlaying)
            m_spawnFx.Stop();

        m_spawnFx.Play();
    }
}