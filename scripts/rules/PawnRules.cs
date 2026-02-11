using System.Collections.Generic;
using Godot;
using Chess.Board;

namespace Chess.Rules;

public static class PawnRules
{
	public static List<Vector2I> Moves(PieceData piece, Vector2I from, PieceData?[,] board)
	{
		var moves = new List<Vector2I>();

		int dir = piece.Color == PieceColor.White ? -1 : 1;
		int startRow = piece.Color == PieceColor.White ? 6 : 1;

		// avance 1
		var one = new Vector2I(from.X, from.Y + dir);
		if (ChessUtils.InBounds(one) && board[one.X, one.Y] == null)
		{
			moves.Add(one);

			// avance 2 desde inicio si libre
			var two = new Vector2I(from.X, from.Y + 2 * dir);
			if (from.Y == startRow && board[two.X, two.Y] == null)
				moves.Add(two);
		}

		// capturas diagonales
		var diagL = new Vector2I(from.X - 1, from.Y + dir);
		var diagR = new Vector2I(from.X + 1, from.Y + dir);

		if (ChessUtils.InBounds(diagL) && board[diagL.X, diagL.Y] is PieceData p1 && p1.Color != piece.Color)
			moves.Add(diagL);

		if (ChessUtils.InBounds(diagR) && board[diagR.X, diagR.Y] is PieceData p2 && p2.Color != piece.Color)
			moves.Add(diagR);

		return moves;
	}
}
