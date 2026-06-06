using UnityEngine;

public class PlayerDisplay : TypeDisplay
{
    [Header("Player References")] 
    [SerializeField] private ParticleSystem[] particles;

    protected override void ApplyColor(Color color)
    {
        base.ApplyColor(color);
        Color.RGBToHSV(color, out float h, out float s, out float v);
        Color particleColor = s == 0f ? color : Color.HSVToRGB(h, 0.8f, v);

        foreach (var particle in particles)
        {
            var main = particle.main;
            main.startColor = particleColor;
        }
    }
}
