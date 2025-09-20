using System.Collections.Generic;
using System.Linq;
using Godot;

namespace GlobalAdventure.Scripts;

public partial class Dungeon : Node2D
{
    private LockedDoor _lockedDoor;
    private List<PuzzleButton> _puzzleButtons;
    private SecretWallLayer _secretWallLayer;
    private Switch _switch;

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
        _switch = GetNode<Switch>("Switch");
        _switch.SwitchedOn += () => SecretWallOpen(true);
        _switch.SwitchedOff += () => SecretWallOpen(false);
    }

    private void SecretWallOpen(bool open)
    {
        _secretWallLayer.SetInvisibleWall(open);
    }

    private void PuzzleButtonPressStateChanged()
    {
        _lockedDoor.OpenDoor(_puzzleButtons.All(x => x.Pressed));
    }
}
