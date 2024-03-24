using Sandbox;
using Sandbox.ActionGraphs;
using System.Diagnostics;

public sealed class Visor : Component
{
	[Property]
	public GameObject Camera { get; set; }

	[Property]
	public GameObject Cube { get; set; }


	protected override void OnStart()
	{

	}

	protected override void OnUpdate()
	{

		Rotation cameraRotation = Camera.Transform.Rotation;
		//Vector3 cameraForward = cameraRotation.Forward;

		// Calcul de la direction dans laquelle le joueur regarde
		Vector3 direction = (Vector3.Forward * cameraRotation);

		Vector3 playerInitial = Transform.Position;
		playerInitial.z += 40;
		Vector3 offsett = direction * 40; // 20 unités devant le joueur
		playerInitial += offsett;

		Vector3 playerPosition = Transform.Position;
		//playerPosition.x += 20;
		Vector3 offset = direction * 10000; // 20 unités devant le joueur
		playerPosition += offset;
		playerPosition.z += 40;

		Cube.Transform.Position = playerInitial;

		var visorTrace = Scene.Trace
			.FromTo( playerInitial, playerPosition )
			.Size( 1f )
			.IgnoreGameObjectHierarchy( GameObject )
			.Run();

		if ( visorTrace.Hit )
		{
			Cube.Transform.Position = visorTrace.EndPosition;
		}
		else
		{
			Cube.Transform.Position = playerPosition;
		}

	}


}
