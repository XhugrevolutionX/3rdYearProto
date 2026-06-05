public class BossDisplay : TypeDisplay
{
    protected override void Start()
    {
        healthBarSlider = UIManager.Instance?.BossHealthBarSlider;
        healthTmp = UIManager.Instance?.BossHealthTmp;
        base.Start();
    }
}