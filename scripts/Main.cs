using Godot;

public partial class Main : Node2D
{
    private Vector2 screenSize;

    private float margin = 40f;

    private int score = 0;

    private RandomNumberGenerator rng;

    private Label scoreLabel;

    [Export]
    private PackedScene pickupScene;

    [Export]
    private float minPickupDistance = 50f;

    public override void _Ready()
    {
        screenSize = GetViewportRect().Size;

        rng = new RandomNumberGenerator();

        scoreLabel = GetNode<Label>("HUD/ScoreLabel");

        UpdateScoreLabel();
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("ui_accept"))
        {
            SpawnPickup();
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
}
