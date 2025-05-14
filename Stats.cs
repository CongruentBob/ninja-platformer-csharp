using Godot;

namespace NinjaPlatformer;

[GlobalClass]
public partial class Stats : Resource
{
    [Export]
    public float Health
    {
        get => _health;
        set
        {
            _health = value;
            if (_health <= 0)
            {
                EmitSignal(SignalName.NoHealth);
            }
        }
    }

    [Signal]
    public delegate void NoHealthEventHandler();

    private float _health = 10f;
}
