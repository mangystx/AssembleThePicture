namespace AssembleThePicture.Models;

public class Piece
{
    public byte[] ImageData { get; set; }
    
    public int CurrentRow { get; set; }
    
    public int CurrentCol { get; set; }

    public int RightRow { get; set; }
    
    public int RightCol { get; set; }

    public bool IsOnRightPlace => CurrentRow == RightRow && CurrentCol == RightCol;
}