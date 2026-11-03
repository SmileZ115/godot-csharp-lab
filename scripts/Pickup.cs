using Godot;

public partial class Pickup : Area2D
{
	[Signal]
	public delegate void PickupCollectedEventHandler(int amount);

	[Export]
	private int points = 1;

	private void _on_body_entered(Node2D body)
	{
		if (body is Player)
		{
			EmitSignal(SignalName.PickupCollected, points);

			QueueFree();
		}
	}
}
