namespace BE
{
    public class ChartDataPoint
    {
        public string Label { get; set; }           // مثلاً: "فروردین" یا "1403/03/20"
        public decimal Income { get; set; }         // درآمد کل
        public decimal Cost { get; set; }           // هزینه کل
        public decimal Profit => Income - Cost;     // سود خالص
    }

}
