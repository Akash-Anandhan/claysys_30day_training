using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ShoppingCartAPI.Models;
using System.Globalization;
using ShoppingCartAPI.DTOs;

namespace ShoppingCartAPI.Services
{
    public partial class ProductsService
    {
        public async Task<int> ImportProductsFromExcelAsync(Microsoft.AspNetCore.Http.IFormFile file)
        {
            _logger.LogInformation("Starting Excel import process.");
            
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Import failed: File is empty or null.");
                throw new ArgumentException("File is empty or null.");
            }

            var addedCount = 0;
            var defaultCategoryId = await _context.Categories.Select(c => c.Id).FirstOrDefaultAsync();

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null) 
                        {
                            _logger.LogWarning("Import failed: No worksheets found in the Excel file.");
                            return 0;
                        }

                        var rowCount = worksheet.Dimension?.Rows ?? 0;

                        if (rowCount < 2)
                        {
                            _logger.LogWarning("Excel file has no data rows.");
                            return 0;
                        }

                        for (int row = 2; row <= rowCount; row++) // Skip header
                        {
                            var name = worksheet.Cells[row, 1].Value?.ToString();
                            
                            if (string.IsNullOrWhiteSpace(name)) continue;

                            // Avoid duplicate
                            var exists = await _context.Products.AnyAsync(p => p.Name == name);
                            if (exists) continue;

                            decimal.TryParse(worksheet.Cells[row, 2].Value?.ToString(), out decimal price);
                            int.TryParse(worksheet.Cells[row, 3].Value?.ToString(), out int stock);

                            var product = new Product
                            {
                                Name = name,
                                Price = price,
                                Stock = stock,
                                CategoryId = defaultCategoryId > 0 ? defaultCategoryId : 1,
                                Description = "Imported from Excel",
                                ImageUrl = ""
                            };

                            _context.Products.Add(product);
                            addedCount++;
                        }
                    }
                }

                if (addedCount > 0)
                {
                    await _context.SaveChangesAsync();
                    _cache.Remove("AllProducts"); // invalidate cache
                    _logger.LogInformation($"Successfully imported {addedCount} products from Excel.");
                }
                else
                {
                    _logger.LogInformation("Excel import finished but no new products were added.");
                }

                return addedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during Excel import.");
                throw;
            }
        }

        public async Task<int> ImportProductsFromCsvAsync(Microsoft.AspNetCore.Http.IFormFile file)
        {
            _logger.LogInformation("Starting CSV import process.");

            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Import failed: File is empty or null.");
                throw new ArgumentException("File is empty or null.");
            }

            var addedCount = 0;
            var defaultCategoryId = await _context.Categories.Select(c => c.Id).FirstOrDefaultAsync();

            try
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    MissingFieldFound = null,
                    HeaderValidated = null
                }))
                {
                    var records = csv.GetRecords<ProductCsvRecord>().ToList();

                    foreach (var record in records)
                    {
                        if (string.IsNullOrWhiteSpace(record.Name)) continue;

                        var exists = await _context.Products.AnyAsync(p => p.Name == record.Name);
                        if (exists) continue;

                        var product = new Product
                        {
                            Name = record.Name,
                            Price = record.Price,
                            Stock = record.Stock,
                            CategoryId = defaultCategoryId > 0 ? defaultCategoryId : 1,
                            Description = "Imported from CSV",
                            ImageUrl = ""
                        };

                        _context.Products.Add(product);
                        addedCount++;
                    }
                }

                if (addedCount > 0)
                {
                    await _context.SaveChangesAsync();
                    _cache.Remove("AllProducts");
                    _logger.LogInformation($"Successfully imported {addedCount} products from CSV.");
                }
                else
                {
                    _logger.LogInformation("CSV import finished but no new products were added.");
                }

                return addedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during CSV import.");
                throw;
            }
        }

        public async Task<byte[]> ExportProductsToExcelAsync()
        {
            _logger.LogInformation("Starting Excel export process.");

            try
            {
               
                var products = await _context.Products
                    .AsNoTracking()
                    .Select(p => new
                    {
                        p.Id,
                        p.Name,
                        p.Description,
                        p.Price,
                        p.Stock
                    })
                    .ToListAsync();

                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Products");

                var headers = new[] { "Id", "Name", "Description", "Price", "Stock" };

                for (int col = 0; col < headers.Length; col++)
                {
                    worksheet.Cells[1, col + 1].Value = headers[col];
                }

                using (var headerRange = worksheet.Cells[1, 1, 1, headers.Length])
                {
                    headerRange.Style.Font.Bold = true;
                }

                for (int i = 0; i < products.Count; i++)
                {
                    var p = products[i];
                    int row = i + 2;

                    worksheet.Cells[row, 1].Value = p.Id;
                    worksheet.Cells[row, 2].Value = p.Name;
                    worksheet.Cells[row, 3].Value = p.Description;
                    worksheet.Cells[row, 4].Value = p.Price;
                    worksheet.Cells[row, 5].Value = p.Stock;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                _logger.LogInformation("Successfully exported {Count} products to Excel.", products.Count);

                return package.GetAsByteArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during Excel export.");
                throw;
            }
        }

        public async Task<byte[]> ExportProductsToCsvAsync()
        {
            _logger.LogInformation("Starting CSV export process.");

            try
            {
                var products = await _context.Products.Select(p => new ProductCsvRecord
                {
                    Name = p.Name,
                    Price = p.Price,
                    Stock = p.Stock
                }).ToListAsync();

                using (var memoryStream = new MemoryStream())
                {
                    using (var writer = new StreamWriter(memoryStream))
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        csv.WriteRecords(products);
                    }
                    _logger.LogInformation($"Successfully exported {products.Count} products to CSV.");
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during CSV export.");
                throw;
            }
        }
    }
}
