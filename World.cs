using Godot;

namespace NinjaPlatformer;

public partial class World : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		RenderingServer.SetDefaultClearColor(new Color("black"));
	}
}
