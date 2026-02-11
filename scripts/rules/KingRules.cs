using System.Collections.Generic;
using Godot;
using Chess.Board;

namespace Chess.Rules;

public static class KingRules
{
	public static List<Vector2I> Moves(PieceData piece, Vector2I from, PieceData?[,] board)
	{
		var moves = new List<Vector2I>();
		for (int dx = -1; dx <= 1; dx++)
		for (int dy = -1; dy <= 1; dy++)
		{
			if (dx == 0 && dy == 0) continue;
			var to = new Vector2I(from.X + dx, from.Y + dy);
			if (!ChessUtils.InBounds(to)) continue;

			var target = board[to.X, to.Y];
			if (target == null || target.Value.Color != piece.Color)
				moves.Add(to);
		}
		return moves;
	}
}
