using System.Collections.Generic;
using Godot;

namespace GlobalAdventure.Scripts;

public partial class SceneManager : Node
{
    private readonly Dictionary<string, string> _paths = new()
    {
        { "World", "res://Scenes/GameScenes/game_scene.tscn" },
        { "Dungeon", "res://Scenes/GameScenes/dungeon.tscn" },
    };

    private Vector2 _playerEnteredDungeonPosition = Vector2.Zero;

    public Error LoadScene(string key)
    {
        if (_paths.TryGetValue(key, out var path) && !string.IsNullOrEmpty(path))
        {
            return GetTree().ChangeSceneToFile(path);
        }
        
        return Error.Failed;
    }

    public void SetPlayerEnteredDungeonPosition(Vector2 playerPosition)
    {
        _playerEnteredDungeonPosition = playerPosition;
    }
    
    public Vector2 GetPlayerEnteredDungeonPosition()
    {
        return _playerEnteredDungeonPosition;
    }
}
