using Godot;

namespace GlobalAdventure.Scripts;

public partial class Player : CharacterBody2D
{
    [Export]
    public int Speed = 100;
    private int _score = 200;
    private float _money = 15.5f;
    private float _damage = 7.5f;
    private string _userName = "Bob";
    private string _firstName = "Paul";
    private bool _isPlayerAlive;
    private AnimatedSprite2D _animatedSprite;

    public override void _Ready()
    {
        _animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }
    public override void _Process(double delta)
    {
        var moveVector = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        Velocity = moveVector * Speed;
        if (Velocity.X > 0)
        {
            _animatedSprite.Play("move_right");
        }
        else if (Velocity.X < 0)
        {
            _animatedSprite.Play("move_left");
        }
        else if (Velocity.Y > 0)
        {
            _animatedSprite.Play("move_down");
        }
        else if (Velocity.Y < 0)
        {
            _animatedSprite.Play("move_up");
        }
        else
        {
            _animatedSprite.Stop();
        }
        
        MoveAndSlide();
    }
    
}
