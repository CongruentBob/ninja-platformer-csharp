using System;
using Godot;

namespace NinjaPlatformer;

public partial class EnemyCannon : Node2D
{
    [Export]
    public Stats Stats { get; set; }
    private Hurtbox _hurtbox;
    private AnimationPlayer _effectsAnimationPlayer;


    // Called when the node enters the scene tree for the first time.

    public override void _Ready()
	{
		_hurtbox = GetNode<Hurtbox>("Hurtbox");
		_hurtbox.HurtboxEntered += OnHurtboxEntered;
		_effectsAnimationPlayer = GetNode<AnimationPlayer>("EffectsAnimationPlayer");
		Stats.NoHealth += OnNoHealth;
	}


    private void OnHurtboxEntered(Hitbox area)
    {
		Stats.Health -= area.Damage;
		_effectsAnimationPlayer.Play("hitflash");
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
