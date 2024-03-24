using Sandbox;

public sealed class Inventory : Component
{
	public int slot = 1;

	[Property] Test Player { get; set; }

	protected override void OnUpdate()
	{
		if ( Input.Down( "Slot1" ) )
			slot = 1;

		if ( Input.Down( "Slot2" ) )
			slot = 2;
	}
}
