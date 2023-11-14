using Chess;

namespace LichessConsoleBot.Services;

public delegate void MoveFound(string move);
public class ChessAi : IChessAi
{
    public event MoveFound? MoveFound;
    public async Task FindMoveForPosition(string fen)
    {
        if (fen == "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")
        {
            MoveFound?.Invoke(GetStartingMove());
        }

        var board = ChessBoard.LoadFromFen(fen);
        var possibleMoves = board.Moves();
        var r = new Random();
        var randomMove = possibleMoves[r.Next(possibleMoves.Count())];
        MoveFound?.Invoke(randomMove.OriginalPosition.ToString() + randomMove.NewPosition.ToString());
    }

    private string GetStartingMove()
    {
        var r = new Random();
        var val = r.NextDouble();
        if (val < 0.50)
        {
            return "e2e4";
        }
        else if (val < 0.80)
        {
            return "d2d4";
        }
        else if (val < 0.9)
        {
            return "c2c4";
        }
        else if (val < 0.96)
        {
            return "f2f4";
        }
        else
        {
            return "g2g3";
        }
    }
}


public interface IChessAi
{
    public event MoveFound? MoveFound;
    public Task FindMoveForPosition(string fen);
}