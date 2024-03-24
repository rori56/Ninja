using Sandbox;

public sealed class Weapons : Component
{

	[Property]
	public GameObject shield {  get; set; }

	[Property]
	public GameObject Camera { get; set; }

	[Property]
	public UnitInfo InfoPlayer { get; set; }

	public GameObject playerShield = null;

	protected override void OnStart()
	{
		playerShield = shield.Clone();
	}

	protected override void OnUpdate()
	{
		if ( Input.Down( "attack2" ) && !Input.Down( "attack1" ) )
		{
			Shield();

			InfoPlayer.Mana -= 0.25f;
			InfoPlayer._lastMissile = 0f;
		}
		else
		{
			playerShield.Enabled = false;
		}

	}

	public void Shield()
	{
		if ( InfoPlayer.Mana > 0.25f )
		{
			InfoPlayer._lastShield = 0f;
			playerShield.Enabled = true;

			// Obtenez la rotation de la caméra
			Rotation cameraRotation = Camera.Transform.Rotation;

			// Calcul de la direction dans laquelle le joueur regarde
			Vector3 direction = (Vector3.Forward * cameraRotation);

			Vector3 playerPosition = Transform.Position;
			//playerPosition.x += 20;
			Vector3 offset = direction * 40;
			playerPosition += offset;
			playerPosition.z += 40;

			playerShield.Transform.Position = playerPosition;
			playerShield.Transform.Rotation = cameraRotation;
		}
	}
}
