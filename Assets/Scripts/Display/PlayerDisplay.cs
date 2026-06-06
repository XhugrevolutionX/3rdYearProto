using UnityEngine;

public class PlayerDisplay : TypeDisplay
{
    [Header("Player References")] 
    [SerializeField] private ParticleSystem[] particles;

    protected override void ApplyColor(Color color)
    {
        base.ApplyColor(color);
        Color.RGBToHSV(color, out float h, out _, out float v);
        Color particleColor = Color.HSVToRGB(h, 0.5f, v);

        foreach (var particle in particles)
        {
            var main = particle.main;
            main.startColor = particleColor;
        }
    }
}
