using Godot;

public partial class Ui : Control
{
	public int Score { get; private set; } = 0;

	private Label _scorelabel;
	
	

	public override void _Ready()
	{
		_scorelabel = GetNode<Label>("Score");
	}

	public override void _Process(double delta)
	{
		_scorelabel.Text = Score.ToString();
		
		if (Input.IsActionJustPressed("reset"))
		{
			GetTree().ReloadCurrentScene();
		}
	}
	
	public void OnEnemyKilled()
	{
		Score += 1;
	}
}
