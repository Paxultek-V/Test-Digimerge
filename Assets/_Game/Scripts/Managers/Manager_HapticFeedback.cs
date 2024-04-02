using UnityEngine;

public class Manager_HapticFeedback : MonoBehaviour
{
    private void OnEnable()
    {
        Taptic.tapticOn = true;

    }

    private void OnDisable()
    {
        Taptic.tapticOn = false;

    }

    private void PlayLightHaptic()
    {
        Taptic.Light();
    }

    private void PlayHeavyHaptic()
    {
        Taptic.Heavy();
    }

}
