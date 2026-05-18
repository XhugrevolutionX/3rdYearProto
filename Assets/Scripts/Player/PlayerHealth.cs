using UnityEngine;

public class PlayerHealth : Health
{
    protected override void Death()
    {
        Debug.Log("Player is death");
    }
}