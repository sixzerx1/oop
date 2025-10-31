using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ReportsDecoratorExample
{
    public interface IReport
    {
        string Generate();
        List<Dictionary<string, object>> GetData();
    }

    public class SalesReport : IReport
    {
        private List<Dictionary<string, object>> _data;

        public SalesReport()
        {
            _data = GenerateFakeSales();
        }

        public string Generate()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Sales Report");
            foreach (var row in _data)
            {
                sb.AppendLine($"{row["id"]} | {((DateTime)row["date"]).ToString("yyyy-MM-dd")} | {row["amount"]} | {row["customer"]}");
            }
            return sb.ToString();
        }

        public List<Dictionary<string, object>> GetData() => _data;

        private List<Dictionary<string, object>> GenerateFakeSales()
        {
            var now = DateTime.Now.Date;
            return new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>{{"id",1},{"date",now.AddDays(-10)}, {"amount",1200.50m}, {"customer","Amir"}, {"product","Coffee"}},
                new Dictionary<string, object>{{"id",2},{"date",now.AddDays(-8)}, {"amount",2500.00m}, {"customer","Ali"}, {"product","Laptop"}},
                new Dictionary<string, object>{{"id",3},{"date",now.AddDays(-5)}, {"amount",75.00m}, {"customer","Emir"}, {"product","Mug"}},
            };
        }
    }

    public class UserReport : IReport
    {
        private List<Dictionary<string, object>> _data;

        public UserReport()
        {
            _data = GenerateFakeUsers();
        }

        public string Generate()
        {
            var sb = new StringBuilder();
            sb.AppendLine("User Report");
            foreach (var row in _data)
            {
                sb.AppendLine($"{row["id"]} | {row["name"]} | {((DateTime)row["signupDate"]).ToString("yyyy-MM-dd")} | {row["country"]} | Purchases: {row["purchases"]}");
            }
            return sb.ToString();
        }

        public List<Dictionary<string, object>> GetData() => _data;

        private List<Dictionary<string, object>> GenerateFakeUsers()
        {
            var now = DateTime.Now.Date;
            return new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>{{"id",1},{"name","Alice"},{"signupDate",now.AddMonths(-14)}, {"country","KZ"}, {"purchases",5}},
                new Dictionary<string, object>{{"id",2},{"name","Bob"},{"signupDate",now.AddMonths(-10)}, {"country","RU"}, {"purchases",2}},
                new Dictionary<string, object>{{"id",3},{"name","Carol"},{"signupDate",now.AddMonths(-6)}, {"country","KZ"}, {"purchases",12}},

            };
        }
    }

    public abstract class ReportDecorator : IReport
    {
        protected IReport _inner;
        protected List<Dictionary<string, object>> _currentData;

        protected ReportDecorator(IReport inner)
        {
            _inner = inner;
            _currentData = inner.GetData().Select(d => new Dictionary<string, object>(d)).ToList();
        }

        public virtual string Generate()
        {
            var sb = new StringBuilder();
            foreach (var row in _currentData)
            {
                sb.AppendLine(string.Join(" | ", row.Select(kv => $"{kv.Key}:{kv.Value}")));
            }
            return sb.ToString();
        }

        public virtual List<Dictionary<string, object>> GetData() => _currentData;
    }

    public class DateFilterDecorator : ReportDecorator
    {
        private readonly string _dateKey;
        private readonly DateTime _from;
        private readonly DateTime _to;

        public DateFilterDecorator(IReport inner, string dateKey, DateTime from, DateTime to) : base(inner)
        {
            _dateKey = dateKey;
            _from = from.Date;
            _to = to.Date;
            Apply();
        }

        private void Apply()
        {
            _currentData = _currentData
                .Where(r => r.ContainsKey(_dateKey) && r[_dateKey] is DateTime dt && dt.Date >= _from && dt.Date <= _to)
                .ToList();
        }

        public override string Generate() => _inner.Generate() + Environment.NewLine + base.Generate();
    }

    public class SortingDecorator : ReportDecorator
    {
        private readonly string _sortKey;
        private readonly bool _descending;

        public SortingDecorator(IReport inner, string sortKey, bool descending = false) : base(inner)
        {
            _sortKey = sortKey;
            _descending = descending;
            Apply();
        }

        private void Apply()
        {
            _currentData = _currentData
                .Where(r => r.ContainsKey(_sortKey))
                .OrderBy(r => r[_sortKey])
                .ToList();

            if (_descending)
                _currentData.Reverse();
        }

        public override string Generate() => _inner.Generate() + Environment.NewLine + base.Generate();
    }

    public class CsvExportDecorator : ReportDecorator
    {
        public CsvExportDecorator(IReport inner) : base(inner)
        {
        }

        public override string Generate()
        {
            if (_currentData == null || _currentData.Count == 0)
                return "CSV (empty)";

            var keys = _currentData.SelectMany(d => d.Keys).Distinct().ToList();
            var sb = new StringBuilder();
            sb.AppendLine(string.Join(",", keys));
            foreach (var row in _currentData)
            {
                var vals = keys.Select(k => EscapeCsv(row.ContainsKey(k) && row[k] != null ? row[k].ToString() : ""));
                sb.AppendLine(string.Join(",", vals));
            }
            return sb.ToString();
        }

        private string EscapeCsv(string s)
        {
            if (s.Contains("\"") || s.Contains(",") || s.Contains("\n"))
                return $"\"{s.Replace("\"", "\"\"")}\"";
            return s;
        }
    }

    public class PdfExportDecorator : ReportDecorator
    {
        public PdfExportDecorator(IReport inner) : base(inner)
        {
        }

        public override string Generate()
        {
            var sb = new StringBuilder();
            sb.AppendLine("----- PDF Document -----");
            foreach (var row in _currentData)
            {
                sb.AppendLine(string.Join(" | ", row.Select(kv => $"{kv.Key}: {kv.Value}")));
            }
            sb.AppendLine("----- End of PDF -----");
            return sb.ToString();
        }
    }

    public class SalesAmountFilterDecorator : ReportDecorator
    {
        private readonly string _amountKey;
        private readonly decimal _minAmount;
        private readonly decimal _maxAmount;
        public SalesAmountFilterDecorator(IReport inner, string amountKey, decimal minAmount = 0, decimal maxAmount = decimal.MaxValue) : base(inner)
        {
            _amountKey = amountKey;
            _minAmount = minAmount;
            _maxAmount = maxAmount;
            Apply();
        }

        private void Apply()
        {
            _currentData = _currentData
                .Where(r => r.ContainsKey(_amountKey) && TryConvertDecimal(r[_amountKey], out var v) && v >= _minAmount && v <= _maxAmount)
                .ToList();
        }

        private bool TryConvertDecimal(object o, out decimal val)
        {
            val = 0;
            if (o is decimal d) { val = d; return true; }
            if (o is double db) { val = (decimal)db; return true; }
            if (o is float f) { val = (decimal)f; return true; }
            if (o is int i) { val = i; return true; }
            if (o is long l) { val = l; return true; }
            if (o is string s && decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed)) { val = parsed; return true; }
            return false;
        }

        public override string Generate() => _inner.Generate() + Environment.NewLine + base.Generate();
    }

    public class UserAttributeFilterDecorator : ReportDecorator
    {
        private readonly string _attrKey;
        private readonly object _value;

        public UserAttributeFilterDecorator(IReport inner, string attrKey, object value) : base(inner)
        {
            _attrKey = attrKey;
            _value = value;
            Apply();
        }

        private void Apply()
        {
            _currentData = _currentData
                .Where(r => r.ContainsKey(_attrKey) && Equals(r[_attrKey], _value))
                .ToList();
        }

        public override string Generate() => _inner.Generate() + Environment.NewLine + base.Generate();
    }

    public class ReportOptions
    {
        public string ReportName { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string SortBy { get; set; }
        public bool SortDesc { get; set; } = false;
        public bool CsvExport { get; set; } = false;
        public bool PdfExport { get; set; } = false;
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public string UserAttrKey { get; set; }
        public object UserAttrValue { get; set; }
    }

    public static class ReportFactory
    {
        public static IReport Build(ReportOptions options)
        {
            IReport report = options.ReportName.ToLower() switch
            {
                "sales" => new SalesReport(),
                "users" => new UserReport(),
                _ => new SalesReport()
            };

            if (options.From.HasValue && options.To.HasValue)
            {
                report = new DateFilterDecorator(report, "date", options.From.Value, options.To.Value);
            }

            if (options.MinAmount.HasValue || options.MaxAmount.HasValue)
            {
                var min = options.MinAmount ?? 0m;
                var max = options.MaxAmount ?? decimal.MaxValue;
                report = new SalesAmountFilterDecorator(report, "amount", min, max);
            }

            if (!string.IsNullOrEmpty(options.UserAttrKey) && options.UserAttrValue != null)
            {
                report = new UserAttributeFilterDecorator(report, options.UserAttrKey, options.UserAttrValue);
            }

            if (!string.IsNullOrEmpty(options.SortBy))
            {
                report = new SortingDecorator(report, options.SortBy, options.SortDesc);
            }

            if (options.CsvExport)
            {
                report = new CsvExportDecorator(report);
            }

            if (options.PdfExport)
            {
                report = new PdfExportDecorator(report);
            }

            return report;
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var opts1 = new ReportOptions
            {
                ReportName = "sales",
                From = DateTime.Now.Date.AddDays(-7),
                To = DateTime.Now.Date,
                SortBy = "amount",
                SortDesc = true,
                CsvExport = true
            };

            var r1 = ReportFactory.Build(opts1);
            Console.WriteLine("=== Report 1 (Sales, last 7 days, sorted by amount desc, CSV) ===");
            Console.WriteLine(r1.Generate());
            Console.WriteLine();

            var opts2 = new ReportOptions
            {
                ReportName = "users",
                UserAttrKey = "country",
                UserAttrValue = "KZ",
                SortBy = "signupDate",
                CsvExport = false,
                PdfExport = true
            };

            var r2 = ReportFactory.Build(opts2);
            Console.WriteLine("=== Report 2 (Users from KZ, sorted by signupDate, PDF) ===");
            Console.WriteLine(r2.Generate());
            Console.WriteLine();

            var opts3 = new ReportOptions
            {
                ReportName = "sales",
                MinAmount = 1000m,
                MaxAmount = 5000m,
                SortBy = "date",
                SortDesc = false
            };

            var r3 = ReportFactory.Build(opts3);
            Console.WriteLine("=== Report 3 (Sales amount between 1000 and 5000, sorted by date) ===");
            Console.WriteLine(r3.Generate());
            Console.WriteLine();

            var opts4 = new ReportOptions
            {
                ReportName = "sales",
                CsvExport = true,
                PdfExport = true
            };

            var r4 = ReportFactory.Build(opts4);
            Console.WriteLine("=== Report 4 (All sales, CSV then PDF decorator applied) ===");
            Console.WriteLine(r4.Generate());
            Console.WriteLine();

            Console.WriteLine("Done");
        }
    }
}
