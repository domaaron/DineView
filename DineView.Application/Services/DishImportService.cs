using CsvHelper;
using CsvHelper.Configuration;
using DineView.Application.infrastructure;
using DineView.Application.models;
using ExcelDataReader;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DineView.Application.Services
{
    public class DishImportService
    {
        private class CsvRow
        {
            public int Id { get; set; }
            public string Name { get; set; } = default!;
            public string Description { get; set; } = default!;
            public float Calories { get; set; } = default!;
            public TimeSpan? PreparationTime { get; set; }
            public string CategoryDesignation { get; set; } = default!;
        }

        private class NotNullStringConverter : CsvHelper.TypeConversion.StringConverter
        {
            public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
            {
                if (string.IsNullOrEmpty(text))
                {
                    throw new CsvHelperException(row.Context);
                }
                return base.ConvertFromString(text, row, memberMapData);
            }
        }

        private class CsvRowMap : ClassMap<CsvRow>
        {
            public CsvRowMap()
            {
                Map(row => row.Id).Index(0);
                Map(row => row.Name).Name("Name").TypeConverter<NotNullStringConverter>();
                Map(row => row.Description).Name("Description").TypeConverter<NotNullStringConverter>();
                Map(row => row.Calories).Convert(args =>
                {
                    string? val = args.Row["Calories"];
                    if (string.IsNullOrEmpty(val)) { return 0f; }
                    return float.Parse(val.AsSpan(), CultureInfo.InvariantCulture);
                });
                Map(row => row.PreparationTime).Convert(args =>
                {
                    string? val = args.Row["PreparationTime"];
                    if (string.IsNullOrEmpty(val)) { return null; }
                    return TimeSpan.ParseExact(val, @"hh\:mm\:ss", CultureInfo.InvariantCulture);
                });
                Map(row => row.CategoryDesignation).Name("CategoryDesignation").TypeConverter<NotNullStringConverter>();
            }
        }

        private readonly DineContext _db;

        public DishImportService(DineContext db)
        {
            _db = db;
        }

        public (bool success, string message) LoadCsv(Stream stream)
        {
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = "\t",
                NewLine = "\r\n",
                ReadingExceptionOccurred = (context) => false
            };

            using var reader = new StreamReader(stream, new UTF8Encoding(false));
            using var csv = new CsvReader(reader, csvConfig);
            csv.Context.RegisterClassMap<CsvRowMap>();

            try
            {
                var records = csv.GetRecords<CsvRow>().ToList();
                return WriteToDatabase(records);
            }
            catch (CsvHelperException ex)
            {
                return (false, $"Error while reading the line {ex.Context.Parser.Row}: {ex.Message}");
            }
        }

        public (bool success, string message) LoadExcel(Stream stream, int maxRows = 1000)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            reader.Read();
            var csvRows = new List<CsvRow>(1024);

            int rowNumber = 0;
            while (reader.Read() && rowNumber++ < maxRows)
            {
                if (reader.FieldCount < 5) { break; }
                if (reader.IsDBNull(0)) { break; }

                try
                {
                    csvRows.Add(new CsvRow
                    {
                        Id = reader.IsDBNull(0) ? throw new ApplicationException("Invalid Id") : (int)reader.GetDouble(0),
                        Name = reader.IsDBNull(1) ? throw new ApplicationException("Invalid dish name") : reader.GetString(1),
                        Description = reader.IsDBNull(1) ? throw new ApplicationException("Invalid description") : reader.GetString(2),
                        Calories = reader.IsDBNull(3) ? 0f : reader.GetFloat(3),
                        PreparationTime = (TimeSpan?)(reader.IsDBNull(4) ? null : reader.GetValue(4)),
                        CategoryDesignation = reader.IsDBNull(5) ? throw new ApplicationException("Invalid dish category") : reader.GetString(5)
                    });
                }
                catch { }
            }

            return WriteToDatabase(csvRows);
        }

        private (bool success, string message) WriteToDatabase(IEnumerable<CsvRow> csvRows)
        {
            var existingIDs = _db.Dishes.Select(d => d.Id).ToHashSet();
            var existingCategories = _db.Categories.Select(c => c.Designation).ToHashSet();

            var newCategories = csvRows
                .Where(c => !existingCategories.Contains(c.CategoryDesignation))
                .GroupBy(c => c.CategoryDesignation)
                .Select(g => new Category(designation: g.Key));
            _db.Categories.AddRange(newCategories);

            try
            {
                _db.SaveChanges();
            }
            catch(DbUpdateException ex)
            {
                return (false, ex.InnerException?.Message ?? ex.Message);
            }

            var categories = _db.Categories.ToDictionary(c => c.Designation, c => c);
            var newDishes = csvRows
                .Where(r => !existingIDs.Contains(r.Id))
                .Select(r => new Dish(
                    name: r.Name,
                    description: r.Description,
                    calories: r.Calories,
                    preparationTime: r.PreparationTime,
                    category: categories[r.CategoryDesignation]
                    ));
            _db.Dishes.AddRange(newDishes);

            try
            {
                var count = _db.SaveChanges();
                return (true, $"Imported {count} Dishes.");
            }
            catch (DbUpdateException ex)
            {
                return (false, ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}
