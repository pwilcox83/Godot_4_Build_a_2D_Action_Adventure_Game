using System.Collections.Generic;
using System.Linq;
using Godot;

namespace GlobalAdventure.Scripts;

public partial class LockedDoor : StaticBody2D
{
    private CollisionShape2D _collisionShape2D;
    private Sprite2D _animatedSprite2D;
    private List<bool> _numberOfButtonsPressed;
    public bool IsDoorOpen { get; private set; }

    public override void _Ready()
    {
        _collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
        _animatedSprite2D = GetNode<Sprite2D>("Sprite2D");
    }

    public void OpenDoor(bool openState)
    {
        if (IsDoorOpen == openState)
        {
            return;
        }

        IsDoorOpen = openState;
        CallDeferred(nameof(ChangeDoorState));
        
    }

    private void ChangeDoorState() 
    {
        if (IsDoorOpen)
        {
            _collisionShape2D.Disabled = true;
            _animatedSprite2D.Visible = false;
        }
        else
        {
            _collisionShape2D.Disabled = false;
            _animatedSprite2D.Visible = true;
        }
    }
}
