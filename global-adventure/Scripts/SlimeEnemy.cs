using Godot;

namespace GlobalAdventure.Scripts;

public partial class SlimeEnemy : CharacterBody2D
{
    public void Destroy()
    {
        CallDeferred(Node.MethodName.QueueFree);
    }
}
