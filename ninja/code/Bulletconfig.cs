using Sandbox;
using System;
using System.Runtime;

public sealed class Bulletconfig : Component, Component.ICollisionListener, Component.ITriggerListener
{

	public float bulletDespawn;

	[Property]
	[Range( 0f, 10000f, 100f )]
	public float bulletSpeed { get; set; } = 8000f;

	[Property] public Rigidbody bulletRigid { get; set; }

	[Property]
	public Test Player { get; set; }



	//public Rotation cameraRotation;
	public Vector3 cameraForward;

	private bool hasCollided = false;

	protected override void OnStart()
	{

		//Vector3 cameraForward = Player.cameraForward;
		//Log.Info( Player.cameraForward );

		bulletRigid.ApplyImpulse( Player.cameraForward * bulletSpeed );
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
	}

	public void OnCollisionStart( Collision c )
	{
				if ( !hasCollided )
				{
					if ( c.Other.Collider.Components.TryGet<UnitInfoBot>( out var unitInfoBot ) )
					{
						unitInfoBot.Damage( 50f );
					}
					if ( c.Other.Collider.Components.TryGet<UnitInfo>( out var unitInfo ) )
					{
						unitInfo.Damage( 50f );
					}
				}

				hasCollided = true;
				GameObject.Destroy();

			}
	
	public void OnTriggerEnter( Collider other )
	{
		GameObject.Destroy();
	}

	public void OnTriggerExit( Collider other )
	{
	}
}
