
using Services.Services.Enums;

namespace Services.Services;

public class BookshelfParams
{
    public PurchaseFilterEnum filter { get; set; }
    public SortEnum sort { get; set; }
    public DirectionEnum direction { get; set; }

    public string? search { get; set; }
    public DigitalityEnum digitality { get; set; }
    public ReadingEnum reading { get; set; }

}
