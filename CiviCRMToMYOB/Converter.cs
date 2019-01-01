using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CiviCRMToMYOB
{
    public class Converter : IConverter
    {
        private readonly List<string> _gstAccounts;
        private readonly List<string> _nonGstAccounts;
        private readonly List<string> _gstForCreditAccounts;

        public Converter(List<string> gstAccounts, List<string> nonGstAccounts, List<string> gstForCreditAccounts)
        {
            _gstAccounts = gstAccounts;
            _nonGstAccounts = nonGstAccounts;
            _gstForCreditAccounts = gstForCreditAccounts;
        }

        public List<List<MyobTransaction>> Convert(List<CiviCrmFormat> transactions,
            List<CiviCrmFormat> gstTransactions)
        {
            var converted = new List<List<MyobTransaction>>();

            foreach (var transaction in transactions.GroupBy(t => t.ContributionId))
            {
                converted.Add(BuildMyobTransaction(gstTransactions, transaction));
            }

            return converted;
        }

        private List<MyobTransaction> BuildMyobTransaction(List<CiviCrmFormat> gstTransactions, IGrouping<string, CiviCrmFormat> transaction)
        {
            var myobTran = new List<MyobTransaction>();
            var hasWrittenGstRecordForCurrentTransaction = false;

            foreach (var lineItem in transaction)
            {
                var gstLineItem = FindMatchingGstTransaction(gstTransactions, lineItem);
                myobTran.Add(ToDebitLine(lineItem, gstLineItem));
                if (gstLineItem != null && !hasWrittenGstRecordForCurrentTransaction)
                {
                    myobTran.Add(ToGstLine(gstLineItem));
                    hasWrittenGstRecordForCurrentTransaction = true;
                }

                myobTran.Add(ToCreditLine(lineItem, gstLineItem));
            }

            return myobTran;
        }

        private CiviCrmFormat FindMatchingGstTransaction(List<CiviCrmFormat> gstTransactions, CiviCrmFormat transaction)
        {
            var gstTransaction = _gstAccounts.Contains(transaction.FinancialAccountCodeDebit)
                ? gstTransactions.FirstOrDefault(g =>
                    g.ContributionId == transaction.ContributionId && _gstAccounts.Contains(transaction.FinancialAccountCodeDebit))
                : null;
            return gstTransaction;
        }

        private MyobTransaction ToCreditLine(CiviCrmFormat transaction, CiviCrmFormat gst)
        {
            return new MyobTransaction
            {
                Date = transaction.Date,
                Memo = transaction.ContributionId,
                AccountNumber = transaction.FinancialAccountCodeCredit,
                IsCredit = "Y",
                Amount = CombinedCreditTransactionAmount(transaction, gst)
            };
        }

        private MyobTransaction ToGstLine(CiviCrmFormat item)
        {
            return new MyobTransaction
            {
                Date = item.Date,
                Memo = item.ContributionId,
                AccountNumber = item.FinancialAccountCodeCredit,
                IsCredit = "Y",
                Amount = item.Amount
            };
        }

        private MyobTransaction ToDebitLine(CiviCrmFormat item, CiviCrmFormat gst)
        {
            return new MyobTransaction
            {
                Date = item.Date,
                Memo = item.ContributionId,
                AccountNumber = item.FinancialAccountCodeDebit,
                IsCredit = "N",
                Amount = CombinedTransactionAmount(item, gst)
            };
        }

        private string CombinedCreditTransactionAmount(CiviCrmFormat transaction, CiviCrmFormat gst)
        {
            if (gst == null  || !_gstForCreditAccounts.Contains(transaction.FinancialAccountCodeCredit))
            {
                return transaction.Amount;
            }

            var amount = decimal.Parse(transaction.Amount, NumberStyles.Currency);

            var intAmount = (int) amount;
            if (amount == intAmount)
            {
                return $"{amount:C2}";
            }

            var total = decimal.Parse(gst.Amount, NumberStyles.Currency) +
                        amount;

            return $"{total:C2}";
        }

        private string CombinedTransactionAmount(CiviCrmFormat transaction, CiviCrmFormat gst)
        {
            if (gst == null || 
                (_gstAccounts.Contains(transaction.FinancialAccountCodeDebit) && _nonGstAccounts.Contains(transaction.FinancialAccountCodeCredit)) || 
                string.Compare(transaction.FinancialType, "Donation", StringComparison.InvariantCultureIgnoreCase) == 0
                )
            {
                return transaction.Amount;
            }

            var total = decimal.Parse(gst.Amount, NumberStyles.Currency) +
                        decimal.Parse(transaction.Amount, NumberStyles.Currency);

            return $"{total:C2}";
        }
    }
}
