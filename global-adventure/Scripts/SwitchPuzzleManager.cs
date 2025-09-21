using Godot;

namespace GlobalAdventure.Scripts;

public partial class SwitchPuzzleManager : Marker2D
{
    private int _score;
    [Export]
    public int PuzzleScore = 2;
    [Signal]
    public delegate void PuzzleSolvedEventHandler();
    [Signal]
    public delegate void PuzzleFailedEventHandler();

    public void IncreaseScore()
    {
        GD.Print("Increase score");
        _score++;
        GD.Print(_score);
        
        if(_score >= PuzzleScore)
        {
            GD.Print("here 1");
            EmitSignal(SignalName.PuzzleSolved);
        }
    }
    
    public void DecreaseScore()
    {
        GD.Print("Decrease score");
        _score--;
        GD.Print(_score);        
        if (_score < PuzzleScore)
        {
            GD.Print("here 2");
            EmitSignal(SignalName.PuzzleFailed);    
        }
        
    }
}
