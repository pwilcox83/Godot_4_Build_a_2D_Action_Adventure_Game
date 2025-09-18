using System;
using Godot;

namespace GlobalAdventure.Scripts;

public partial class GameScene : Node2D
{
    private Vector2 _playerPosition;
    private DungeonEntrance _dungeonEntrance;
    private Player _player;
    private SceneManager _sceneManager;
    private PuzzleButton _puzzleButton;

    public override void _Ready()
    {
        _player = GetNode<Player>("ObstacleLayer/Player");
        _dungeonEntrance = GetNode<DungeonEntrance>("ObstacleLayer/DungeonEntrance");
        _dungeonEntrance.PlayerEntered += PlayerEnteredDungeon;
        _sceneManager = GetNode<SceneManager>("/root/SceneManager");
        GD.Print("Player position: " + _sceneManager.GetPlayerEnteredDungeonPosition());
        if (_sceneManager.GetPlayerEnteredDungeonPosition() != Vector2.Zero)
        {
            _player.GlobalPosition = _sceneManager.GetPlayerEnteredDungeonPosition();
        }
        _puzzleButton = GetNode<PuzzleButton>("ObstacleLayer/PuzzleButton");
        _puzzleButton.PuzzleButtonPressed += () => OpenDungeonDoor(true);
        _puzzleButton.PuzzleButtonUnpressed += () => OpenDungeonDoor(false);

    }

    private void OpenDungeonDoor(bool doorOpen)
    {
        _dungeonEntrance.OpenDoor(doorOpen);
    }

    private void PlayerEnteredDungeon()
    {
        _sceneManager.SetPlayerEnteredDungeonPosition(_player.GlobalPosition with{ Y = _player.GlobalPosition.Y + 8});
    }
}
