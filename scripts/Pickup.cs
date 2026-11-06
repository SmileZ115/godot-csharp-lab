using Godot;

public partial class Pickup : Area2D
{
	[Signal]
	public delegate void PickupCollectedEventHandler(int amount);

	private Timer lifetimeTimer;

	private PickupRarity rarity = PickupRarity.Common;

	private int points;

	[Export]
	private double lifetime = 10.0;

	public override void _Ready()
	{
		lifetimeTimer = GetNode<Timer>("LifetimeTimer");
		lifetimeTimer.WaitTime = lifetime;
		lifetimeTimer.Start();
	}

	private void _on_body_entered(Node2D body)
	{
		if (body is Player)
		{
			EmitSignal(SignalName.PickupCollected, points);

			QueueFree();
		}
	}

	private void LifetimeTimerTimeout()
	{
		QueueFree();
		GD.Print("Pickup expired!");
	}

	public void SetPoints(int amount)
	{
		points = amount;
	}

	public void SetRarity(PickupRarity newRarity)
	{
		rarity = newRarity;
		if (rarity == PickupRarity.Common)
		{
			Modulate = Colors.Gray;
		}
		else if (rarity == PickupRarity.Special)
		{
			Modulate = Colors.Blue;
		}
		else if (rarity == PickupRarity.Rare)
		{
			Modulate = Colors.Gold;
		}
	}
}
