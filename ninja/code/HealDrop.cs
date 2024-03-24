using Sandbox;
using System.ComponentModel.Design;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

public sealed class HealDrop : Component, Component.ITriggerListener
{

	[Property]
	private float rotationSpeed = 100f;

	private float currentRotation = 0f;

	public bool isDestroyed;


	protected override void OnUpdate()
	{
		if ( GameObject.Enabled )
		{

			float rotationDelta = rotationSpeed * Time.Delta;

			currentRotation += rotationDelta;

			currentRotation %= 360f;

			GameObject.Transform.Rotation = Rotation.FromYaw( currentRotation );

		}


	}

	public void OnTriggerEnter( Collider other )
	{


		if ( other.Tags.Has( "player" ) )
		{
			if ( other.Components.TryGet<UnitInfo>( out var unitInfo ) )
			{
				unitInfo.healDrop( 50f );
			}

			//GameObject.Destroy();
			GameObject.Enabled = false;

		}
	}

	public void OnTriggerExit( Collider other )
	{
	}

}
