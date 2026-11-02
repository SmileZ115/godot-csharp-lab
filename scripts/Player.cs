using Godot;

public partial class Player : Sprite2D
{
	[Export]
	private float speed = 200f;

	public override void _Process(double delta)
	{
		Vector2 dir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

		Position += dir * speed * (float)delta;
	}
}
