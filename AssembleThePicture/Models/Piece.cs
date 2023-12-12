namespace AssembleThePicture.Models;

public class Piece
{
    public byte[] ImageData { get; set; }

    private int _rotate;

    public int Rotate
    {
        get => _rotate;
        set => _rotate = (_rotate + value) % 360;
    }

    public int Width { get; set; }
    
    public int Height { get; set; }
    
    public int CurrentRow { get; set; }
    
    public int CurrentCol { get; set; }

    public int RightRow { get; set; }
    
    public int RightCol { get; set; }

    public bool IsOnRightPlace => CurrentRow == RightRow && CurrentCol == RightCol && Rotate == 0;
}