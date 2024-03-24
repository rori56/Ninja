using Sandbox;
using System.Threading.Tasks;

public sealed class ShieldImpact : Component, Component.ITriggerListener
{
	[Property]
	public GameObject Impact {  get; set; }

	public GameObject ImpactObj;

	protected override void OnUpdate()
	{

	}

	public void OnTriggerEnter( Collider other )
	{
		if ( other.Tags.Has( "bullet" ) )
		{
			Vector3 ImpactPos = other.Transform.Position;
			ImpactObj = Impact.Clone( ImpactPos );
		}
	}

	public void OnTriggerExit( Collider other )
	{
	}
}
