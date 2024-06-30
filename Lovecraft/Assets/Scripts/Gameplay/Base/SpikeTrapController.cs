using System.Collections;
using UnityEngine;

public class SpikeTrapController : MonoBehaviour
{
    public Transform Spikes;
    public float riseHeight = 2f;
    public float riseTime = 0.5f;
    public float stayUpTime = 1f;
    public float sinkTime = 1f;

    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = Spikes.position;
        StartCoroutine(SpikeRoutine());
    }

    IEnumerator SpikeRoutine()
    {
        while (true)
        {
            //rise
            yield return StartCoroutine(MoveSpike(initialPosition, initialPosition + Vector3.up * riseHeight, riseTime));
            //stay
            yield return new WaitForSeconds(stayUpTime);
            //fall
            yield return StartCoroutine(MoveSpike(initialPosition + Vector3.up * riseHeight, initialPosition, sinkTime));
        }
    }

    IEnumerator MoveSpike(Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;
        while(elapsed < duration)
        {
            Spikes.position = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Spikes.position = to;
    }
}
