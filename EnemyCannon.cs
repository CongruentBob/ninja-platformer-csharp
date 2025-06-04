using System;
using Godot;

namespace NinjaPlatformer;

public partial class EnemyCannon : Node2D
{
    [Export]
    public Stats Stats { get; set; }
    private Hurtbox _hurtbox;
    private AnimationPlayer _effectsAnimationPlayer;
    private Sprite2D _sprite2D;
    private Shaker _shaker;
    private PackedScene _sparksPackedScene = ResourceLoader.Load<PackedScene>("res://sparks_particle_burst_effect.tscn");
    private PackedScene _impactPackedScene = ResourceLoader.Load<PackedScene>("res://impact_particle_burst_effect.tscn");

    // Called when the node enters the scene tree for the first time.

    public override void _Ready()
    {
        _hurtbox = GetNode<Hurtbox>("Hurtbox");
        _hurtbox.HurtboxEntered += OnHurtboxEntered;
        _effectsAnimationPlayer = GetNode<AnimationPlayer>("EffectsAnimationPlayer");
        Stats.NoHealth += OnNoHealth;
        _sprite2D = GetNode<Sprite2D>("Sprite2D");
        _shaker = new Shaker(_sprite2D);
	}

    private void OnHurtboxEntered(Hitbox hitbox)
    {
        var sparks = _sparksPackedScene.Instantiate<ParticleBurst>();
        GetTree().CurrentScene.AddChild(sparks);
        sparks.GlobalPosition = _sprite2D.GlobalPosition;

        var impact = _impactPackedScene.Instantiate<ParticleBurst>();
        GetTree().CurrentScene.AddChild(impact);
        impact.GlobalPosition = _sprite2D.GlobalPosition.MoveToward(hitbox.GlobalPosition, -8);
        impact.Rotation = _sprite2D.GlobalPosition.DirectionTo(hitbox.GlobalPosition).Angle();
        
        Stats.Health -= hitbox.Damage;
        _effectsAnimationPlayer.Play("hitflash");
        _shaker.Shake(2, .2f);
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
