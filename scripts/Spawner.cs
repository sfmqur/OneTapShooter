using Godot;

public partial class Spawner : Node2D
{
	[Export] public PackedScene? EnemyScene { get; set; }
	
	/// <summary>
	/// How quickly spawn interval decreases over time.
	/// Higher = faster ramp. Interval = 1 / (1 + elapsed * DifficultyFactor).
	/// </summary>
	[Export] public float DifficultyFactor { get; set; } = 0.1f;

	private float _velocity = 1f;
	private float _elapsed = 0f;
	private Timer _spawnTimer;
	private Ui _ui;

	public override void _Ready()
	{
		_ui = GetParent().GetNode<Ui>("UI");

		_spawnTimer = new Timer();
		AddChild(_spawnTimer);
		_spawnTimer.WaitTime = 1.0;
		_spawnTimer.Timeout += OnSpawnTimerTimeout;
		_spawnTimer.Start();
	}

	public override void _Process(double delta)
	{
		_elapsed += (float)delta;
		checkBoundaries();
	}

	private void OnSpawnTimerTimeout()
	{
		spawnEnemy();

		float nextInterval = 1f / (1f + _elapsed * DifficultyFactor);
		_spawnTimer.WaitTime = Mathf.Max(0.1f, nextInterval);
		_spawnTimer.Start();
	}

	private void spawnEnemy()
	{
		if (EnemyScene is null) return;
		var rng = new RandomNumberGenerator();
		var viewportWidth = GetViewport().GetVisibleRect().Size.X;
		var enemy = EnemyScene.Instantiate<Enemy>();
		enemy.Position = new Vector2(rng.RandfRange(8f, viewportWidth - 8f), -9f);
		if (_ui is not null)
			enemy.OnDeath += _ui.OnEnemyKilled;
		GetParent()?.AddChild(enemy);
	}

	private void checkBoundaries()
	{
		var viewportSize = GetViewport().GetVisibleRect().Size;
		if (Position.X < 0)
		{
			Position = new Vector2(8, Position.Y);
			_velocity = 1f;
		}
		else if (Position.X > viewportSize.X)
		{
			Position = new Vector2(viewportSize.X - 8, Position.Y);
			_velocity = -1f;
		}
	}
}
