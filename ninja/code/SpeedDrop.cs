using Sandbox;
using System.ComponentModel.Design;
using System.Security.Cryptography.X509Certificates;

public sealed class SpeedDrop : Component, Component.ITriggerListener
{

	[Property]
	private float rotationSpeed = 100f;

	private float currentRotation = 0f;


	protected override void OnUpdate()
	{
		float rotationDelta = rotationSpeed * Time.Delta;

		currentRotation += rotationDelta;

		currentRotation %= 360f;

		GameObject.Transform.Rotation = Rotation.FromYaw( currentRotation );
	}

	public void OnTriggerEnter( Collider other )
	{
		if ( other.Components.TryGet<Test>( out var test ) )
		{
			test.speedDrop();
		}

		if ( other.Tags.Has( "player" ) )
			GameObject.Destroy();

	}

	public void OnTriggerExit( Collider other )
	{
	}
}
