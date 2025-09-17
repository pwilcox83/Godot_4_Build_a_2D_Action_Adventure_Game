using System.Linq;
using Godot;
using Godot.Collections;

namespace GlobalAdventure.Scripts;

public partial class Dungeon : Node2D
{
    private Array<Node> _puzzleButtons;
    private LockedDoor _lockedDoor;

    public override void _Ready()
    {
        _puzzleButtons = GetTree().GetNodesInGroup("puzzleButtons");

        var count = 0;
        foreach (var node in _puzzleButtons)
        {
            if (node is PuzzleButton puzzleButton)
            {
                puzzleButton.PuzzleButtonPressed += PuzzleButtonPressStateChanged ;
                puzzleButton.PuzzleButtonUnpressed += PuzzleButtonPressStateChanged;
                count++;
            }

            
        }
        
        _lockedDoor = GetNode<LockedDoor>("LockedDoor");
    }

    private void PuzzleButtonPressStateChanged()
    {
        var index = 0;
        foreach (var node in _puzzleButtons)
        {
            if (node is PuzzleButton puzzleButton)
            {
                var pressed = puzzleButton.Pressed;
                _lockedDoor.ButtonPressedState(index, pressed);
                index++;
            }
            
        }
    } 
}
