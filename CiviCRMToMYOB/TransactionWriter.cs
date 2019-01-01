using System.Collections.Generic;
using System.IO;

namespace CiviCRMToMYOB
{
    public class TransactionWriter : ITransactionWriter
    {
        public TextWriter WriteToString(TextWriter writer, List<List<MyobTransaction>> transactions)
        {
            writer.WriteLine("{},,,,,,,,,");
            writer.WriteLine("Journal Number,Date,Memo,Account Number,Is Credit,Amount,Job,Allocation Memo,Category,Is Year-End Adjustment");

            var firstTransaction = true;

            foreach (var transaction in transactions)
            {
                if (!firstTransaction)
                {
                    writer.WriteLine(",,,,,,,,,");
                }

                foreach (var row in transaction)
                {
                    writer.WriteLine($"{row.JournalNumber},{row.Date},{row.Memo},{row.AccountNumber},{row.IsCredit},{row.Amount},{row.Job},{row.AllocationMemo},{row.Category},{row.IsYearEndAdjustment}");
                }

                firstTransaction = false;
            }

            return writer;
        }
    }
}