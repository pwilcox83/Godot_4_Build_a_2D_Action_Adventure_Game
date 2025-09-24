using System.Collections.Generic;

namespace GlobalAdventure.Scripts;

public static class CommonAnimationDictionary
{
    private static Dictionary<(Action,Direction), string> _animations = new Dictionary<(Action, Direction), string>
    {
        {(Action.Idle, Direction.Up), "idle_up" },
        {(Action.Idle, Direction.Down), "idle_down" },
        {(Action.Idle, Direction.Left), "idle_left" },
        {(Action.Idle, Direction.Right), "idle_right" },
        {(Action.Move, Direction.Up), "move_up" },
        {(Action.Move, Direction.Down), "move_down" },
        {(Action.Move, Direction.Left), "move_left" },
        {(Action.Move, Direction.Right), "move_right" },
        {(Action.Attack, Direction.Up), "attack_up" },
        {(Action.Attack, Direction.Down), "attack_down" },
        {(Action.Attack, Direction.Left), "attack_left" },
        {(Action.Attack, Direction.Right), "attack_right" },
        {(Action.Death, Direction.NotRequired), "death" },
    };
    
    public static string Get(Action action, Direction direction)
    {
        return _animations.ContainsKey((action, direction)) ? 
           _animations[(action, direction)]:
           string.Empty;
    }
}
