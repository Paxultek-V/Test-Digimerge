using UnityEngine;

public class Spawner_Animator : MonoBehaviour
{
    private Animator m_animator = null;


    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }


    private void OnEnable()
    {
        Spawner_ValueActor.OnSpawnValue += OnSpawnValue;
    }

    private void OnDisable()
    {
        Spawner_ValueActor.OnSpawnValue -= OnSpawnValue;
    }

    private void OnSpawnValue(ValueActor_Value value, Vector3 pos, float a, float b)
    {
        m_animator.ResetTrigger("Shoot");
        m_animator.SetTrigger("Shoot");
    }
}
