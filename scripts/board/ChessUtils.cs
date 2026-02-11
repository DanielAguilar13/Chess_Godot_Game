using Godot;

namespace Chess.Board;

public static class ChessUtils
{
	public static bool InBounds(Vector2I c) => c.X is >= 0 and < 8 && c.Y is >= 0 and < 8;

	public static Vector2I WorldToCell(Vector2 world, Vector2 origin, int cellSize)
	{
		var local = world - origin;
		return new Vector2I((int)(local.X / cellSize), (int)(local.Y / cellSize));
	}

	public static Vector2 CellToWorld(Vector2I cell, Vector2 origin, int cellSize)
		=> origin + new Vector2(cell.X * cellSize, cell.Y * cellSize);
}
