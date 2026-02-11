using Godot;

namespace Chess.Board;

public readonly struct Move
{
	public Vector2I From { get; }
	public Vector2I To { get; }

	public Move(Vector2I from, Vector2I to)
	{
		From = from;
		To = to;
	}
}
