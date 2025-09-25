using System.Threading.Tasks;
using Godot;

namespace GlobalAdventure.Scripts;

public partial class SlimeEnemy : CharacterBody2D
{
    [Export]
    public int Speed = 50;
    [Export]
    public int Health = 2;
    [Export]
    public int Acceleration = 5;
    private Area2D _playerDetectionArea;
    private Player _playerTarget;
    private AnimatedSprite2D _animatedSprite;
    private AudioStreamPlayer2D _damageSFX;
    private GpuParticles2D _deathFX;
    private CollisionShape2D _hitBox;
    
    public override void _Ready()
    {
        _playerDetectionArea = GetNode<Area2D>("PlayerDetectionArea");
        _playerDetectionArea.BodyEntered += PlayerInPlayerDetectionArea;
        _animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _damageSFX = GetNode<AudioStreamPlayer2D>("DamageSFX");
        _deathFX = GetNode<GpuParticles2D>("DeathFX");
        _hitBox = GetNode<CollisionShape2D>("CollisionShape2D");
    }

    public override void _PhysicsProcess(double delta)
    {
        if(Health <= 0) return;
        ChasePlayer();
        Animate();
        MoveAndSlide();
    }

    private void ChasePlayer()
    {
        if(_playerTarget is null) return;
        var distanceToTarget = _playerTarget.GlobalPosition - GlobalPosition;
        var directionToTargetNormalised = distanceToTarget.Normalized();
        // Velocity = directionToTargetNormalised * Speed;
        Velocity = Velocity.MoveToward(directionToTargetNormalised * Speed, Acceleration);
    }

    private void Animate()
    {
        var normalVelocity = Velocity.Normalized();
        if (normalVelocity.X > 0.707f)
        {
            _animatedSprite.Play(CommonAnimationDictionary.Get(Action.Move, Direction.Right));
        }
        if (normalVelocity.X < -0.707f)
        {
            _animatedSprite.Play(CommonAnimationDictionary.Get(Action.Move, Direction.Left));
        }
        if (normalVelocity.Y > 0.707f)
        {
            _animatedSprite.Play(CommonAnimationDictionary.Get(Action.Move, Direction.Down));
        }
        if (normalVelocity.Y < -0.707f)
        {
            _animatedSprite.Play(CommonAnimationDictionary.Get(Action.Move, Direction.Up));
        }
    }


    private void PlayerInPlayerDetectionArea(Node2D body)
    {
        if (body is not Player player) return; 
        _playerTarget = player;
    }

    public async Task Destroy()
    {
        _deathFX.Emitting = true;
        CallDeferred(nameof(Disable));
        await ToSignal(GetTree().CreateTimer(1), "timeout");
        CallDeferred(Node.MethodName.QueueFree);
    }

    private void Disable()
    {
        _animatedSprite.Visible = false;
        _hitBox.Disabled = true;
    }

    public async Task Hit(Vector2 positionHitFrom, int strength)
    {
        var distanceToEnemy = GlobalPosition - positionHitFrom;
        var knockBackDirection = distanceToEnemy.Normalized();
        Velocity += knockBackDirection * strength;
        var hitColor = new Color(50, 0, 0);
        var ogColor = new Color(1, 1, 1);
        Modulate = hitColor;
        _damageSFX.Play();
        await ToSignal(GetTree().CreateTimer(0.2), "timeout");
        if(!IsInstanceValid(this)) return;
        Modulate = ogColor;
        
        Health--;
        if (Health <= 0)
        {        
            Destroy();
        }
    }
}
