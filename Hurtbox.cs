using Godot;

namespace NinjaPlatformer;

[GlobalClass]
public partial class Hurtbox : Area2D
{
    [Signal]
    public delegate void HurtboxEnteredEventHandler(Hitbox area);
    [Export]
	public bool IsInvincible
	{
		get => _isInvincible;
		set
		{
			_isInvincible = value;
			foreach (var child in GetChildren())
			{
				if (child is not CollisionShape2D
					&& child is not CollisionPolygon2D) continue;
				child.SetDeferred(CollisionShape2D.PropertyName.Disabled, _isInvincible);
			}
		}
	}

	private bool _isInvincible;

    public void TakeHit(Hitbox hitbox)
    {
        if (IsInvincible) return;
        EmitSignal(SignalName.HurtboxEntered, hitbox);
    }
}
