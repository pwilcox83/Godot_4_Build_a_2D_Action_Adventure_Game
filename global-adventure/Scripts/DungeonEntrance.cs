using Godot;

namespace GlobalAdventure.Scripts;

public partial class DungeonEntrance : Node2D
{
    [Export]
    public PackedScene SceneToLoad;
    private Area2D _entranceArea;
    public override void _Ready()
    {
        _entranceArea = GetNode<Area2D>("Entrance");
        _entranceArea.BodyEntered += EnteredEventHandler;
        _entranceArea.BodyExited += ExitedEventHandler;
        GD.Print("READY");
        GD.Print(SceneToLoad);
        
    }

    private void ExitedEventHandler(Node2D body)
    {
        GD.Print("exited");
    }

    private void EnteredEventHandler(Node2D body)
    {
        CallDeferred(nameof(LoadScene),SceneToLoad);
    }

    private void LoadScene(PackedScene packedScene)
    {
        GetTree().ChangeSceneToPacked(packedScene);
    }
}
