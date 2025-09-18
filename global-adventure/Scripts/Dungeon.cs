using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace GlobalAdventure.Scripts;

public partial class Dungeon : Node2D
{
    private List<PuzzleButton> _puzzleButtons;
    private LockedDoor _lockedDoor;

    public override void _Ready()
    {
        _lockedDoor = GetNode<LockedDoor>("LockedDoor");
        var puzzleNodes = GetTree().GetNodesInGroup("puzzleButtons");
        _puzzleButtons =  puzzleNodes.OfType<PuzzleButton>().ToList();

        foreach (var puzzleButton in _puzzleButtons)
        {
            puzzleButton.PuzzleButtonPressed += PuzzleButtonPressStateChanged ;
            puzzleButton.PuzzleButtonUnpressed += PuzzleButtonPressStateChanged;
        }
    }
    
    private void PuzzleButtonPressStateChanged()
    {
        _lockedDoor.OpenDoor(_puzzleButtons.All(x => x.Pressed));
    }
}
