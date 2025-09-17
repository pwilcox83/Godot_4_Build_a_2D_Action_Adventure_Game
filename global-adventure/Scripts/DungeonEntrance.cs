using Godot;

namespace GlobalAdventure.Scripts;

public partial class DungeonEntrance : Node2D
{
    [Export]
    public string SceneToLoad;
    
    [Export]
    public int NumberOfButtonsRequired;
    
    private int _numberOfButtonsPressed;
    private Area2D _entranceArea;
    private static SceneManager _sceneManager;
    private LockedDoor _lockedDoor;
    [Signal]
    public delegate void PlayerEnteredEventHandler();
        
    public override void _Ready()
    {
        _entranceArea = GetNode<Area2D>("Entrance");
        _entranceArea.BodyEntered += EnteredEventHandler;
        _sceneManager = GetNode<SceneManager>("/root/SceneManager");
        _lockedDoor = GetNode<LockedDoor>("Entrance/LockedDoor");
        _lockedDoor.SetNumberOfButtonsRequired(NumberOfButtonsRequired);
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

    private void SetDoorOpen(bool doorOpen)
    {
       _lockedDoor.ButtonPressedState(0,doorOpen);
    }

    public void PuzzleDoorButtonPressed()
    {
        GD.Print("Button pressed : {" + _numberOfButtonsPressed + "}");
        _numberOfButtonsPressed++;
        SetDoorOpen(true);
    }
    
    public void PuzzleDoorButtonUnpressed()
    {
        GD.Print("Button pressed : {" + _numberOfButtonsPressed + "}");
        _numberOfButtonsPressed = 0;
        SetDoorOpen(false);
    }

    public void OpenDoor(bool b)
    {
        _lockedDoor.ButtonPressedState(0,true);
    }
}
