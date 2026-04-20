using Godot;

public partial class Player : Node2D
{
	[Signal] public delegate void OnDeathEventHandler();
	[Export] public float Health { get; set; } = 10f; 
	[Export] public float Speed { get; set; } = 200f;
	[Export] public float ProjectileSpeed { get; set; } = 200f;
	[Export] public float ProjectileDamage { get; set; } = 1f;
	[Export] public PackedScene? ProjectileScene { get; set; }


	private float _velocity = 1f;

	public override void _Ready()
	{
		var animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animation.Play("default");
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("tap"))
		{
			_velocity = -_velocity;
			launchProjectile();
		}

		Position += new Vector2(_velocity * Speed * (float)delta, 0);
		checkBoundaries();
		checkHealth();
	}

	public void OnCollide(Node2D body)
	{
		var sender = body.GetParent();
		if (sender is Projectile projectile && projectile.Sender is Enemy enemy2)
		{ 
			Health -= 1;
			GetNode<AudioStreamPlayer2D>("AudioHitPlayer").Play();
			projectile.Destroy();
		}
	}
	
	private void checkBoundaries()
	{
		var viewportSize = GetViewport().GetVisibleRect().Size;
		if (Position.X < 0)
		{
			Position = new Vector2(8, Position.Y);
			_velocity = 1f;
		}
		else if (Position.X >  viewportSize.X)
		{
			Position = new Vector2(viewportSize.X-8, Position.Y);
			_velocity = -1f;
		}
	}

	private void checkHealth()
	{
		if (Health <= 0)
		{
			EmitSignal(SignalName.OnDeath);
			QueueFree();
		}
	}
	private void launchProjectile()
	{
		if (ProjectileScene is null) return;
		var projectile = ProjectileScene.Instantiate<Projectile>();
		projectile.Sender = this;
		projectile.Damage = ProjectileDamage;
		projectile.Speed = ProjectileSpeed;
		projectile.Direction = -1f;

		projectile.Position = new Vector2(Position.X, Position.Y - 10);
		GetParent()?.AddChild(projectile, true);

		GetNode<AudioStreamPlayer2D>("AudioShotPlayer").Play();
	}

	
}
