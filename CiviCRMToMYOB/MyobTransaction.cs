namespace CiviCRMToMYOB
{
    public class MyobTransaction    
    {
        public string JournalNumber { get; set; }
        public string Date { get; set; }
        public string Memo { get; set; }
        public string AccountNumber { get; set; }
        public string IsCredit { get; set; }
        public string Amount { get; set; }
        public string Job { get; set; }
        public string AllocationMemo { get; set; }
        public string Category { get; set; }
        public string IsYearEndAdjustment { get; set; }
    }
}