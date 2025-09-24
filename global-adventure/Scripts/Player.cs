using System.Threading.Tasks;
using Godot;

namespace GlobalAdventure.Scripts;

public partial class Player : CharacterBody2D
{
    [Export] public int Acceleration = 10;

    [Export] public int Health = 100;

    [Export] public int PushStrength = 200;

    [Export] public int Speed = 100;

    [Export] public int WeaponStrength = 200;
    private AnimatedSprite2D _animatedSprite;
    private AnimationPlayer _animationPlayer;

    private float _damage = 7.5f;
    private Timer _deathTimer;
    private Direction _direction = Direction.Down;
    private Area2D _interactionArea;
    private bool _isAttacking;
    private bool _canInteract;
    private bool _isPlayerAlive = true;
    private bool _isPushedBacked;
    
    private AnimatedSprite2D _playerHealthIndicator;
    private Area2D _playerHitBox;

    private SceneManager _sceneManager;

    private int _scrollCount;

    private Label _scrollCountLabel;
    private Area2D _weaponArea2d;

    private Sprite2D _weaponSprite;

    private Timer _weaponTimer;
    
    
    public override void _Ready()
    {
        _sceneManager = GetNode<SceneManager>("/root/SceneManager");
        SetupAnimationInstances();
        SetupArea2ds();
        SetupUi();
        SetupTimers();
        _weaponSprite = GetNode<Sprite2D>("WeaponSprite");
    }

    public override void _Process(double delta)
    {
        _scrollCountLabel.Text = _sceneManager.OpenedChests.Count.ToString();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_isPlayerAlive) return;
        DetermineDirection();
        MovePlayer();
        HandlePlayerCollisionsWithPushableObjects();
        MoveAndSlide();
    }

    private void DetermineDirection()
    {
        if(Input.IsActionJustPressed("move_up")) _direction = Direction.Up;
        else if(Input.IsActionJustPressed("move_down")) _direction = Direction.Down;
        else if(Input.IsActionJustPressed("move_left")) _direction = Direction.Left;
        else if(Input.IsActionJustPressed("move_right")) _direction = Direction.Right;
    }

    private void MovePlayer()
    {
        if (_isAttacking) return;
        var moveVector = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        Velocity = Velocity.MoveToward(moveVector.Normalized() * Speed, Acceleration);
        
        if (Velocity.X > 0)
        {
            _interactionArea.Position = new Vector2(5, 2);
        }
        else if (Velocity.X < 0)
        {
            _interactionArea.Position = new Vector2(-5, 2);
        }
        else if (Velocity.Y > 0)
        {
            _interactionArea.Position = new Vector2(0, 8);
        }
        else if (Velocity.Y < 0)
        {
            _interactionArea.Position = new Vector2(0, -4);
        }

        if (Velocity == Vector2.Zero)
        {
            _animatedSprite.Stop();

        }
        else
        {
            _animatedSprite.Play(CommonAnimationDictionary.Get(Action.Move, _direction));
        }
        
        if (Input.IsActionJustPressed("interact") && !_canInteract)
        {
            Attack();
        }
    }

    private void InteractWithGameObject(Node2D body, bool active)
    {
        _canInteract = active;
        switch (body)
        {
            case Npc npc when npc.IsInGroup("interactable"):
                npc.IsInteractable = active;
                break;
            case Switch gameSwitch when gameSwitch.IsInGroup("interactable"):
                gameSwitch.IsInteractable = active;
                break;
            case TreasureChest tressureChest when tressureChest.IsInGroup("interactable"):
                tressureChest.IsInteractable = active;
                break;
            default:
                break;
        }
    }

    private void HideWeapon()
    {
        _isAttacking = false;
        ActivateWeapon(false);
    }

    private void EnemyEnteredWeaponArea2d(Node2D body)
    {
        if (body is not SlimeEnemy enemy) return;
        enemy.Hit(GlobalPosition, WeaponStrength);
    }

    private void Attack()
    {
        if (!_weaponTimer.IsStopped()) return;
        ActivateWeapon(true);
        _weaponTimer.Start();

        _isAttacking = true;
        Velocity = Vector2.Zero;
        _animationPlayer.Play(CommonAnimationDictionary.Get(Action.Attack, _direction));
        _animatedSprite.Play(CommonAnimationDictionary.Get(Action.Move, _direction));
    }

    private void ActivateWeapon(bool visible)
    {
        _weaponSprite.Visible = visible;
        _weaponArea2d.Monitoring = visible;

        if (visible) return;
        _animatedSprite.Play(CommonAnimationDictionary.Get(Action.Move, _direction));
    }

    private void HandlePlayerCollisionsWithPushableObjects()
    {
        var collision = GetLastSlideCollision();

        if (collision?.GetCollider() is not Node node || !node.IsInGroup("pushable")) return;
        if (node is not RigidBody2D rigidBodyNode) return;

        var normal = collision.GetNormal();
        rigidBodyNode.ApplyCentralForce(-normal * PushStrength);
    }

    private async Task PlayerHitByGameObject(Node2D body)
    {
        if (body is not SlimeEnemy slimeEnemy) return;
        _isPushedBacked = true;
        _sceneManager.PlayerHealth--;
        var flashWhiteColor = new Color(50, 50, 50);
        var _ogColor = new Color(1, 1, 1);
        Modulate = flashWhiteColor;
        await ToSignal(GetTree().CreateTimer(0.2), "timeout");
        _playerHealthIndicator.Frame = _sceneManager.PlayerHealth;
        Modulate = _ogColor;
        var distanceToPlayer = GlobalPosition - slimeEnemy.GlobalPosition;
        var knockBackDirection = distanceToPlayer.Normalized();
        var knockBackForce = 250;
        Velocity += knockBackDirection * knockBackForce;
        if (_sceneManager.PlayerHealth >= 0) return;
        CallDeferred(MethodName.Die);
    }
    

    private void Die()
    {
        _isPlayerAlive = false;
        _animatedSprite.Play(CommonAnimationDictionary.Get(Action.Death, Direction.NotRequired));
        if (!_deathTimer.IsStopped()) return;
        _deathTimer.Start();
    }

    private void SetupUi()
    {
        _scrollCountLabel = GetNode<Label>("%TreasureLabel");
        _scrollCountLabel.Text = _sceneManager.OpenedChests.Count.ToString();
        _playerHealthIndicator.Frame = _sceneManager.PlayerHealth;
    }

    private void SetupTimers()
    {
        _weaponTimer = GetNode<Timer>("WeaponSprite/WeaponTimer");
        _weaponTimer.Timeout += HideWeapon;

        _deathTimer = GetNode<Timer>("DeathTimer");
        _deathTimer.Timeout += ReloadScene;
    }

    private void ReloadScene()
    {
        _sceneManager.PlayerHealth = 3;
        GetTree().ReloadCurrentScene();
    }

    private void SetupArea2ds()
    {
        _interactionArea = GetNode<Area2D>("InteractionArea");
        _interactionArea.BodyEntered += body => InteractWithGameObject(body, true);
        _interactionArea.BodyExited += body => InteractWithGameObject(body, false);
        ;
        _playerHitBox = GetNode<Area2D>("HitBoxArea2d");
        _playerHitBox.BodyEntered += (body) => PlayerHitByGameObject(body);
        _weaponArea2d = GetNode<Area2D>("WeaponSprite/WeaponArea2D");
        _weaponArea2d.BodyEntered += EnemyEnteredWeaponArea2d;
    }

    private void SetupAnimationInstances()
    {
        _animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _playerHealthIndicator = GetNode<AnimatedSprite2D>("%PlayerHealthIndicator");
    }
}
