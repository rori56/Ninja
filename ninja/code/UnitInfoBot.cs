using Sandbox;

public enum UnitTypeBot
{
	[Icon( "check_box_outline_blank" )]
	None,
	[Icon( "boy" )]
	Player,
	[Icon( "mood_bad" )]
	Ennemy
}

public sealed class UnitInfoBot : Component
{
	[Property]
	public UnitType Team { get; set; }

	[Property]
	[Range( 0.1f, 100f, 0.1f )]
	public float MaxHealth { get; set; } = 100f;

	public float Health { get; private set; }

	public bool Alive { get; private set; } = true;
	TimeSince _lastDamage;
	//TimeUntil _nextHeal;

	protected override void OnUpdate()
	{
		base.OnUpdate();
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
