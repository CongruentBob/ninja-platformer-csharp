using System.Diagnostics;
using Godot;

namespace NinjaPlatformer;

[GlobalClass]
public partial class Hitbox : Area2D
{
	[Export]
	public float Damage { get; set; } = 1.0f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
	}

	private void OnAreaEntered(Area2D area2D)
	{
        if (area2D is Hurtbox hurtbox)
        {
			hurtbox.TakeHit(this);
        }
        else 
        {
            Debug.Assert(false, "The hitbox detected an area that wasn't a Hitbox");
        }
	}
}
