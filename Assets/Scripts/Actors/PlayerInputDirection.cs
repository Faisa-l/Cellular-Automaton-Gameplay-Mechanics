using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInputDirection", menuName = "Custom/PlayerInputDirection")]
public class PlayerInputDirection : ScriptableObject
{
    public Vector2 input = new();
}