using Sandbox;
using System;
using System.Runtime;

public sealed class Bulletconfig : Component, Component.ICollisionListener
{

	public float bulletDespawn;

	[Property]
	public GameObject Camera { get; set; }

	[Property]
	[Range( 0f, 10000f, 100f )]
	public float bulletSpeed { get; set; } = 8000f;

	[Property] public Rigidbody bulletRigid { get; set; }

	[Property]
	public Test Player { get; set; }

	public Rotation cameraRotation;


	protected override void OnStart()
	{

		if ( bulletRigid != null )
		{

			Rotation cameraRotation = Camera.Transform.Rotation;
			Vector3 cameraForward = cameraRotation.Forward;

			bulletRigid.ApplyImpulse( cameraForward * bulletSpeed );
		}
		else
		{
			Log.Info( "bulletRigid est null" );
		}
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
	}

	public void OnCollisionStart( Collision c )
	{
		if ( c.Other.Collider.Components.TryGet<UnitInfoBot>( out var unitInfoBot ) )
			unitInfoBot.Damage( 50f );

		GameObject.Destroy();

	}


	public void OnCollisionUpdate( Collision c )
	{
	}

	public void OnCollisionStop( CollisionStop c )
	{
	}

}
