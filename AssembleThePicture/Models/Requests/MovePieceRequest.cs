namespace AssembleThePicture.Models.Requests;

public class MovePieceRequest
{
    public int Piece1Row { get; set; }
    
    public int Piece1Col { get; set; }
    
    public int Piece2Row { get; set; }
    
    public int Piece2Col { get; set; }
}