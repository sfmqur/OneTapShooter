using Godot;
using System;

public partial class Enemy : Node2D
{
	[Signal] public delegate void OnDeathEventHandler();

	[Export] public float MaxHealth = 5f;
	[Export] public float MinShootTime = 0.25f; 
	[Export] public float Speed { get; set; } = 50f;
	[Export] public float ProjectileSpeed { get; set; } = 200f;
	[Export] public float ProjectileDamage { get; set; } = 1f;
	[Export] public PackedScene? ProjectileScene { get; set; }
	public float Health { get; set; } = 1f; 
	public float ShootTime { get; set; } = 1f; 


	
	private Timer _shootTimer;
 
	public override void _Ready()
	{
		var rng = new RandomNumberGenerator();
		Health = rng.RandfRange(0f, MaxHealth);
		ShootTime = rng.RandfRange(MinShootTime, 1f);

		var animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animation.Play("default");

		_shootTimer = new Timer();
		AddChild(_shootTimer);
		_shootTimer.WaitTime = ShootTime;
		_shootTimer.Timeout += launchProjectile;
		_shootTimer.Start();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position += new Vector2(0, Speed * (float)delta);
		
		checkHealth();
		checkBoundaries();
	}
	
	public void OnCollide(Node2D body)
	{
		var sender = body.GetParent();
		if (sender is Projectile projectile && projectile.Sender is Player player)
		{
			 Health -= 1;
			projectile.Destroy();
		}
		else if (sender is Projectile projectile2)
		{
			projectile2.Destroy();
		}
		else if (body is Player player2)
		{
			QueueFree();
		}
	}

	private void checkHealth()
	{
		if (Health <= 0)
		{
			EmitSignal(SignalName.OnDeath);
			GetNode<AudioStreamPlayer2D>("AudioSquishPlayer").Play();

			QueueFree();
		}
	}
	
	private void checkBoundaries()
	{
		var viewportSize = GetViewport().GetVisibleRect().Size;
		if (Position.Y >  viewportSize.X)
		{
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
		projectile.Direction = 1f;
		projectile.RotationDegrees = 180f;

		projectile.Position = new Vector2(Position.X, Position.Y + 9);
		GetParent()?.AddChild(projectile, true);
	}
}
