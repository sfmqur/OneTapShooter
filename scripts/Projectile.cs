using Godot;
using System;

public partial class Projectile : Node2D
{
	[Export] public float Damage { get; set; } = 1f;
	[Export] public float Speed { get; set; } = 200f;

	public Node2D? Sender { get; set; } = null; 

	public float Direction { get; set; } = 1f; 
	
	private Vector2 _viewportSize;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_viewportSize = GetViewport().GetVisibleRect().Size;
		GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("default");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Position.X > _viewportSize.X || Position.Y > _viewportSize.Y || Position.X < 0 || Position.Y < 0)
		{
			QueueFree();
		}
		
		Position += new Vector2(0, Direction * Speed * (float)delta);
	}

	public void Destroy()
	{
		QueueFree();
	}
}
