namespace AssembleThePicture.Models.Requests;

public class MovePieceRequest
{
    public int CurrentRow { get; set; }
    
    public int CurrentCol { get; set; }
    
    public int NewRow { get; set; }
    
    public int NewCol { get; set; }
}