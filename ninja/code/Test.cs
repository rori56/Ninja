using Sandbox;
using Sandbox.Citizen;
using System.Net.Http;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Sandbox.UI;
using System.Numerics;



public sealed class Test : Component
{
	// attacher camera
	[Property]
	[Category( "Components" )]
	public GameObject Camera { get; set; }

	// attacher controller
	[Property]
	[Category( "Components" )]
	public CharacterController Controller { get; set; }

	// attacher animatorhelper
	[Property]
	[Category( "Components" )]
	public CitizenAnimationHelper Animator { get; set; }

	[Property]
	public UnitInfo Unit { get; set; }

	[Property]
	public GameObject bullet { get; set; }

		public Transform bulletSpawn;

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
	public Vector3 EyeWorldPosition => Transform.Local.PointToWorld( EyePosition );

	public Angles EyeAngles { get; set; }
	public Angles viseur { get; set; }
	Transform _initialCameraTransform;
	TimeSince _LastPunch;
	TimeSince _LastMissile;

	public int slot;

	bool isRightHand = true;

	public Rotation cameraRotation;

	// cone de la range de l'attaque
	protected override void DrawGizmos()
	{
		if ( !Gizmo.IsSelected ) return;

		var draw = Gizmo.Draw;

		draw.LineSphere( EyePosition, 10f) ;
		draw.LineCylinder( EyePosition, EyePosition + Transform.Rotation.Forward * PunchRange, 5f, 5f, 10 );

	}

	// events qui se refresh
	protected override void OnUpdate()
	{

		EyeAngles += Input.AnalogLook;
		EyeAngles = EyeAngles.WithPitch( MathX.Clamp( EyeAngles.pitch, -80f, 80f ) );
		Transform.Rotation = Rotation.FromYaw( EyeAngles.yaw );

		if ( Camera != null)
			Camera.Transform.Local = _initialCameraTransform.RotateAround( EyePosition, EyeAngles.WithYaw( 0f ) );

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
			Controller.ApplyFriction( 5f);

			if ( Input.Pressed( "Jump" ) )
			{
				Controller.Acceleration = 5f;
				Controller.Punch( Vector3.Up * JumpStrengh );

				if ( Animator != null )
					Animator.TriggerJump();
			}

			if ( Input.Down( "Crouch" ) )
				Animator.DuckLevel = 1;
			else
				Animator.DuckLevel = 0;

			if ( Input.Down( "Slot1" ) )
				slot=1;

			if ( Input.Down( "Slot2" ) )
				slot=2;

		}
		else
		{
			Controller.Velocity += Scene.PhysicsWorld.Gravity * Time.Delta;
		}

		Controller.Move();

		if ( Animator != null )
		{
			Animator.IsGrounded = Controller.IsOnGround;
			Animator.WithVelocity( Controller.Velocity);
		}

		if ( _LastPunch >= 3f )
			Animator.HoldType = CitizenAnimationHelper.HoldTypes.None;

		if (Input.Down( "attack1" ) && _LastPunch >= PunchCooldown && slot == 1)
			Punch();

		if ( Input.Down( "attack1" ) && _LastPunch >= PunchCooldown && slot == 2 )
			Missile();
	}

	
	protected override void OnStart()
	{

		if ( Camera != null )
			_initialCameraTransform = Camera.Transform.Local;

	}

	// Main nue SLOT 1
	public void Punch()
	{
		if ( Animator != null )
		{
			Animator.HoldType = CitizenAnimationHelper.HoldTypes.Punch;
			Animator.Target.Set( "b_attack", true );
		}

		var punchTrace = Scene.Trace
			.FromTo( EyeWorldPosition, EyeWorldPosition + EyeAngles.Forward * PunchRange )
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

	// Missile SLOT 2
	public void Missile()
	{
					
		if ( Unit.Mana >= 20 && Animator != null )
		{

			Unit.Mana -= 20;
			Unit._lastMissile = 0f;

				//lancer droite gauche
				// Utilisez la variable isRightHand pour déterminer la main à utiliser
				Animator.Handedness = isRightHand ? CitizenAnimationHelper.Hand.Right : CitizenAnimationHelper.Hand.Left;
				Animator.HoldType = CitizenAnimationHelper.HoldTypes.HoldItem;
				Animator.Target.Set( "b_attack", true );

				// Inversez l'état de la main pour la prochaine fois
				isRightHand = !isRightHand;

			// Obtenez la rotation de la caméra
			Rotation cameraRotation = Camera.Transform.Rotation;

			// Calcul de la direction dans laquelle le joueur regarde
			Vector3 direction = (Vector3.Forward * cameraRotation);




			Vector3 playerPosition = Transform.Position;
			//playerPosition.x += 20;
			Vector3 offset = direction * 40; // 20 unités devant le joueur
			playerPosition += offset;
			playerPosition.z += 40;

			Log.Info( Unit.Mana );


			bullet.Clone( playerPosition );

		}
/*		//lancer droite gauche
		if ( Animator != null )
		{
			// Utilisez la variable isRightHand pour déterminer la main à utiliser
			Animator.Handedness = isRightHand ? CitizenAnimationHelper.Hand.Right : CitizenAnimationHelper.Hand.Left;
			Animator.HoldType = CitizenAnimationHelper.HoldTypes.HoldItem;
			Animator.Target.Set( "b_attack", true );

			// Inversez l'état de la main pour la prochaine fois
			isRightHand = !isRightHand;
		};*/


		/*		if ( punchTrace.Hit )
				{
					if ( punchTrace.GameObject.Components.TryGet<UnitInfo>( out var unitInfo ) )
						unitInfo.Damage( PunchStrengh );

					if ( punchTrace.GameObject.Components.TryGet<UnitInfoBot>( out var unitInfoBot ) )
						unitInfoBot.Damage( PunchStrengh );
				}*/

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
