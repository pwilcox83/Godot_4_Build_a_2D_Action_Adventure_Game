using Godot;

namespace GlobalAdventure.Scripts;

public partial class DungeonEntrance : Node2D
{
    [Signal]
    public delegate void PlayerEnteredEventHandler();

    private static SceneManager _sceneManager;

    private Area2D _entranceArea;
    private LockedDoor _lockedDoor;

    [Export] 
    public string SceneToLoad;

    [Export] 
    public bool IsOpen;

    public override void _Ready()
    {
        _entranceArea = GetNode<Area2D>("Entrance");
        _entranceArea.BodyEntered += EnteredEventHandler;
        _sceneManager = GetNode<SceneManager>("/root/SceneManager");
        _lockedDoor = GetNode<LockedDoor>("Entrance/LockedDoor");
        if (IsOpen)
        {
            OpenDoor(true);
        }
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
