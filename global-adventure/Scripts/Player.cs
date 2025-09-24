using System;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Metadata;
using Godot;

namespace GlobalAdventure.Scripts;

public partial class Player : CharacterBody2D
{
    [Export] 
    public int Health = 100;
    [Export] 
    public int PushStrength = 200;
    [Export]
    public int WeaponStrength = 200;
    [Export] 
    public int Speed = 100;
    [Export] 
    public int Acceleration = 10;
    
    private float _damage = 7.5f;
    private string _firstName = "Paul";

    private AnimatedSprite2D _animatedSprite;
    private AnimationPlayer _animationPlayer;
    private AnimatedSprite2D _playerHealthIndicator;
    
    private Area2D _interactionArea;
    private Area2D _playerHitBox;
    private Area2D _weaponArea2d;
    
    private Label _scrollCountLabel;
    
    private SceneManager _sceneManager;
    
    private Sprite2D _weaponSprite;
    
    private Timer _weaponTimer;
    private Timer _deathTimer;
    
    private bool _isPlayerAlive = true;
    private bool _isAttacking;
    
    private int _scrollCount;
    
    public override void _Ready()
    {
        _sceneManager =  GetNode<SceneManager>("/root/SceneManager");
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
        MovePlayer();
        HandlePlayerCollisionsWithPushableObjects();
        MoveAndSlide();
    }
    
    private void MovePlayer()
    {
        if(_isAttacking) return;
        var moveVector = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        Velocity = Velocity.MoveToward(moveVector.Normalized() * Speed, Acceleration);

        if (Velocity.X > 0)
        {
            _animatedSprite.Play("move_right");
            _interactionArea.Position = new Vector2(5, 2);
        }
        else if (Velocity.X < 0)
        {
            _animatedSprite.Play("move_left");
            _interactionArea.Position = new Vector2(-5, 2);
        }
        else if (Velocity.Y > 0)
        {
            _animatedSprite.Play("move_down");
            _interactionArea.Position = new Vector2(0, 8);
        }
        else if (Velocity.Y < 0)
        {
            _animatedSprite.Play("move_up");
            _interactionArea.Position = new Vector2(0, -4);
        }
        else
        {
            _animatedSprite.Stop();
        }

        if (Input.IsActionJustPressed("interact"))
        {
            Attack();
        }
    }
    
     private static void InteractWithGameObject(Node2D body, bool active)
    {
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
        if(body is not SlimeEnemy enemy) return;
        enemy.Hit(GlobalPosition, WeaponStrength);
    }
    
    private void Attack()
    {
        if(!_weaponTimer.IsStopped()) return;
        ActivateWeapon(true);
        _weaponTimer.Start();
        
        var playerAnimation = _animatedSprite.Animation;
        _isAttacking = true;
        Velocity = Vector2.Zero;
        switch (playerAnimation)
        {
            case "move_down":
                _animatedSprite.Play(CommonAnimationDictionary.Get(Action.Move, Direction.Down));
                _animationPlayer.Play(CommonAnimationDictionary.Get(Action.Attack, Direction.Down));
                break;
            case "move_up":
                _animatedSprite.Play(CommonAnimationDictionary.Get(Action.Move, Direction.Up));
                _animationPlayer.Play(CommonAnimationDictionary.Get(Action.Attack, Direction.Up));
                break;
            case "move_left":
                _animatedSprite.Play(CommonAnimationDictionary.Get(Action.Move, Direction.Left));
                _animationPlayer.Play(CommonAnimationDictionary.Get(Action.Attack, Direction.Left));
                break;
            case "move_right":
                _animatedSprite.Play(CommonAnimationDictionary.Get(Action.Move, Direction.Right));
                _animationPlayer.Play(CommonAnimationDictionary.Get(Action.Attack, Direction.Right));
                break;
            default:
                break;
            
        }
    }

    private void ActivateWeapon(bool visible)
    {
        _weaponSprite.Visible = visible;
        _weaponArea2d.Monitoring = visible;
        
        if(visible) return;
        var playerAnimation = _animatedSprite.Animation;
        switch (playerAnimation)
        {
            case "attack_down":
                _animatedSprite.Play(CommonAnimationDictionary.Get(Action.Move, Direction.Down));
                break;
            case "attack_up":
                _animatedSprite.Play(CommonAnimationDictionary.Get(Action.Move, Direction.Up));
                break;
            case "attack_left":
                _animatedSprite.Play(CommonAnimationDictionary.Get(Action.Move, Direction.Left));
                break;
            case "attack_right":
                _animatedSprite.Play(CommonAnimationDictionary.Get(Action.Move, Direction.Right));
                break;
        }
    }
    
    private void HandlePlayerCollisionsWithPushableObjects()
    {
        var collision = GetLastSlideCollision();

        if (collision?.GetCollider() is not Node node || !node.IsInGroup("pushable")) return;
        if (node is not RigidBody2D rigidBodyNode) return;

        var normal = collision.GetNormal();
        rigidBodyNode.ApplyCentralForce(-normal * PushStrength);
    }

    private void PlayerHitByGameObject(Node2D body)
    {
        if (body is not SlimeEnemy slimeEnemy) return;
        _sceneManager.PlayerHealth--;
        _playerHealthIndicator.Frame = _sceneManager.PlayerHealth;
        
        var distanceToPlayer = GlobalPosition - slimeEnemy.GlobalPosition;
        var knockBackDirection = distanceToPlayer.Normalized();
        var knockBackForce = 250;
        Velocity += knockBackDirection * knockBackForce;
        GD.Print($"Player health: {_sceneManager.PlayerHealth}");
        if (_sceneManager.PlayerHealth >= 0) return;
        CallDeferred(MethodName.Die);
    }
    
    private void Die()
    {
        _isPlayerAlive = false;
        _animatedSprite.Play(CommonAnimationDictionary.Get(Action.Death, Direction.NotRequired));
        if(!_deathTimer.IsStopped()) return;
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
        _interactionArea.BodyExited += body => InteractWithGameObject(body, false);;
        _playerHitBox = GetNode<Area2D>("HitBoxArea2d");
        _playerHitBox.BodyEntered += PlayerHitByGameObject;
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
