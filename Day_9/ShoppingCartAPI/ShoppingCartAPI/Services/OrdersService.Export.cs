using CsvHelper;
using OfficeOpenXml;
using ShoppingCartAPI.DTOs;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ShoppingCartAPI.Services
{
    public partial class OrdersService
    {
        public async Task<byte[]> ExportOrdersToExcelAsync()
        {
            var orders = await GetOrdersAsync();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Orders");

            worksheet.Cells[1, 1].Value = "Order ID";
            worksheet.Cells[1, 2].Value = "Date";
            worksheet.Cells[1, 3].Value = "Status";
            worksheet.Cells[1, 4].Value = "Total Amount";
            worksheet.Cells[1, 5].Value = "Shipping Address";
            worksheet.Cells[1, 6].Value = "Payment Type";

            using (var range = worksheet.Cells[1, 1, 1, 6])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            int row = 2;
            foreach (var order in orders)
            {
                worksheet.Cells[row, 1].Value = order.Id;
                worksheet.Cells[row, 2].Value = order.OrderDate;
                worksheet.Cells[row, 3].Value = order.Status;
                worksheet.Cells[row, 4].Value = order.TotalAmount;
                worksheet.Cells[row, 5].Value = order.ShippingAddress;
                worksheet.Cells[row, 6].Value = order.PaymentType;
                row++;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            return await package.GetAsByteArrayAsync();
        }

        public async Task<byte[]> ExportOrdersToCsvAsync()
        {
            var orders = await GetOrdersAsync();

            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            var exportData = orders.Select(o => new
            {
                OrderID = o.Id,
                Date = o.OrderDate,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                ShippingAddress = o.ShippingAddress,
                PaymentType = o.PaymentType
            }).ToList();

            csv.WriteRecords(exportData);
            writer.Flush();
            return memoryStream.ToArray();
        }
    }
}
