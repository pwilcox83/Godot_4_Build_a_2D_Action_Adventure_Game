using Godot;

namespace GlobalAdventure.Scripts;

public partial class Switch : StaticBody2D
{
    [Export]
    public bool IsPuzzleSwitch;
    
    public bool IsInteractable { get; set; }
    public bool IsSwitchedOn { get; private set; }
    
    [Signal]
    public delegate void SwitchedOffEventHandler();
    [Signal]
    public delegate void SwitchedOnEventHandler();
    
    
    
    private AnimatedSprite2D _animatedSprite2D;

    public override void _Ready()
    {
        _animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }
    public override void _Process(double delta)
    {
        if (!Input.IsActionJustPressed("interact") || !IsInteractable) return;
        
        IsSwitchedOn = !IsSwitchedOn;
        _animatedSprite2D.Play(IsSwitchedOn ? "activated" : "deactivated");

        if (IsSwitchedOn)
        {
            EmitSignal(SignalName.SwitchedOn);
            
        }
        else
        {
            EmitSignal(SignalName.SwitchedOff);
        }
    }
}
