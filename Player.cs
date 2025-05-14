using System;
using Godot;

namespace NinjaPlatformer;

public partial class Player : CharacterBody2D
{
	[Export]
	public float MaxSpeed { get; set; } = 120;
	[Export]
	public float Acceleration { get; set; } = 10000;
	[Export]
	public float AirAcceleration { get; set; } = 2000;
	[Export]
	public float Friction { get; set; } = 10000;
	[Export]
	public float AirFriction { get; set; } = 500;
	[Export]
	public float UpGravity { get; set; } = 500;
	[Export]
	public float DownGravity { get; set; } = 600;
	[Export]
	public float JumpAmount { get; set; } = 200;

	private CharacterState _state = CharacterState.MOVE;
	private float _coyoteTime;
	private Node2D _anchor;
	private AnimationPlayer _animationPlayerLower;
	private AnimationPlayer _animationPlayerUpper;
	private RayCast2D _rayCastLower;
	private RayCast2D _rayCastUpper;
    private Hurtbox _hurtbox;

    public override void _Ready()
	{
		base._Ready();

		_anchor = GetNode<Node2D>("Anchor");
		_animationPlayerLower = GetNode<AnimationPlayer>("AnimationPlayerLower");
		_animationPlayerLower.CurrentAnimationChanged += OnCurrentLowerAnimationChanged;
		_animationPlayerUpper = GetNode<AnimationPlayer>("AnimationPlayerUpper");
		_rayCastLower = GetNode<RayCast2D>("Anchor/RayCastLower");
		_rayCastUpper = GetNode<RayCast2D>("Anchor/RayCastUpper");
		_hurtbox = GetNode<Hurtbox>("Anchor/Hurtbox");
		_hurtbox.HurtboxEntered += OnHurtboxEntered;
	}

    public override void _PhysicsProcess(double delta)
	{
		var deltaF = (float)delta;
		switch (_state)
		{
			case CharacterState.MOVE:
				_coyoteTime -= deltaF;

				var xInput = Input.GetAxis("move_left", "move_right");

				if (xInput != 0)
				{
					AccelerateHorizontally(xInput, deltaF);
				}
				else
				{
					ApplyFriction(deltaF);
				}

				ApplyGravity(deltaF);

				if ((IsOnFloor() || _coyoteTime > 0) && Input.IsActionJustPressed("jump"))
				{
					Jump();
				}

				if (Input.IsActionJustPressed("attack"))
				{
					// _animationPlayerLower.Play("attack");
					_animationPlayerUpper.Play("attack");
				}

				if (xInput != 0)
				{
					// run
					_anchor.Scale = _anchor.Scale with { X = Math.Sign(xInput) };
					_animationPlayerLower.Play("run");
					// _animationPlayerUpper.Play("run");
				}
				else
				{
					// idle/stand
					_animationPlayerLower.Play("idle");
					// _animationPlayerUpper.Play("idle");
				}

				if (!IsOnFloor())
				{
					// airbourne
					_animationPlayerLower.Play("jump");
					// _animationPlayerUpper.Play("jump");
				}

				var wasOnFloor = IsOnFloor();
				MoveAndSlide();

				if (wasOnFloor && !IsOnFloor() && Velocity.Y >= 0)
				{
					_coyoteTime = 0.1f;
				}

				if (ShouldWallClimb())
				{
					_animationPlayerUpper.Play("hang");
					_state = CharacterState.CLIMB;
				}

				break;
			case CharacterState.CLIMB:
				var wallNormal = GetWallNormal();
				var input = Input.GetVector("move_left", "move_right", "move_up", "move_down");

				Velocity = Velocity with { Y = input.Y * MaxSpeed * 0.8f };

				if (input.Y != 0)
				{
					_animationPlayerLower.Play("climb");
				}
				else
				{
					_animationPlayerLower.Play("hang");
				}

				var requestDetach = Math.Sign(input.X) == wallNormal.X;
				var requestWallJump = (requestDetach || Input.IsActionJustPressed("jump")) && !Input.IsActionPressed("move_down");

				if (!ShouldWallClimb() || requestDetach)
				{
					if (Input.IsActionPressed("move_up"))
					{
						Jump();
					}

					_state = CharacterState.MOVE;
				}

				if (requestWallJump)
				{
					Velocity = Velocity with { X = wallNormal.X * MaxSpeed };
					_anchor.Scale = _anchor.Scale with { X = Math.Sign(wallNormal.X) };
					Jump();
					_state = CharacterState.MOVE;
				}

				MoveAndSlide();

				break;
		}
	}

	private void AccelerateHorizontally(float horizontalDirection, float delta)
	{
		var accelerationAmount = IsOnFloor() ? AirAcceleration : Acceleration;

		Velocity = Velocity.MoveToward(Velocity with { X = MaxSpeed * horizontalDirection }, accelerationAmount * delta); // * Math.Abs(horizontalDirection)
	}

	private void ApplyFriction(float delta)
	{
		var frictionAmount = IsOnFloor() ? AirFriction : Friction;

		Velocity = Velocity.MoveToward(Velocity with { X = 0 }, frictionAmount * delta);
	}

	private void ApplyGravity(float delta)
	{
		if (!IsOnFloor())
		{
			if (Velocity.Y <= 0)
			{
				Velocity += UpGravity * delta * Vector2.Down;
			}
			else
			{
				Velocity += DownGravity * delta * Vector2.Down;
			}
		}
	}

	private void Jump()
	{
		// jump
		Velocity = Velocity with { Y = -JumpAmount };
	}

	private bool ShouldWallClimb()
	{
		return _rayCastUpper.IsColliding()
			&& _rayCastLower.IsColliding()
			&& !IsOnFloor();
	}

    private void OnHurtboxEntered(Hitbox area)
    {
		Console.WriteLine("Damn!");
    }

	private void OnCurrentLowerAnimationChanged(string animationName)
	{
		// sync upper animation with lower
		if (_animationPlayerUpper.CurrentAnimation == "attack") return;

		_animationPlayerUpper.Play(animationName);
	}

	private void OnLowerAnimationFinished(string animationName)
	{
		if (animationName == "attack")
		{
			_animationPlayerUpper.Play(_animationPlayerLower.CurrentAnimation);
			// play from the same frame
			_animationPlayerUpper.Seek(_animationPlayerLower.CurrentAnimationPosition);
		}
	}

	protected override void Dispose(bool disposing)
	{
		_animationPlayerLower.CurrentAnimationChanged -= OnCurrentLowerAnimationChanged;

		base.Dispose(disposing);
	}
}
