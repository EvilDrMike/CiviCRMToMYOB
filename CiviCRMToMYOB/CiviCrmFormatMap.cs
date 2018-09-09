using CsvHelper.Configuration;

namespace CiviCRMToMYOB
{
    public sealed class CiviCrmFormatMap : ClassMap<CiviCrmFormat>
    {
        public CiviCrmFormatMap()
        {
            Map(m => m.FinancialAccountCodeDebit).Name("Financial Account Code - Debit");
            Map(m => m.FinancialAccountCodeCredit).Name("Financial Account Code - Credit");
            Map(m => m.Date).Name("Transaction Date");
            Map(m => m.TransNumber).Name("Trans #");
            Map(m => m.Amount);
            Map(m => m.ContactName).Name("Contact Name");
        }
    }
}