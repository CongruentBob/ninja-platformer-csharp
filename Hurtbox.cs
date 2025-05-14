using Godot;

namespace NinjaPlatformer;

[GlobalClass]
public partial class Hurtbox : Area2D
{
    [Signal]
    public delegate void HurtboxEnteredEventHandler(Hitbox area);
	private bool _isInvincible;

    public void TakeHit(Hitbox hitbox)
    {
        if (_isInvincible) return;
        EmitSignal(SignalName.HurtboxEntered, hitbox);
    }
}
