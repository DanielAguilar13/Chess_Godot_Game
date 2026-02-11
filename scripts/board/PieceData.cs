namespace Chess.Board;

public enum PieceType { King, Queen, Rook, Bishop, Knight, Pawn }
public enum PieceColor { White, Black }

public readonly struct PieceData
{
	public PieceType Type { get; }
	public PieceColor Color { get; }
	public int Id { get; } // útil en red

	public PieceData(PieceType type, PieceColor color, int id)
	{
		Type = type;
		Color = color;
		Id = id;
	}
}
