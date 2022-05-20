using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFG_Tracker.DatabaseTable
{
    public class BarCodeSettings
    {
        public bool PrinterType { get; set; }
        public int? PrinterDPI { get; set; }
        public float PageWidth { get; set; }
        public float PageHeight { get; set; }
        public float StickerWidth { get; set; }
        public float StickerHeight { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public float CellPadingLeft { get; set; }
        public float CellPadingTop { get; set; }
        public float CellPadingRight { get; set; }
        public float CellPadingBottom { get; set; }
        public float PageMarginLeft { get; set; }
        public float PageMarginTop { get; set; }
        public float PageMarginRight { get; set; }
        public float PageMarginBottom { get; set; }
        public bool AcrossThenDown { get; set; }
        public float VerticalGap { get; set; }
        public float HorizantalGap { get; set; }
        public bool PageOriantation { get; set; }
        public bool? BarOrQR { get; set; }
        public string Plant_Id { get; set; }
        public string Company_Id { get; set; }
    }
}
