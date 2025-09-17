using Godot;

namespace GlobalAdventure.Scripts;

public partial class PuzzleButton : Area2D
{
    private int _objectsInColider = 0;
    private AnimatedSprite2D _animatedSprite2D;
    
    [Signal]
    public delegate void PuzzleButtonPressedEventHandler();
    [Signal]
    public delegate void PuzzleButtonUnpressedEventHandler();

    public bool Pressed { get; private set; }

    public override void _Ready()
    {
        _animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        BodyEntered += _ => PlayAnimation(true);
        BodyExited += _ => PlayAnimation(false);
    }

    private void PlayAnimation( bool entered)
    {
        if (entered)
        {
            Pressed = true;
            EmitSignal(SignalName.PuzzleButtonPressed);
            _objectsInColider++;
            _animatedSprite2D.Play("pressed");
            return;
        }
        _objectsInColider--;
        if (_objectsInColider == 0)
        {
            Pressed = false;
            EmitSignal(SignalName.PuzzleButtonUnpressed);
            _animatedSprite2D.Play("unpressed");    
        }
    }
}
