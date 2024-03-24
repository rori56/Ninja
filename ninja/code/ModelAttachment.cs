using Sandbox;

public sealed class ModelAttachment : Component
{

	[Property]
	public SkinnedModelRenderer model { get; set; }

	[Property]
	public GameObject firehandR { get; set; }

	[Property]
	public GameObject firehandL { get; set; }

	[Property]
	public Test player { get; set; }

	[Property]
	public Inventory inventaire { get; set; }

	private bool missileEffectClonedR = false;

	private bool missileEffectClonedL = false;

	protected override void OnUpdate()
	{
		var handTransformR = model.GetAttachment( "hand_R" ).Value;
		var handTransformL = model.GetAttachment( "hand_L" ).Value;
		firehandR.Transform.Position = handTransformR.Position;
		firehandL.Transform.Position = handTransformL.Position;

		if ( inventaire.slot == 2)
		{
			firehandR.Enabled = true;
			firehandL.Enabled = true;
		}
		else
		{
			firehandR.Enabled = false;
			firehandL.Enabled = false;
		}
	}
}
