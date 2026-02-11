using System.Collections.Generic;
using Godot;
using Chess.Board;

namespace Chess.Core;

public sealed class ChessMatch
{
	public PieceData?[,] Board { get; } = new PieceData?[8, 8];
	public PieceColor Turn { get; private set; } = PieceColor.White;

	public void ResetToInitial()
	{
		for (int x = 0; x < 8; x++)
		for (int y = 0; y < 8; y++)
			Board[x, y] = null;

		int id = 1;

		// Peones
		for (int x = 0; x < 8; x++)
		{
			Board[x, 6] = new PieceData(PieceType.Pawn, PieceColor.White, id++);
			Board[x, 1] = new PieceData(PieceType.Pawn, PieceColor.Black, id++);
		}

		// Piezas mayores
		PlaceBackRank(PieceColor.White, 7, ref id);
		PlaceBackRank(PieceColor.Black, 0, ref id);

		Turn = PieceColor.White;
	}

	private void PlaceBackRank(PieceColor color, int y, ref int id)
	{
		Board[0, y] = new PieceData(PieceType.Rook, color, id++);
		Board[1, y] = new PieceData(PieceType.Knight, color, id++);
		Board[2, y] = new PieceData(PieceType.Bishop, color, id++);
		Board[3, y] = new PieceData(PieceType.Queen, color, id++);
		Board[4, y] = new PieceData(PieceType.King, color, id++);
		Board[5, y] = new PieceData(PieceType.Bishop, color, id++);
		Board[6, y] = new PieceData(PieceType.Knight, color, id++);
		Board[7, y] = new PieceData(PieceType.Rook, color, id++);
	}

	public bool ApplyMove(Move move)
	{
		var from = move.From;
		var to = move.To;

		var piece = Board[from.X, from.Y];
		if (piece == null) return false;

		// mover
		Board[to.X, to.Y] = piece;
		Board[from.X, from.Y] = null;

		// cambiar turno
		Turn = Turn == PieceColor.White ? PieceColor.Black : PieceColor.White;
		return true;
	}
}
