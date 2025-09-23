using System.Linq;
using Godot;

namespace GlobalAdventure.Scripts;

public partial class TreasureChest : StaticBody2D
{
    [Export]
    public string TreasureName;
    public bool IsInteractable { get; set; }
    public bool IsOpen { get; private set; }
    [Signal]
    public delegate void SwitchedOffEventHandler();
    [Signal]
    public delegate void SwitchedOnEventHandler();

    private AnimatedSprite2D _animatedSprite2D;
    private Sprite2D _sprite2D;
    private Timer _timer;
    private SceneManager _sceneManager;
    private AudioStreamPlayer2D _chestSound;

    public override void _Ready()
    {
        _animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _sprite2D = GetNode<Sprite2D>("Sprite2D");
        _timer = GetNode<Timer>("Timer");
        _timer.Timeout += () => ShowTreasure(false);
        
        _sceneManager = GetNode<SceneManager>("/root/SceneManager");
        if (_sceneManager.OpenedChests.Contains(TreasureName))
        {
            OpenChest();
        }
        
        _chestSound = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
        
    }

    private void ShowTreasure(bool showTreasure)
    {
        if (showTreasure)
        {
            _chestSound.Play();    
        }
        _sprite2D.Visible = showTreasure;
    }

    public override void _Process(double delta)
    {
        if (!Input.IsActionJustPressed("interact") || !IsInteractable || IsOpen) return;
        OpenChest();
    }

    private void OpenChest()
    {
        IsOpen = true;
        _animatedSprite2D.Play("opened");
        IsInteractable = false;
        if (_sceneManager.OpenedChests.Contains(TreasureName)) return;  
        ShowTreasure(true);
        _timer.Start();
        _sceneManager.OpenedChests.Add(TreasureName);
    }
}
