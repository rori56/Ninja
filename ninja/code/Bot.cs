using Sandbox;
using Sandbox.Citizen;

public sealed class Bot : Component
{

	// attacher controller
	[Property]
	[Category( "Components" )]
	public CharacterController Controller { get; set; }

	// attacher animatorhelper
	[Property]
	[Category( "Components" )]
	public CitizenAnimationHelper Animator { get; set; }

	// marche
	[Property]
	[Category( "Stats" )]
	public float WalkSpeed { get; set; } = 120f;

	// courir
	[Property]
	[Category( "Stats" )]
	public float RunSpeed { get; set; } = 250f;

	// saut
	[Property]
	[Category( "Stats" )]
	public float JumpStrengh { get; set; } = 400f;

	// attack
	[Property]
	[Category( "Stats" )]
	[Range( 0f, 5f, 0.1f )]
	public float PunchStrengh { get; set; } = 1f;

	// attack CD
	[Property]
	[Category( "Stats" )]
	[Range( 0f, 2f, 0.1f )]
	public float PunchCooldown { get; set; } = 0.5f;

	// range attack
	[Property]
	[Category( "Stats" )]
	[Range( 0f, 200f, 5f )]
	public float PunchRange { get; set; } = 50f;

	// viseur
	[Property]
	public Vector3 EyePosition { get; set; }

	public Angles EyeAngles { get; set; }
	Transform _initialCameraTransform;
	TimeSince _LastPunch;

	// cone de la range de l'attaque
	protected override void DrawGizmos()
	{
		if ( !Gizmo.IsSelected ) return;

		var draw = Gizmo.Draw;

		draw.LineSphere( EyePosition, 10f );
		draw.LineCylinder( EyePosition, EyePosition + Transform.Rotation.Forward * PunchRange, 5f, 50f, 10 );
	}

	// events qui se refresh
	protected override void OnUpdate()
	{
		base.OnUpdate();
	}

	protected override void OnFixedUpdate()
	{
		base.OnFixedUpdate();

		if ( Controller == null ) return;

		var wishSpeed = Input.Down( "Run" ) ? RunSpeed : WalkSpeed;
		var wishVelocity = Input.AnalogMove.Normal * wishSpeed * Transform.Rotation;

		Controller.Accelerate( wishVelocity );

		if ( Controller.IsOnGround )
		{
			Controller.Acceleration = 10f;
			Controller.ApplyFriction( 5f );

			if ( Input.Pressed( "Jump" ) )
			{
				Controller.Acceleration = 5f;
				Controller.Punch( Vector3.Up * JumpStrengh );

				if ( Animator != null )
					Animator.TriggerJump();
			}

		}
		else
		{
			Controller.Velocity += Scene.PhysicsWorld.Gravity * Time.Delta;
		}

		Controller.Move();

		if ( Animator != null )
		{
			Animator.IsGrounded = Controller.IsOnGround;
			Animator.WithVelocity( Controller.Velocity );
		}

		if ( _LastPunch >= 7f )
			Animator.HoldType = CitizenAnimationHelper.HoldTypes.None;

		if ( Input.Pressed( "Punch" ) && _LastPunch >= PunchCooldown )
			Punch();
	}

	protected override void OnStart()
	{
		base.OnStart();
	}

	public void Punch()
	{
		if ( Animator != null )
		{
			Animator.HoldType = CitizenAnimationHelper.HoldTypes.Punch;
			Animator.Target.Set( "b_attack", true );
		}

		var punchTrace = Scene.Trace
			.FromTo( EyePosition, EyePosition + EyeAngles.Forward * PunchRange )
			.Size( 10f )
			.WithoutTags( "player" )
			.IgnoreGameObjectHierarchy( GameObject )
			.Run();

		if ( punchTrace.Hit )
		{ 
			if ( punchTrace.GameObject.Components.TryGet<UnitInfo>( out var unitInfo ) )
				unitInfo.Damage( PunchStrengh );

			if ( punchTrace.GameObject.Components.TryGet<UnitInfoBot>( out var unitInfoBot ) )
				unitInfoBot.Damage( PunchStrengh );
		}

		_LastPunch = 0f;
	}
	protected override void OnEnabled()
	{
		base.OnEnabled();
	}

	protected override void OnDisabled()
	{
		base.OnDisabled();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}
}
