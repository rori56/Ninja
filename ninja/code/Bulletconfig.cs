using Sandbox;
using System;
using System.Runtime;
using System.Collections;
using System.Diagnostics;

public sealed class Bulletconfig : Component
{

	//public Rigidbody bulletRigid = null;
	//public Rigidbody bulletRigid = new Rigidbody();

	public float bulletDespawn;

	[Property]
	public GameObject Camera { get; set; }

	[Property]
	[Range( 0f, 10000f, 100f )]
	public float bulletSpeed { get; set; } = 8000f;

	[Property] public Rigidbody bulletRigid { get; set; }

	public Rotation cameraRotation;

	protected override void OnStart()
	{

		if ( bulletRigid != null )
		{

			Rotation cameraRotation = Camera.Transform.Rotation;
			Vector3 cameraForward = cameraRotation.Forward;

			bulletRigid.ApplyImpulse( cameraForward * bulletSpeed );
			//bulletRigid.ApplyForce( Vector3.Forward * bulletSpeed * Time.Delta );
		}
		else
		{
			// Si bulletRigid est null, affichez un message 
			Log.Info( "bulletRigid est null. Assurez-vous de l'initialiser correctement avant d'utiliser cette classe." );
		}
	}
}
