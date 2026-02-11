using Godot;
using Chess.Board;

namespace Chess.SceneBoard;

public partial class Piece : Node2D
{
	[Export] public PieceType Type { get; set; }
	[Export] public PieceColor Color { get; set; }
	[Export] public int Id { get; set; }

	public Vector2I Cell { get; set; }

	public void SetTexture(Texture2D tex)
		=> GetNode<Sprite2D>("Sprite2D").Texture = tex;
}
