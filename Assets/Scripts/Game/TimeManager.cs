using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    private Coroutine _hitStopCoroutine;
    private float _hitStopEndTime;

    private void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Pause()
    {
        if (_hitStopCoroutine != null) { StopCoroutine(_hitStopCoroutine); _hitStopCoroutine = null; }
        _hitStopEndTime = 0f;
        Time.timeScale = 0f;
    }

    public void DoHitStop(float duration, float timeScale = 0f)
    {
        float endTime = Time.unscaledTime + duration;
        if (endTime <= _hitStopEndTime) return;

        _hitStopEndTime = endTime;

        if (_hitStopCoroutine != null) StopCoroutine(_hitStopCoroutine);
        _hitStopCoroutine = StartCoroutine(HitStopRoutine(duration, timeScale));
    }

    private IEnumerator HitStopRoutine(float duration, float timeScale)
    {
        Time.timeScale = timeScale;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        _hitStopCoroutine = null;
    }
}
