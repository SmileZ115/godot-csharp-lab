using Godot;

public partial class Main : Node2D
{
    private Vector2 screenSize;

    private float margin = 40f;

    private int score = 0;

    private const int MaxSpawnAttempts = 5;

    private RandomNumberGenerator rng;

    private Label scoreLabel;

    private Timer pickupTimer;

    private Label spawnStatusLabel;

    private Label pickupCountLabel;

    [Export]
    private PackedScene pickupScene;

    [Export]
    private float minPickupDistance = 50f;

    [Export]
    private int maxPickups = 10;

    public override void _Ready()
    {
        screenSize = GetViewportRect().Size;

        rng = new RandomNumberGenerator();

        pickupTimer = GetNode<Timer>("PickupTimer");

        scoreLabel = GetNode<Label>("HUD/ScoreLabel");

        spawnStatusLabel = GetNode<Label>("HUD/SpawnStatusLabel");

        pickupCountLabel = GetNode<Label>("HUD/PickupCountLabel");

        UpdateScoreLabel();
        UpdateSpawnStatusLabel();
        UpdatePickupCountLabel();
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("ui_accept"))
        {
            SpawnPickup();
        }

        if (Input.IsActionJustPressed("toggle_spawn"))
        {
            TogglePickupTimer();
        }
    }

    private void OnPickupCollected(int amount)
    {
        AddScore(amount);
    }

    private void UpdateScoreLabel()
    {
        scoreLabel.Text = "Score: " + score;
    }

    private void AddScore(int amount)
    {
        score += amount;
        UpdateScoreLabel();
    }

    private void SpawnPickup()
    {
        var pickups = GetTree().GetNodesInGroup("Pickups");
        if (pickups.Count >= maxPickups)
        {
            GD.Print("Maximum pickup limit reached!");
            return;
        }
        for (int i = 0; i < MaxSpawnAttempts; i++)
        {
            var spawnPosition = new Vector2(
            rng.RandfRange(margin, screenSize.X - margin),
            rng.RandfRange(margin, screenSize.Y - margin)
            );

            if (IsSpawnPositionValid(spawnPosition))
            {
                var pickup = (Pickup)pickupScene.Instantiate();

                int roll = rng.RandiRange(1, 100);

                if (roll <= 70)
                {
                    pickup.SetPoints(1);
                    pickup.SetRarity(PickupRarity.Common);
                }
                else if (roll <= 95)
                {
                    pickup.SetPoints(5);
                    pickup.SetRarity(PickupRarity.Special);
                }
                else
                {
                    pickup.SetPoints(10);
                    pickup.SetRarity(PickupRarity.Rare);
                }

                pickup.Position = spawnPosition;
                
                pickup.PickupCollected += OnPickupCollected;

                pickup.TreeExited += OnPickupRemoved;

                AddChild(pickup);

                UpdatePickupCountLabel();

                return;
            }
        }

        GD.Print($"Failed to find a valid spawn position after {MaxSpawnAttempts} attempts.");
    }

    private bool IsSpawnPositionValid(Vector2 position)
    {
        var pickups = GetTree().GetNodesInGroup("Pickups");

        foreach (Pickup pickup in pickups)
        {
            if (pickup.Position.DistanceTo(position) < minPickupDistance)
            {
                return false;
            }
        }
        return true;
    }

    private void _on_pickup_timer_timeout()
    {
        SpawnPickup();
    }

    private void TogglePickupTimer()
    {
        if (pickupTimer.IsStopped())
        {
            pickupTimer.Start();
            GD.Print("Pickup timer started.");
        }
        else
        {
            pickupTimer.Stop();
            GD.Print("Pickup timer stopped.");
        }
        UpdateSpawnStatusLabel();
    }

    private void UpdateSpawnStatusLabel()
    {
        spawnStatusLabel.Text = pickupTimer.IsStopped() ? "Spawn: Off" : "Spawn: On";
    }

    private void UpdatePickupCountLabel()
    {
        var pickupCount = GetTree().GetNodesInGroup("Pickups").Count;
        pickupCountLabel.Text = $"Pickups: {pickupCount} / {maxPickups}";
    }

    private void OnPickupRemoved()
    {
        UpdatePickupCountLabel();
    }
}
