using Chess;

namespace LichessConsoleBot.Services;

public class GameState
{
    private ChessBoard _board { get; set; }
    private string _movesString { get; set; } = "";

    public GameState()
    {
        _board = new ChessBoard();
    }

    public (bool isMoveAccepted, PieceColor whosTurnIsIt, string fen) ChangeMoves(string moves)
    {
        if(moves.Length <= _movesString.Length)
        {
            return (false, _board.Turn, _board.ToFen());
        }
        else
        {
            _movesString = moves;
            var movesArray = moves.Split(" ");
            // I can optimize this to not make all these moves later
            _board = new ChessBoard();
            foreach (var move in movesArray)
            {
                if(move.Length == 0)
                {
                    continue;
                }
                _board.Move(move);
            }
            return (true, _board.Turn, _board.ToFen());
        }
    }

    internal PieceColor GetWhosTurnIsIt()
    {
        return _board.Turn;
    }
}
