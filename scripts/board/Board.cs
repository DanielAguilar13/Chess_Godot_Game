using Godot;
using System.Collections.Generic;
using Chess.Board;
using Chess.Core;
using Chess.Rules;
using Chess.SceneBoard;

namespace Chess.SceneBoard;

public partial class Board : Node2D
{
	[Export] public int CellSize = 80;
	[Export] public Vector2 BoardOrigin = new(0, 0);
	[Export] public PackedScene PieceScene;

	private readonly Dictionary<int, Piece> _pieceNodesById = new();
	private readonly Dictionary<string, Texture2D> _texCache = new();

	private ChessMatch _match = new();
	private Vector2I? _selectedCell = null;
	private List<Vector2I> _selectedMoves = new();

	public override void _Ready()
	{
		_match.ResetToInitial();
		RebuildAllPieces();
	}

	public override void _UnhandledInput(InputEvent e)
	{
		if (e is not InputEventMouseButton mb) return;
		if (!mb.Pressed || mb.ButtonIndex != MouseButton.Left) return;

		var cell = ChessUtils.WorldToCell(GetGlobalMousePosition(), BoardOrigin, CellSize);
		if (!ChessUtils.InBounds(cell)) return;

		HandleClick(cell);
	}

	private void HandleClick(Vector2I cell)
	{
		var piece = _match.Board[cell.X, cell.Y];

		// no hay selección
		if (_selectedCell == null)
		{
			if (piece == null) return;
			if (piece.Value.Color != _match.Turn) return;

			Select(cell, piece.Value);
			return;
		}

		// si clic en otra pieza del mismo color, cambiar selección
		if (piece != null && piece.Value.Color == _match.Turn)
		{
			Select(cell, piece.Value);
			return;
		}

		// intentar mover a destino
		if (_selectedMoves.Contains(cell))
		{
			var move = new Move(_selectedCell.Value, cell);

			// Captura sonido: si hay pieza en destino
			bool capture = _match.Board[cell.X, cell.Y] != null;

			_match.ApplyMove(move);
			ApplyMoveVisual(move);

			var audio = GetNodeOrNull<Node>("/root/Audio") as dynamic;
			if (audio != null)
				audio.PlaySfx(capture ? "res://assets/audio/sfx/capture.wav" : "res://assets/audio/sfx/move.wav");
			
			ClearSelection();
		}
		else
		{
			// movimiento ilegal
			var audio = GetNodeOrNull<Node>("/root/Audio") as dynamic;
			audio?.PlaySfx("res://assets/audio/sfx/illegal.wav");
			ClearSelection();
		}
	}

	private void Select(Vector2I cell, PieceData piece)
	{
		ClearSelection();

		_selectedCell = cell;
		_selectedMoves = MoveGenerator.GetMoves(piece, cell, _match.Board);

		// (Opcional) highlight: por ahora escalamos la pieza seleccionada
		if (_match.Board[cell.X, cell.Y] is PieceData p)
		{
			if (_pieceNodesById.TryGetValue(p.Id, out var node))
				node.Scale = new Vector2(1.08f, 1.08f);
		}
	}

	private void ClearSelection()
	{
		// reset scale de todas (simple, escolar pro)
		foreach (var kv in _pieceNodesById)
			kv.Value.Scale = Vector2.One;

		_selectedCell = null;
		_selectedMoves.Clear();
	}

	// ----- Visuals -----
	private void RebuildAllPieces()
	{
		foreach (var child in GetChildren())
			if (child is Piece) child.QueueFree();

		_pieceNodesById.Clear();

		for (int x = 0; x < 8; x++)
		for (int y = 0; y < 8; y++)
		{
			var pd = _match.Board[x, y];
			if (pd == null) continue;

			SpawnPieceNode(pd.Value, new Vector2I(x, y));
		}
	}

	private void SpawnPieceNode(PieceData pd, Vector2I cell)
	{
		var node = PieceScene.Instantiate<Piece>();
		node.Type = pd.Type;
		node.Color = pd.Color;
		node.Id = pd.Id;
		node.Cell = cell;

		node.SetTexture(LoadTextureFor(pd));
		AddChild(node);

		node.GlobalPosition = ChessUtils.CellToWorld(cell, BoardOrigin, CellSize) + new Vector2(CellSize / 2f, CellSize / 2f);

		_pieceNodesById[pd.Id] = node;
	}

	private void ApplyMoveVisual(Move move)
	{
		var from = move.From;
		var to = move.To;

		// Si capturamos: eliminar el nodo de la pieza capturada (si existía)
		// Como ya actualizamos Board en _match, la capturada ya no está ahí.
		// Entonces buscamos qué nodo estaba en 'to' antes: lo detectamos por colisión de posiciones.
		// Para simple/pro: hacemos limpieza de nodos que quedaron sin estar en el board.
		SyncNodesWithBoard();

		// Mover el nodo del que está en 'to' (después del apply)
		var pd = _match.Board[to.X, to.Y];
		if (pd == null) return;

		if (_pieceNodesById.TryGetValue(pd.Value.Id, out var moverNode))
		{
			moverNode.Cell = to;
			moverNode.GlobalPosition = ChessUtils.CellToWorld(to, BoardOrigin, CellSize) + new Vector2(CellSize / 2f, CellSize / 2f);
		}
	}

	private void SyncNodesWithBoard()
	{
		// Construir set de IDs existentes en el modelo
		var aliveIds = new HashSet<int>();
		for (int x = 0; x < 8; x++)
		for (int y = 0; y < 8; y++)
			if (_match.Board[x, y] is PieceData pd) aliveIds.Add(pd.Id);

		// Eliminar nodos que ya no existen (capturados)
		var toRemove = new List<int>();
		foreach (var (id, node) in _pieceNodesById)
		{
			if (!aliveIds.Contains(id))
			{
				node.QueueFree();
				toRemove.Add(id);
			}
		}
		foreach (var id in toRemove) _pieceNodesById.Remove(id);
	}

	private Texture2D LoadTextureFor(PieceData pd)
	{
		var key = $"{pd.Color.ToString().ToLower()}_{pd.Type.ToString().ToLower()}"; // black_queen
		if (_texCache.TryGetValue(key, out var t)) return t;

		var path = $"res://assets/pieces/{key}.png";
		var tex = GD.Load<Texture2D>(path);
		_texCache[key] = tex;
		return tex;
	}
}
