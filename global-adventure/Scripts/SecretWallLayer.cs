using Godot;

namespace GlobalAdventure.Scripts;

public partial class SecretWallLayer : TileMapLayer
{
    public override void _Ready()
    {
        
    }
    public void SetInvisibleWall(bool visible)
    {
        Visible = !visible;
        CollisionEnabled = !visible;
    }
}
