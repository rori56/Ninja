using Sandbox;
using System.Numerics;

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
	[Range( 1f, 100f, 1f )]
	public float MaxHealth { get; set; } = 100f;

	[Property]
	[Range( 0f, 2f, 0.1f )]
	public float HealthRegenAmount { get; set; } = 1f;

	[Property]
	[Range( 0f, 50f, 10f )]
	public float ManaRegenAmount { get; set; } = 20f;

	[Property]
	[Range( 1f, 5f, 1f )]
	public float HealthRegenTimer { get; set; } = 3f;

	[Property]
	[Range( 1f, 5f, 1f )]
	public float ManaRegenTimer { get; set; } = 3f;

	[Property]
	[Range( 1f, 200f, 1f )]
	public float MaxMana { get; set; } = 200f;

	public float Mana { get; set; }

	public float Health { get; private set; }

	public bool Alive { get; private set; } = true;

	public TimeSince _lastMissile;
	TimeSince _lastDamage;
	TimeUntil _nextHeal;
	TimeUntil _nextMana;

	protected override void OnUpdate()
	{
		if ( _lastDamage >= HealthRegenTimer && Health != MaxHealth && Alive )
		{
			if ( _nextHeal )
			{
				Damage( -HealthRegenAmount );
				_nextHeal = 1f;
				Log.Info( Health );
			}
		}

		if ( _lastMissile >= ManaRegenTimer && Mana != MaxMana && Alive )
		{
			if ( _nextMana )
			{
				Log.Info( _lastMissile );
				Regen( ManaRegenAmount );
				_nextMana = 1f;
			}
		}
	}

	protected override void OnStart()
	{
		Health = 10;
		Mana = MaxMana;
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

	public void Regen( float regen )
	{
		if ( !Alive ) return;
			Mana = MathX.Clamp( Mana + regen, 0f, MaxMana );

	}

	public void Krill()
	{
		Health = 0f;
		Alive = false;

		GameObject.Destroy();
	}
}
