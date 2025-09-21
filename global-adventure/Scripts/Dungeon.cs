using System.Collections.Generic;
using System.Linq;
using Godot;

namespace GlobalAdventure.Scripts;

public partial class Dungeon : Node2D
{
    private LockedDoor _lockedDoor;
    private List<PuzzleButton> _puzzleButtons;
    private List<Switch> _puzzleSwitches;
    private SecretWallLayer _secretWallLayer;
    private Switch _switch;
    private SwitchPuzzleManager _switchPuzzleManager;

    public override void _Ready()
    {
        _lockedDoor = GetNode<LockedDoor>("LockedDoor");
        var puzzleNodes = GetTree().GetNodesInGroup("puzzleButtons");
        _puzzleButtons = puzzleNodes.OfType<PuzzleButton>().ToList();
        foreach (var puzzleButton in _puzzleButtons)
        {
            puzzleButton.PuzzleButtonPressed += PuzzleButtonPressStateChanged;
            puzzleButton.PuzzleButtonUnpressed += PuzzleButtonPressStateChanged;
        }
        _secretWallLayer = GetNode<SecretWallLayer>("SecretWallLayer");
        _switchPuzzleManager = GetNode<SwitchPuzzleManager>("SwitchPuzzleManager");
        _switchPuzzleManager.PuzzleSolved += () => SecretWallOpen(true);
        _switchPuzzleManager.PuzzleFailed += () => SecretWallOpen(false);
        
        _puzzleSwitches = GetNode("SwitchPuzzleManager").GetChildren().OfType<Switch>().ToList();
        for (var i = 0; i < _puzzleSwitches.Count; i++)
        {
            if (i is 0 or 2)
            {
                _puzzleSwitches[i].SwitchedOn += _switchPuzzleManager.IncreaseScore;
                _puzzleSwitches[i].SwitchedOff += _switchPuzzleManager.DecreaseScore;
            }
            else
            {
                _puzzleSwitches[i].SwitchedOn += _switchPuzzleManager.DecreaseScore;
                _puzzleSwitches[i].SwitchedOff += _switchPuzzleManager.IncreaseScore;
            }
        }
    }

    private void SecretWallOpen(bool open)
    {
        GD.Print($"Secret wall {(open ? "opened" : "closed")}");
        
        _secretWallLayer.SetInvisibleWall(open);
    }

    private void PuzzleButtonPressStateChanged()
    {
        _lockedDoor.OpenDoor(_puzzleButtons.All(x => x.Pressed));
    }
}
