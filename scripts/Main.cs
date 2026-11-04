using Godot;

public partial class Main : Node2D
{
    private Vector2 screenSize;

    private float margin = 40f;

    private int score = 0;

    private RandomNumberGenerator rng;

    private Label scoreLabel;

    private Timer pickupTimer;

    private Label spawnStatusLabel;

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

        UpdateScoreLabel();
        UpdateSpawnStatusLabel();
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
        for (int i = 0; i < 5; i++)
        {
            var spawnPosition = new Vector2(
            rng.RandfRange(margin, screenSize.X - margin),
            rng.RandfRange(margin, screenSize.Y - margin)
            );

            if (IsSpawnPositionValid(spawnPosition))
            {
                var pickup = (Pickup)pickupScene.Instantiate();

                pickup.Position = spawnPosition;
                
                pickup.PickupCollected += OnPickupCollected;

                AddChild(pickup);

                return;
            }
        }

        GD.Print("Failed to find a valid spawn position for pickup after 5 attempts.");
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
        if (pickupTimer.IsStopped())
        {
            spawnStatusLabel.Text = "Auto Spawn: OFF";
        }
        else
        {
            spawnStatusLabel.Text = "Auto Spawn: ON";
        }
    }
}
