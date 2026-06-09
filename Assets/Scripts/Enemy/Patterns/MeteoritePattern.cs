using UnityEngine;

public class MeteoritePattern : BossPattern
{
    private readonly MeteoriteStrike _strikePrefab;
    private readonly float _duration;
    private readonly float _interval;
    private readonly float _minSpawnRadius;
    private readonly float _maxSpawnRadius;

    private float _durationTimer;
    private float _intervalTimer;

    private static readonly TypeColor[] AllTypes =
    {
        TypeColor.RED, TypeColor.GREEN, TypeColor.BLUE,
        TypeColor.YELLOW, TypeColor.ORANGE, TypeColor.PURPLE
    };

    public MeteoritePattern(MeteoriteStrike strikePrefab, float duration, float interval, float minSpawnRadius, float maxSpawnRadius)
    {
        _strikePrefab = strikePrefab;
        _duration = duration;
        _interval = interval;
        _minSpawnRadius = minSpawnRadius;
        _maxSpawnRadius = maxSpawnRadius;
    }

    public override void Begin(BossController boss)
    {
        base.End(boss);
        _durationTimer = _duration;
        _intervalTimer = _interval;
        SpawnStrike(boss);
    }

    public override void Tick(BossController boss)
    {
        _durationTimer -= Time.deltaTime;
        if (_durationTimer <= 0f) { IsComplete = true; return; }

        _intervalTimer -= Time.deltaTime;
        if (_intervalTimer <= 0f)
        {
            _intervalTimer = _interval;
            SpawnStrike(boss);
        }
    }

    private void SpawnStrike(BossController boss)
    {
        Vector2 offset = Random.insideUnitCircle.normalized * Random.Range(_minSpawnRadius, _maxSpawnRadius);
        Vector3 position = boss.transform.position + new Vector3(offset.x, 0f, offset.y);
        TypeColor type = AllTypes[Random.Range(0, AllTypes.Length)];

        var strike = Object.Instantiate(_strikePrefab, position, Quaternion.identity);
        strike.Init(position, boss, type);
    }
}
