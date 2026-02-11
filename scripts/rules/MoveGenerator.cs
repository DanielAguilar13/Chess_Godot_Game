using System.Collections.Generic;
using Godot;
using Chess.Board;

namespace Chess.Rules;

public static class MoveGenerator
{
	private static readonly Vector2I[] RookDirs = { new(1,0), new(-1,0), new(0,1), new(0,-1) };
	private static readonly Vector2I[] BishopDirs = { new(1,1), new(1,-1), new(-1,1), new(-1,-1) };

	public static List<Vector2I> GetMoves(PieceData piece, Vector2I from, PieceData?[,] board)
	{
		return piece.Type switch
		{
			PieceType.Knight => KnightRules.Moves(piece, from, board),
			PieceType.Bishop => RayMoves(piece, from, board, BishopDirs),
			PieceType.Rook   => RayMoves(piece, from, board, RookDirs),
			PieceType.Queen  => RayMoves(piece, from, board, RookDirs, BishopDirs),
			PieceType.King   => KingRules.Moves(piece, from, board),
			PieceType.Pawn   => PawnRules.Moves(piece, from, board),
			_ => new List<Vector2I>()
		};
	}

	private static List<Vector2I> RayMoves(PieceData piece, Vector2I from, PieceData?[,] board, params Vector2I[][] dirGroups)
	{
		var moves = new List<Vector2I>();
		foreach (var dirs in dirGroups)
		foreach (var dir in dirs)
		{
			var cur = from + dir;
			while (ChessUtils.InBounds(cur))
			{
				var target = board[cur.X, cur.Y];
				if (target == null)
				{
					moves.Add(cur);
				}
				else
				{
					if (target.Value.Color != piece.Color) moves.Add(cur);
					break;
				}
				cur += dir;
			}
		}
		return moves;
	}
}
