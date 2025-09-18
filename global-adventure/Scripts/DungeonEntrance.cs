using Godot;

namespace GlobalAdventure.Scripts;

public partial class DungeonEntrance : Node2D
{
    [Export]
    public string SceneToLoad;
    
    [Signal]
    public delegate void PlayerEnteredEventHandler();
    
    private Area2D _entranceArea;
    private static SceneManager _sceneManager;
    private LockedDoor _lockedDoor;
    
    public override void _Ready()
    {
        _entranceArea = GetNode<Area2D>("Entrance");
        _entranceArea.BodyEntered += EnteredEventHandler;
        _sceneManager = GetNode<SceneManager>("/root/SceneManager");
        _lockedDoor = GetNode<LockedDoor>("Entrance/LockedDoor");
    }



    private void EnteredEventHandler(Node2D body)
    {
        EmitSignal(SignalName.PlayerEntered);
        CallDeferred(nameof(LoadScene));
    }

    private void LoadScene()
    {
        _sceneManager.LoadScene(SceneToLoad);
    }
    
    public void OpenDoor(bool doorOpen)
    {
        _lockedDoor.OpenDoor(doorOpen);
    }
}
