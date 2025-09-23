using Godot;
using Godot.Collections;

namespace GlobalAdventure.Scripts;

public partial class Npc : StaticBody2D
{
    private CanvasLayer _canvasLayer;
    private Label _dialogueLabel;
    private AudioStreamPlayer2D _npcSound;
    private int _dialogueLineIndex;

    [Export] public Array<string> DialogueLines = new()
    {
        "Hello, I am a test npc.",
        "I am here to test the dialogue system.",
        "I hope you enjoy my game.",
        "Goodbye."
    };

    public bool IsInteractable;

    public override void _Ready()
    {
        _canvasLayer = GetNode<CanvasLayer>("CanvasLayer");
        _dialogueLabel = GetNode<Label>("CanvasLayer/NpcTextLabel");
        _npcSound = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
    }

    public override void _Process(double delta)
    {
        if (!IsInteractable || !Input.IsActionJustPressed("interact")) return;
        
        _canvasLayer.Visible = true;
        GetTree().Paused = true;
        if (_dialogueLineIndex >= 0 && _dialogueLineIndex < DialogueLines.Count)
        {
            _npcSound.Play();
            _dialogueLabel.Text = DialogueLines[_dialogueLineIndex];
            _dialogueLineIndex++;
        }
        else
        {
            _dialogueLineIndex = 0;
            _canvasLayer.Visible = false;
            GetTree().Paused = false;
        }
    }
}
