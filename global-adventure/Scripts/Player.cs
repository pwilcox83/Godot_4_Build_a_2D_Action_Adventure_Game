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
    [Export] public int Speed = 100;
    
    private AnimatedSprite2D _animatedSprite;
    private float _damage = 7.5f;
    private string _firstName = "Paul";
    private Area2D _interactionArea;
    private bool _isPlayerAlive;
    private float _money = 15.5f;
    private int _score = 200;
    private string _userName = "Bob";
    private int _scrollCount;
    private Label _scrollCountLabel;
    private SceneManager _sceneManager;
    private Area2D _playerHitBox;
    private AnimatedSprite2D _playerHealthIndicator;
    private Area2D _weaponArea2d;
    private Sprite2D _weaponSprite;
    private Timer _weaponTimer;
    private bool _isAttacking;
    private AnimationPlayer _animationPlayer;

    public override void _Ready()
    {
        _sceneManager =  GetNode<SceneManager>("/root/SceneManager");
        _animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _interactionArea = GetNode<Area2D>("InteractionArea");
        _interactionArea.BodyEntered += InteractionAreaEntered;
        _interactionArea.BodyExited += InteractionAreaExited;
        _scrollCountLabel = GetNode<Label>("%TreasureLabel");
        _scrollCountLabel.Text = _sceneManager.OpenedChests.Count.ToString();
        _playerHitBox = GetNode<Area2D>("HitBoxArea2d");
        _playerHitBox.BodyEntered += PlayerHitByGameObject;
        _playerHealthIndicator = GetNode<AnimatedSprite2D>("%PlayerHealthIndicator");
        _playerHealthIndicator.Frame = _sceneManager.PlayerHealth;
        _weaponArea2d = GetNode<Area2D>("WeaponSprite/WeaponArea2D");
        _weaponArea2d.BodyEntered += EnemyEnteredWeaponArea2d;
        _weaponSprite = GetNode<Sprite2D>("WeaponSprite");
        _weaponTimer = GetNode<Timer>("WeaponSprite/WeaponTimer");
        _weaponTimer.Timeout += HideWeapon;
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }
    
    private void PlayerHitByGameObject(Node2D body)
    {
        if (body is not SlimeEnemy) return;
        _sceneManager.PlayerHealth--;
        _playerHealthIndicator.Frame = _sceneManager.PlayerHealth;
        if (_sceneManager.PlayerHealth >= 0) return;
        CallDeferred(MethodName.Die);
    }
    
    private void Die()
    {
        _sceneManager.PlayerHealth = 3;
        GetTree().ReloadCurrentScene();
    }

    private void InteractionAreaEntered(Node2D body)
    {
        InteractWithGameObject(body, true);
    }
    
    private void InteractionAreaExited(Node2D body)
    {
        InteractWithGameObject(body, false);
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

    public override void _Process(double delta)
    {
        _scrollCountLabel.Text = _sceneManager.OpenedChests.Count.ToString();
    }

    public override void _PhysicsProcess(double delta)
    {
        MovePlayer();
        HandlePlayerCollisionsWithPushableObjects();
        MoveAndSlide();
    }

    private void MovePlayer()
    {
        if(_isAttacking) return;
        var moveVector = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        Velocity = moveVector * Speed;
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

    private void HandlePlayerCollisionsWithPushableObjects()
    {
        var collision = GetLastSlideCollision();

        if (collision?.GetCollider() is not Node node || !node.IsInGroup("pushable")) return;
        if (node is not RigidBody2D rigidBodyNode) return;

        var normal = collision.GetNormal();
        rigidBodyNode.ApplyCentralForce(-normal * PushStrength);
    }
    
    private void HideWeapon()
    {
        _isAttacking = false;
        ActivateWeapon(false);
    }
    private void EnemyEnteredWeaponArea2d(Node2D body)
    {
        if(body is not SlimeEnemy enemy) return;
        HandleEnemyHitByWeapon(enemy);
    }

    private void HandleEnemyHitByWeapon(SlimeEnemy enemy)
    {
        enemy.Destroy();
    }
    
    private void Attack()
    {
        ActivateWeapon(true);
        _weaponTimer.Start();
        
        var playerAnimation = _animatedSprite.Animation;
        _isAttacking = true;
        Velocity = Vector2.Zero;
        switch (playerAnimation)
        {
            case "move_down":
                _animatedSprite.Play(MovePositions.MoveDown.GetDescription());
                _animationPlayer.Play(AttackPositions.AttackDown.GetDescription());
                break;
            case "move_up":
                _animatedSprite.Play(MovePositions.MoveUp.GetDescription());
                _animationPlayer.Play(AttackPositions.AttackUp.GetDescription());
                break;
            case "move_left":
                _animatedSprite.Play(MovePositions.MoveLeft.GetDescription());
                _animationPlayer.Play(AttackPositions.AttackLeft.GetDescription());
                break;
            case "move_right":
                _animatedSprite.Play(MovePositions.MoveRight.GetDescription());
                _animationPlayer.Play(AttackPositions.AttackRight.GetDescription());
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
                _animatedSprite.Play(MovePositions.MoveDown.GetDescription());
                break;
            case "attack_up":
                _animatedSprite.Play(MovePositions.MoveUp.GetDescription());
                break;
            case "attack_left":
                _animatedSprite.Play(MovePositions.MoveLeft.GetDescription());
                break;
            case "attack_right":
                _animatedSprite.Play(MovePositions.MoveRight.GetDescription());
                break;
            default:
                break;
            
        }
    }
}

public enum AttackPositions
{
    [Description("attack_up")]
    AttackUp,
    [Description("attack_down")]
    AttackDown,
    [Description("attack_left")]
    AttackLeft,
    [Description("attack_right")]
    AttackRight,
    
}

public enum MovePositions
{
    [Description("move_up")]
    MoveUp,
    [Description("move_down")]
    MoveDown,
    [Description("move_left")]
    MoveLeft,
    [Description("move_right")]
    MoveRight,
    
}
public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        FieldInfo field = value.GetType().GetField(value.ToString());

        if (field != null)
        {
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(
                field, typeof(DescriptionAttribute));

            if (attribute != null)
            {
                return attribute.Description;
            }
        }

        return value.ToString(); // fallback if no description
    }
}
