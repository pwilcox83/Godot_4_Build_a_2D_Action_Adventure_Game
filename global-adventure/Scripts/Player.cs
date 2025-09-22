using Godot;

namespace GlobalAdventure.Scripts;

public partial class Player : CharacterBody2D
{
    private AnimatedSprite2D _animatedSprite;
    private float _damage = 7.5f;
    private string _firstName = "Paul";
    private Area2D _InteractionArea;
    private bool _isPlayerAlive;
    private float _money = 15.5f;
    private int _score = 200;
    private string _userName = "Bob";
    private int _scrollCount;
    private Label _scrollCountLabel;
    private SceneManager _sceneManager;

    [Export] public int Health = 100;

    [Export] public int PushStrength = 200;

    [Export] public int Speed = 100;

    public override void _Ready()
    {
        _sceneManager =  GetNode<SceneManager>("/root/SceneManager");
        _animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _InteractionArea = GetNode<Area2D>("InteractionArea");
        _InteractionArea.BodyEntered += InteractionAreaEntered;
        _InteractionArea.BodyExited += InteractionAreaExited;
        _scrollCountLabel = GetNode<Label>("%TreasureLabel");
        _scrollCountLabel.Text = _sceneManager.OpenedChests.Count.ToString();
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
        var moveVector = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        Velocity = moveVector * Speed;
        if (Velocity.X > 0)
        {
            _animatedSprite.Play("move_right");
            _InteractionArea.Position = new Vector2(5, 2);
        }
        else if (Velocity.X < 0)
        {
            _animatedSprite.Play("move_left");
            _InteractionArea.Position = new Vector2(-5, 2);
        }
        else if (Velocity.Y > 0)
        {
            _animatedSprite.Play("move_down");
            _InteractionArea.Position = new Vector2(0, 8);
        }
        else if (Velocity.Y < 0)
        {
            _animatedSprite.Play("move_up");
            _InteractionArea.Position = new Vector2(0, -4);
        }
        else
        {
            _animatedSprite.Stop();
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
}
