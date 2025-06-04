using System.Threading.Tasks;
using Godot;

namespace NinjaPlatformer;

public partial class Shaker(Node2D target) : RefCounted
{
    private Node2D _target = target;
    private int _shakes = 4;

    public async Task Shake(float magnitude, float duration)
    {
        var startingPosition = _target.Position;
        var currentMagnitude = magnitude;
        for (var i = 0; i < _shakes; i++)
        {
            _target.Position = startingPosition + new Vector2((float)GD.RandRange(-magnitude, magnitude), (float)GD.RandRange(-magnitude, magnitude));
            await ToSignal(_target.GetTree().CreateTimer(duration / _shakes), SceneTreeTimer.SignalName.Timeout);
            currentMagnitude -= currentMagnitude / _shakes;
        }

        _target.Position = startingPosition;
    }
}
