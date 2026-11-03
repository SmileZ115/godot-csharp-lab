using Godot;

public partial class Player : CharacterBody2D
{
	[Export]
	private float speed = 200f;

	public override void _PhysicsProcess(double delta)
	{
		Vector2 dir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

		Velocity = dir * speed;

		bool collided = MoveAndSlide();

		if (collided)
		{
			GodotObject collider = GetLastSlideCollision().GetCollider();
			if (collider is StaticBody2D)
			{
				GD.Print("StaticBody2D body detected!");
			}
			else
			{
				GD.Print("Other body detected!");
			}
		}
	}
}
