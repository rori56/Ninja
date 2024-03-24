using Sandbox;

public sealed class LootSpawner : Component
{

	[Property]
	public GameObject heal1 {  get; set; }

	[Property]
	public GameObject heal3 { get; set; }

	[Property]
	public GameObject heal4 { get; set; }

	[Property]
	public GameObject heal5 { get; set; }

	[Property]
	public int delay { get; set; }


	protected override void OnUpdate()
	{
		if ( heal1.Enabled == false )
		{
			Respawn1();
		}

	}

	public async void Respawn1()
	{
		await Task.Delay( delay );
		heal1.Enabled = true;
		Log.Info( "respawn" );
	}

}
