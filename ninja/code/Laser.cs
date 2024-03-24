using Sandbox;
using Sandbox.ActionGraphs;
using System.Diagnostics;

public sealed class Laser : Component
{
	[Property]
	public GameObject Camera { get; set; }

	[Property]
	public LineRenderer laserLine { get; set; }


	protected override void OnStart()
	{
		laserLine.Width = 1f;
	}

	protected override void OnUpdate()
	{

		Rotation cameraRotation = Camera.Transform.Rotation;
		Vector3 cameraForward = cameraRotation.Forward;

		Vector3 direction = (Vector3.Forward * cameraRotation);

		Vector3 playerStart = Transform.Position;
		Vector3 offset = direction * 40;
		playerStart += offset;
		playerStart.z += 40;
		Vector3 playerEnd = playerStart + direction * 500f;

		DrawLaser( playerStart, playerEnd );

	}

	void DrawLaser( Vector3 start, Vector3 end )
	{
	}
}
