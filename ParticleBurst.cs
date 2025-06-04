using Godot;

namespace NinjaPlatformer;

[GlobalClass]
public partial class ParticleBurst : GpuParticles2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Finished += QueueFree;
		Emitting = true;
		Explosiveness = 1;
		OneShot = true;
		LocalCoords = true;
	}
}
