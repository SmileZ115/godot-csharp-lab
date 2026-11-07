using Godot;

public partial class Pickup : Area2D
{
	[Signal]
	public delegate void PickupCollectedEventHandler(int amount, PickupRarity pickupRarity);

	[Export]
	private double lifetime = 10.0;

	private Timer lifetimeTimer;

	private PickupRarity rarity = PickupRarity.Common;

	private int points = 1;

	public int Points
	{
		get
		{
			return points;
		}
	}

	public PickupRarity Rarity
	{
		get
		{
			return rarity;
		}
	}

	public override void _Ready()
	{
		lifetimeTimer = GetNode<Timer>("LifetimeTimer");
		lifetimeTimer.WaitTime = lifetime;
		lifetimeTimer.Start();
	}

	public void SetPoints(int amount)
	{
		points = amount;
	}

	public void SetRarity(PickupRarity newRarity)
	{
		rarity = newRarity;
		
		switch (rarity)
		{
			case PickupRarity.Common:
				Modulate = Colors.Gray;
				break;
			case PickupRarity.Special:
				Modulate = Colors.Blue;
				break;
			case PickupRarity.Rare:
				Modulate = Colors.Gold;
				break;
			default:
				Modulate = Colors.White;
				GD.Print("Unknown rarity type.");
				break;
		}
	}

	private void _on_body_entered(Node2D body)
	{
		if (body is Player)
		{
			EmitSignal(SignalName.PickupCollected, points, (int)rarity);

			QueueFree();
		}
	}

	private void LifetimeTimerTimeout()
	{
		GD.Print("Pickup expired!");

		QueueFree();
	}
}
