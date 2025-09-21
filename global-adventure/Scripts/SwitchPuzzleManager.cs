using Godot;

namespace GlobalAdventure.Scripts;

public partial class SwitchPuzzleManager : Marker2D
{
    private int _score;
    [Export]
    public int PuzzleScore;
    [Signal]
    public delegate void PuzzleSolvedEventHandler();
    [Signal]
    public delegate void PuzzleFailedEventHandler();

    public void IncreaseScore()
    {
        _score++;
        
        if(_score >= PuzzleScore)
        {
            EmitSignal(SignalName.PuzzleSolved);
        }
    }
    
    public void DecreaseScore()
    {
        _score--;       
        if (_score < PuzzleScore)
        {
            EmitSignal(SignalName.PuzzleFailed);    
        }
    }
}
