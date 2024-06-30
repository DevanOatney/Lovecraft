using UnityEngine;

public class BreathingEffect : MonoBehaviour
{
    public float scaleAmount = 0.1f;
    public float duration = 2f;

    private Vector3 initialScale;
    private float timer;

    private void Start()
    {
        initialScale = transform.localScale;
    }

    private void Update()
    {
        float scaleFactor = 1 + scaleAmount * Mathf.Sin((timer / duration) * Mathf.PI * 2);
        transform.localScale = initialScale * scaleFactor;

        timer += Time.deltaTime;
        if (timer > duration)
        {
            timer -= duration;
        }
    }
}
