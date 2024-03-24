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
using System.Net.Security;
using System.Reflection.Metadata;



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
	public SkinnedModelRenderer model { get; set; }

	[Property]
	public UnitInfo Unit { get; set; }

	[Property]
	public GameObject bullet { get; set; }

	[Property]
	public Inventory inventaire { get; set; }

	public Transform bulletSpawn;

	// marche
	[Property]
	[Category( "Stats" )]
	public float WalkSpeed { get; set; } = 300f;

	// courir
	[Property]
	[Category( "Stats" )]
	public float RunSpeed { get; set; } = 450f;

	// saut
	[Property]
	[Category( "Stats" )]
	public float JumpStrengh { get; set; } = 550f;

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

	// range attack
	[Property]
	[Category( "Stats" )]
	public float missileMana  = 5f;

	// viseur
	[Property]
	public Vector3 EyePosition { get; set; }
	public Vector3 EyeWorldPosition => Transform.Local.PointToWorld( EyePosition );

	public Angles EyeAngles { get; set; }
	public Angles viseur { get; set; }
	Transform _initialCameraTransform;
	TimeSince _LastPunch;
	TimeSince _LastMissile;

	bool isRightHand = true;

	public Rotation cameraRotation => Camera.Transform.Rotation;

	public Vector3 cameraForward => cameraRotation.Forward;

	[Property]
	public GameObject missileEffect { get; set; }

	private bool missileEffectClonedR = false;

	private bool missileEffectClonedL = false;

	public bool speedToggle = false;


	// cone de la range de l'attaque
	protected override void DrawGizmos()
	{
		if ( !Gizmo.IsSelected ) return;

		var draw = Gizmo.Draw;

		draw.LineSphere( EyePosition, 10f );
		draw.LineCylinder( EyePosition, EyePosition + Transform.Rotation.Forward * PunchRange, 5f, 5f, 10 );

	}

	// events qui se refresh
	protected override void OnUpdate()
	{

		EyeAngles += Input.AnalogLook;
		EyeAngles = EyeAngles.WithPitch( MathX.Clamp( EyeAngles.pitch, -80f, 80f ) );
		Transform.Rotation = Rotation.FromYaw( EyeAngles.yaw );

		if ( Camera != null )
			Camera.Transform.Local = _initialCameraTransform.RotateAround( EyePosition, EyeAngles.WithYaw( 0f ) );

		//var handTransformR = model.GetAttachment( "hand_R" ).Value;
		//var handTransformL = model.GetAttachment( "hand_L" ).Value;

		/*		if ( !missileEffectClonedR )
				{
					missileEffect.Clone();
					missileEffect.Transform.Position = handTransformR.Position;
					missileEffectClonedR = true;
				}
				if ( !missileEffectClonedL )
				{
					missileEffect.Clone();
					missileEffect.Transform.Position = handTransformL.Position;
					missileEffectClonedL = true;
				}*/

		//var handTransformR = model.GetAttachment( "hand_R" ).Value;
		//missileEffect.Clone(handTransformR.Position);

		//var handTransformL = model.GetAttachment( "hand_L" ).Value;
		//missileEffect.Transform.Position = handTransformR.Position;
		//missileEffect.Transform.Position = handTransformL.Position;
	}

	protected override void OnFixedUpdate()
	{
		if ( Controller == null ) return;

		if ( speedToggle == true )
		{
			if ( Controller == null ) return;

			var wishSpeedT = Input.Down( "Run" ) ? RunSpeed : WalkSpeed;
			//wishSpeedT = wishSpeedT * 1.15f;
			var wishVelocityT = (Input.AnalogMove.Normal * wishSpeedT * Transform.Rotation);

			Controller.Accelerate( wishVelocityT * 1.75f );

			if ( Controller.IsOnGround )
			{
				Controller.Acceleration = 10f;
				Controller.ApplyFriction( 5f );

				if ( Input.Pressed( "Jump" ) )
				{
					Controller.Acceleration = 2f;
					Controller.Punch( Vector3.Up * JumpStrengh );


					if ( Animator != null )
						Animator.TriggerJump();
				}

				if ( Input.Down( "Crouch" ) )
				{
					Animator.DuckLevel = 1;
					Controller.Acceleration = 3f;
				}
				else
					Animator.DuckLevel = 0;

			}
			else
			{
				Controller.Velocity += Scene.PhysicsWorld.Gravity * Time.Delta;
				Controller.ApplyFriction( 0.5f );
				Controller.Accelerate( Vector3.Down * 3f );
			}

			Controller.Move();

			if ( Animator != null )
			{
				Animator.IsGrounded = Controller.IsOnGround;
				Animator.WithVelocity( Controller.Velocity );
			}
		}

		if ( speedToggle == false )
		{
			var wishSpeed = Input.Down( "Run" ) ? RunSpeed : WalkSpeed;
			var wishVelocity = Input.AnalogMove.Normal * wishSpeed * Transform.Rotation;

			Controller.Accelerate( wishVelocity );

			if ( Controller.IsOnGround )
			{
				Controller.Acceleration = 10f;
				Controller.ApplyFriction( 5f );

				if ( Input.Pressed( "Jump" ) )
				{
					Controller.Acceleration = 2f;
					Controller.Punch( Vector3.Up * JumpStrengh );


					if ( Animator != null )
						Animator.TriggerJump();
				}

				/*				if ( Input.Down( "Crouch" ) )
								{
									Animator.DuckLevel = 1;
									Controller.Acceleration = 3f;
								}
								else
									Animator.DuckLevel = 0;*/

				if ( Input.Pressed( "Crouch" ) && Input.Down( "Forward" ) )
				{
					Rotation playerRot = Transform.Rotation;
					Vector3 playerForward = playerRot.Forward;
					Controller.Punch( playerForward * 1200 );
					Controller.ApplyFriction( 20f );
				}

				if ( Input.Pressed( "Crouch" ) && Input.Down( "Backward" ) )
				{
					Rotation playerRot = Transform.Rotation;
					Vector3 playerBackward = playerRot.Backward;
					Controller.Punch( playerBackward * 1200 );
					Controller.ApplyFriction( 13f );
				}

				if ( Input.Pressed( "Crouch" ) && Input.Down( "Left" ) )
				{
					Rotation playerRot = Transform.Rotation;
					Vector3 playerLeft = playerRot.Left;
					Controller.Punch( playerLeft * 1200 );
					Controller.ApplyFriction( 20f );
				}

				if ( Input.Pressed( "Crouch" ) && Input.Down( "Right" ) )
				{
					Rotation playerRot = Transform.Rotation;
					Vector3 playerRight = playerRot.Right;
					Controller.Punch( playerRight * 1200 );
					Controller.ApplyFriction( 20f );
				}

			}
			else
			{
				Controller.Velocity += Scene.PhysicsWorld.Gravity * Time.Delta;
				Controller.ApplyFriction( 0.5f );
				Controller.Accelerate( Vector3.Down * 3f );
			}

			Controller.Move();

			if ( Animator != null )
			{
				Animator.IsGrounded = Controller.IsOnGround;
				Animator.WithVelocity( Controller.Velocity );
			}
		}


		if ( _LastPunch >= 3f )
			Animator.HoldType = CitizenAnimationHelper.HoldTypes.None;

		if ( Input.Down( "attack1" ) && _LastPunch >= PunchCooldown && inventaire.slot == 1 )
			Punch();

		if ( Input.Down( "attack1" ) && _LastPunch >= PunchCooldown && inventaire.slot == 2 )
			Missile();

		Rotation cameraRotation = Camera.Transform.Rotation;
		Vector3 playerPosition = Transform.Position;
		playerPosition.z += 70;
		Vector3 cameraDirection = Vector3.Backward * cameraRotation * 150f;
		Vector3 cameraCol = playerPosition + cameraDirection;


		var cameraTrace = Scene.Trace
			.FromTo( playerPosition, cameraCol )
			.Size( 15f )
			.IgnoreGameObjectHierarchy( GameObject )
			.Run();

		if ( cameraTrace.Hit )
		{
			Vector3 newCamera = cameraTrace.EndPosition;

			// Calculer un vecteur de décalage relatif en fonction de la direction de la caméra
			Vector3 cameraRight = Vector3.Cross( Vector3.Up, cameraDirection );
			float offsetAmount = 0.2f; // Ajustez ce coefficient selon la quantité de décalage souhaitée

			// Ajouter un décalage relatif vers la droite ou la gauche
			newCamera += cameraRight * offsetAmount;

			Camera.Transform.Position = newCamera;
		}
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

			Unit.Mana -= missileMana;
			Unit._lastMissile = 0f;

			//lancer droite gauche
			Animator.Handedness = isRightHand ? CitizenAnimationHelper.Hand.Right : CitizenAnimationHelper.Hand.Left;
			Animator.HoldType = CitizenAnimationHelper.HoldTypes.HoldItem;
			Animator.Target.Set( "b_attack", true );

			// Inversez l'état de la main pour la prochaine fois
			isRightHand = !isRightHand;

			// Obtenez la rotation de la caméra
			Rotation cameraRotation = Camera.Transform.Rotation;
			Vector3 cameraForward = cameraRotation.Forward;

			// Calcul de la direction dans laquelle le joueur regarde
			Vector3 direction = (Vector3.Forward * cameraRotation);

			Vector3 playerPosition = Transform.Position;
			//playerPosition.x += 20;
			Vector3 offset = direction * 40; // 20 unités devant le joueur
			playerPosition += offset;
			playerPosition.z += 40;



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

	public void cameraCollision()
	{
		Rotation cameraRotation = Camera.Transform.Rotation;
		Vector3 cameraBackward = cameraRotation.Backward;

	}

	public void speedDrop()
	{
		speedToggle = true;
		Task.Delay( 5000 ).ContinueWith(_ => { speedToggle = false; });
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

/*	public void AttachToHand()
	{
		Transform handTransform = model.GetAttachment( "hand_R" ).Value;
		Sphere.Transform.Position = handTransform.Position;
	}*/
	/*
		Transform handTransform = model.GetAttachmentName(hand_R);

		// Vérifier si la transformation de la main est valide
		if ( handTransform != null )
		{
			// Attribuer la position et la rotation du GameObject à la main du personnage
			sphere.Transform.Position = handTransform.Position;

*//*			// Attacher le GameObject au parent de la main du personnage
			sphere.Transform.Parent = handTransform.Parent;*//*
		}
	}*/
}
