using System.Collections.Generic;
using Godot;
using Chess.Board;

namespace Chess.Rules;

public static class KnightRules
{
	private static readonly Vector2I[] Deltas = {
		new(1,2), new(2,1), new(2,-1), new(1,-2),
		new(-1,-2), new(-2,-1), new(-2,1), new(-1,2)
	};

	public static List<Vector2I> Moves(PieceData piece, Vector2I from, PieceData?[,] board)
	{
		var moves = new List<Vector2I>();
		foreach (var d in Deltas)
		{
			var to = from + d;
			if (!ChessUtils.InBounds(to)) continue;

			var target = board[to.X, to.Y];
			if (target == null || target.Value.Color != piece.Color)
				moves.Add(to);
		}
		return moves;
	}
}
