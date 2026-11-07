using Godot;

public partial class Main : Node2D
{
    [Export]
    private PackedScene pickupScene;

    [Export]
    private float minPickupDistance = 50f;

    [Export]
    private int maxPickups = 10;

    private Vector2 screenSize;

    private float margin = 40f;

    private int score = 0;

    private int highScore = 0;

    private int highCombo = 0;

    private int combo = 0;

    private const int MaxSpawnAttempts = 5;

    private RandomNumberGenerator rng;

    private Timer comboTimer;

    private Timer pickupTimer;

    private Label scoreLabel;

    private Label spawnStatusLabel;

    private Label pickupCountLabel;

    private Label lastPickupLabel;

    private Label highScoreLabel;

    private Label highComboLabel;

    private Label comboLabel;

    public override void _Ready()
    {
        screenSize = GetViewportRect().Size;

        rng = new RandomNumberGenerator();

        pickupTimer = GetNode<Timer>("PickupTimer");

        comboTimer = GetNode<Timer>("ComboTimer");

        scoreLabel = GetNode<Label>("HUD/ScoreLabel");

        spawnStatusLabel = GetNode<Label>("HUD/SpawnStatusLabel");

        pickupCountLabel = GetNode<Label>("HUD/PickupCountLabel");

        lastPickupLabel = GetNode<Label>("HUD/LastPickupLabel");

        highScoreLabel = GetNode<Label>("HUD/HighScoreLabel");

        comboLabel = GetNode<Label>("HUD/ComboLabel");

        highComboLabel = GetNode<Label>("HUD/HighComboLabel");

        UpdateScoreLabel();
        UpdateSpawnStatusLabel();
        UpdatePickupCountLabel();
        UpdateComboLabel();
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

        if (Input.IsActionJustPressed("reset_score"))
        {
            ResetScore();
        }
    }

    private void OnPickupCollected(int amount, PickupRarity pickupRarity)
    {
        UpdateCombo();

        amount += CalculatePercentageBonus(amount);

        AddScore(amount);

        UpdateLastPickupColor(pickupRarity);
        lastPickupLabel.Text = $"Last Pickup: {pickupRarity}";
    }

    private void UpdateComboLabel()
    {
        comboLabel.Text = "Combo: " + combo;

        highComboLabel.Text = "High Combo: " + highCombo;
    }

    private void UpdateCombo()
    {
        combo += 1;
        comboTimer.Start();

        if (combo > highCombo)
        {
            highCombo = combo;
        }

        UpdateComboLabel();
    }

    private int CalculatePercentageBonus(int basePoints)
    {
        float percentage = combo * 0.005f;
        float bonus = basePoints * percentage;
        return Mathf.RoundToInt(bonus);
    }

    private void UpdateScoreLabel()
    {
        scoreLabel.Text = "Score: " + score;

        highScoreLabel.Text = "High Score: " + highScore;
    }

    private void AddScore(int amount)
    {
        score += amount;

        if (score > highScore)
        {
            highScore = score;

            GD.Print($"New high score! {highScore} points!");
        }

        UpdateScoreLabel();
    }

    private void ResetScore()
    {
        score = 0;
        combo = 0;

        comboTimer.Stop();

        UpdateComboLabel();
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

    private void OnComboTimerTimeout()
    {
        combo = 0;
        UpdateComboLabel();
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

    private void UpdateLastPickupColor(PickupRarity rarity)
    {
        switch (rarity)
        {
            case PickupRarity.Common:
                lastPickupLabel.Modulate = Colors.Gray;
                break;
            case PickupRarity.Special:
                lastPickupLabel.Modulate = Colors.Blue;
                break;
            case PickupRarity.Rare:
                lastPickupLabel.Modulate = Colors.Gold;
                break;
            default:
                lastPickupLabel.Modulate = Colors.White;
                GD.Print("Unknown rarity type.");
                break;
        }
    }
}
