using System.Collections.Generic;
using System.Linq;
using Godot;

namespace GlobalAdventure.Scripts;

public partial class LockedDoor : StaticBody2D
{
    [Export]
    public int NumberOfButtonsRequired = 0;
    private CollisionShape2D _collisionShape2D;
    private Sprite2D _animatedSprite2D;
    private List<bool> _numberOfButtonsPressed;
    
    public override void _Ready()
    {
        if (NumberOfButtonsRequired > 0)
        {
           _numberOfButtonsPressed = [];
           for (var i = 0; i < NumberOfButtonsRequired; i++)
           {
               _numberOfButtonsPressed.Add(false);
           }
        }
        _collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
        _animatedSprite2D = GetNode<Sprite2D>("Sprite2D");
    }

    public void SetNumberOfButtonsRequired(int numberOfButtonsRequired)
    {
        if (numberOfButtonsRequired <= 0) return;
        _numberOfButtonsPressed = [];
        for (var i = 0; i < numberOfButtonsRequired; i++)
        {
            _numberOfButtonsPressed.Add(false);
        }
    }
    
    private void SetDoorOpen()
    {
        if (_numberOfButtonsPressed.All(x => x == true) )
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
    
    public void ButtonPressedState(int buttonNumber, bool b)
    {
        _numberOfButtonsPressed[buttonNumber] = b;
        CallDeferred(nameof(SetDoorOpen));
    }
}
