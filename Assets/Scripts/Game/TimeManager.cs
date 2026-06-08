using System;
using System.Collections;
using UnityEngine;

[Serializable]
public struct HitStopData
{
    public float Duration;
    public float TimeScale;
}

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

    public void DoHitStop(HitStopData data)
    {
        float endTime = Time.unscaledTime + data.Duration;
        if (endTime <= _hitStopEndTime) return;

        _hitStopEndTime = endTime;

        if (_hitStopCoroutine != null) StopCoroutine(_hitStopCoroutine);
        _hitStopCoroutine = StartCoroutine(HitStopRoutine(data));
    }

    private IEnumerator HitStopRoutine(HitStopData data)
    {
        Time.timeScale = data.TimeScale;
        yield return new WaitForSecondsRealtime(data.Duration);
        Time.timeScale = 1f;
        _hitStopCoroutine = null;
    }
}
