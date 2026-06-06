using UnityEngine;

public class TypeDisplay : Display
{
    [Header("Player References")]
    [SerializeField] private Renderer characterRenderer;

    [Header("Type Colors")]
    [SerializeField] private Color whiteColor  = Color.white;
    [SerializeField] private Color redColor    = Color.red;
    [SerializeField] private Color greenColor  = Color.green;
    [SerializeField] private Color blueColor   = Color.blue;
    [SerializeField] private Color yellowColor = Color.yellow;
    [SerializeField] private Color orangeColor = new(1f, 0.5f, 0f);
    [SerializeField] private Color purpleColor = new(0.5f, 0f, 0.5f);

    public void SetType(TypeColor type)
    {
        Color color = type switch
        {
            TypeColor.RED    => redColor,
            TypeColor.GREEN  => greenColor,
            TypeColor.BLUE   => blueColor,
            TypeColor.YELLOW => yellowColor,
            TypeColor.ORANGE => orangeColor,
            TypeColor.PURPLE => purpleColor,
            _                => whiteColor
        };
        
        ApplyColor(color);
    }
    
    protected virtual void ApplyColor(Color color) => characterRenderer.material.SetColor("_Color", color);
}
