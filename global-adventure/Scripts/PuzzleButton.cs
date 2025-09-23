using Godot;

namespace GlobalAdventure.Scripts;

public partial class PuzzleButton : Area2D
{
    [Signal]
    public delegate void PuzzleButtonPressedEventHandler();

    [Signal]
    public delegate void PuzzleButtonUnpressedEventHandler();

    private AnimatedSprite2D _animatedSprite2D;

    private int _objectsInCollider;

    [Export] 
    public bool SingleUse;
    
    private AudioStreamPlayer2D _buttonSound;

    public bool Pressed { get; private set; }

    public override void _Ready()
    {
        _animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        BodyEntered += _ => PlayAnimation(true);
        BodyExited += _ => PlayAnimation(false);
        _buttonSound = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
    }

    private void PlayAnimation(bool entered)
    {
        if (entered)
        {
            _buttonSound.PitchScale = 1.0f;
            _buttonSound.Play();
            Pressed = true;
            EmitSignal(SignalName.PuzzleButtonPressed);
            _objectsInCollider++;
            _animatedSprite2D.Play("pressed");
            return;
        }

        _buttonSound.PitchScale = 0.8f;
        _buttonSound.Play();

        _objectsInCollider--;
        
        if (_objectsInCollider != 0 || SingleUse) return;
        
        Pressed = false;
        EmitSignal(SignalName.PuzzleButtonUnpressed);
        _animatedSprite2D.Play("unpressed");
    }
}
