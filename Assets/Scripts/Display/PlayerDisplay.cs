using UnityEngine;

public class PlayerDisplay : TypeDisplay
{
    [Header("Player References")] 
    [SerializeField] private ParticleSystem[] particles;
    
    [Header("Player Parameters")]
    [SerializeField] private Gradient redGradient;
    [SerializeField] private Gradient greenGradient;
    [SerializeField] private Gradient blueGradient;
    [SerializeField] private Gradient yellowGradient;
    [SerializeField] private Gradient orangeGradient;
    [SerializeField] private Gradient purpleGradient;

    public override void SetType(TypeColor type)
    {
        base.SetType(type);

        Gradient gradient = type switch
        {
            TypeColor.RED    => redGradient,
            TypeColor.GREEN  => greenGradient,
            TypeColor.BLUE   => blueGradient,
            TypeColor.YELLOW => yellowGradient,
            TypeColor.ORANGE => orangeGradient,
            TypeColor.PURPLE => purpleGradient,
            _                => null
        };

        if (gradient == null) return;

        foreach (var particle in particles)
        {
            var col = particle.colorOverLifetime;
            col.enabled = true;
            col.color = new ParticleSystem.MinMaxGradient(gradient);
        }
    }

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
