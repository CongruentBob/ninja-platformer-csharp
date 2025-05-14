using System;
using Godot;

namespace NinjaPlatformer;

public partial class EnemyCannon : Node2D
{
    [Export]
    public Stats Stats { get; set; }
    private Hurtbox _hurtbox;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		_hurtbox = GetNode<Hurtbox>("Hurtbox");
		_hurtbox.HurtboxEntered += OnHurtboxEntered;
		Stats.NoHealth += OnNoHealth;
	}


    private void OnHurtboxEntered(Hitbox area)
    {
		Stats.Health -= area.Damage;
    }

    private void OnNoHealth()
    {
        QueueFree();
    }

    protected override void Dispose(bool disposing)
    {
		_hurtbox.HurtboxEntered -= OnHurtboxEntered;
        base.Dispose(disposing);
    }
}
