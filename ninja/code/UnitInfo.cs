using Sandbox;

public enum UnitType
{
	[Icon( "check_box_outline_blank" )]
	None,
	[Icon( "boy" )]
	Player,
	[Icon( "mood_bad" )]
	Ennemy
}

public sealed class UnitInfo : Component
{
	[Property]
	public UnitType Team { get; set; }

	[Property]
	[Range( 0.1f, 10f, 0.1f )]
	public float MaxHealth { get; set; } = 5f;

	[Property]
	[Range( 0f, 2f, 0.1f )]
	public float HealthRegenAmount { get; set; } = 0.5f;

	[Property]
	[Range( 1f, 5f, 1f )]
	public float HealthRegenTimer { get; set; } = 3f;

	public float Health { get; private set; }

	public bool Alive { get; private set; } = true;
	TimeSince _lastDamage;
	TimeUntil _nextHeal;

	protected override void OnUpdate()
	{
		if ( _lastDamage >= HealthRegenTimer && Health != MaxHealth && Alive )
		{
			if ( _nextHeal )
			{
				Damage( -HealthRegenAmount );
				_nextHeal = 1f;
			}
		}
	}

	protected override void OnStart()
	{
		Health = MaxHealth;
	}

	public void Damage( float damage )
	{
		if ( !Alive ) return;

		Health = MathX.Clamp( Health - damage, 0f, MaxHealth );

		if ( damage > 0 )
			_lastDamage = 0f;

		if ( Health <= 0 )
			Krill();
	}

	public void Krill()
	{
		Health = 0f;
		Alive = false;

		GameObject.Destroy();
	}
}
