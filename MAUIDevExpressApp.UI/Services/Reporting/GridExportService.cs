//using MAUIDevExpressApp.Shared.DTOs;
//using Syncfusion.Maui.DataGrid;
//using Syncfusion.Maui.DataGrid.Exporting;
//using Syncfusion.Pdf;
//using Syncfusion.Pdf.Interactive;
//using Syncfusion.XlsIO;
//using System;
//using System.Collections.ObjectModel;
//using System.IO;
//using System.Threading.Tasks;

//namespace MAUIDevExpressApp.UI.Services.Reporting
//{
//    public class GridExportService
//    {
//        public async Task<string> ExportToExcelAsync(SfDataGrid dataGrid)
//        {
//            try
//            {
//                // Create DataGrid exporter
//                var exporter = new DataGridExcelExportingController();

//                // Configure export options
//                var options = new DataGridExcelExportingOption();
//                options.ExportMode = ExportMode.Value;
//                options.CanExportColumnWidth = true;
//                options.CanExportRowHeight = true;
//                options.DefaultColumnWidth = 100;
//                options.DefaultRowHeight = 20;
//                options.ExcelVersion = ExcelVersion.Excel2016;
//                options.StartRowIndex = 2;
//                options.StartColumnIndex = 1;
//                options.CanExportAllPages = true;

//                // Add custom styling
//                exporter.CellExporting += (sender, e) =>
//                {
//                    // Style header row
//                    if (e.CellType == ExportCellType.HeaderCell)
//                    {
//                        e.BackgroundColor = System.Drawing.Color.LightGray;
//                        e.Style.FontAttributes = FontAttributes.Bold;
//                    }

//                    // Style alternate rows
//                    if (e.CellType == ExportCellType.RecordCell && e.RowIndex % 2 == 0)
//                    {
//                        e.Style.BackgroundColor = System.Drawing.Color.AliceBlue;
//                    }

//                    // Special handling for status column
//                    if (e.CellType == ExportCellType.RecordCell && e.ColumnName == "StatusDisplay")
//                    {
//                        var status = e.CellValue?.ToString();
//                        if (status == "Active")
//                        {
//                            e.Style.BackgroundColor = System.Drawing.Color.LightGreen;
//                        }
//                        else if (status == "Inactive")
//                        {
//                            e.Style.BackgroundColor = System.Drawing.Color.LightPink;
//                        }
//                    }
//                };

//                // Handle workbook creation
//                exporter.WorkbookCreated += (sender, e) =>
//                {
//                    // Add title
//                    e.Workbook.Worksheets[0].Range["A1"].Text = "Permissions Report";
//                    e.Workbook.Worksheets[0].Range["A1"].CellStyle.Font.Bold = true;
//                    e.Workbook.Worksheets[0].Range["A1"].CellStyle.Font.Size = 14;

//                    // Add date
//                    e.Workbook.Worksheets[0].Range["A2"].Text = $"Generated: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}";
//                };

//                // Export to Excel
//                var excelEngine = exporter.ExportToExcel(dataGrid, options);
//                var workbook = excelEngine.Excel.Workbooks[0];

//                // Save the workbook
//                string filePath = Path.Combine(FileSystem.CacheDirectory, "PermissionsReport.xlsx");
//                using (var fileStream = new FileStream(filePath, FileMode.Create))
//                {
//                    workbook.SaveAs(fileStream);
//                }

//                return filePath;
//            }
//            catch (Exception ex)
//            {
//                throw new Exception($"Excel export failed: {ex.Message}", ex);
//            }
//        }

//        public async Task<string> ExportToPdfAsync(SfDataGrid dataGrid)
//        {
//            try
//            {
//                // Create DataGrid exporter
//                var exporter = new DataGridPdfExportingController();

//                // Configure export options
//                var options = new DataGridPdfExportingOption();
//                options.ExportMode = ExportMode.Value;
//                options.ExportColumnWidth = true;
//                options.FitAllColumnsInOnePage = true;
//                options.RepeatHeaders = true;
//                options.DefaultColumnWidth = 100;
//                options.DefaultRowHeight = 20;
//                options.StartRowIndex = 2;
//                options.StartColumnIndex = 1;
//                options.ExportAllPages = true;

//                // Set PDF document properties
//                options.DocumentOptions = new PdfDocumentOptions()
//                {
//                    PageOrientation = PdfPageOrientation.Landscape,
//                    PageSize = PdfPageSize.A4
//                };

//                // Add title and date
//                exporter.HeaderFooterExporting += (sender, e) =>
//                {
//                    // Add title
//                    var titleStyle = new PdfGridCellStyle();
//                    titleStyle.BackgroundBrush = PdfBrushes.White;
//                    titleStyle.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 18, PdfFontStyle.Bold);
//                    titleStyle.TextBrush = PdfBrushes.DarkBlue;
//                    titleStyle.StringFormat = new PdfStringFormat() { Alignment = PdfTextAlignment.Center };

//                    e.PdfDocument.DrawString("Permissions Report", titleStyle.Font, titleStyle.TextBrush,
//                        new Syncfusion.Drawing.PointF(e.PdfPage.GetClientSize().Width / 2, 20));

//                    // Add date
//                    var dateStyle = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
//                    e.PdfDocument.DrawString($"Generated: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}", dateStyle,
//                        PdfBrushes.Black, new Syncfusion.Drawing.PointF(e.PdfPage.GetClientSize().Width - 250, 50));

//                    // Add footer
//                    e.PdfDocument.DrawString("© " + DateTime.Now.Year + " - Permissions Report",
//                        new PdfStandardFont(PdfFontFamily.Helvetica, 8),
//                        PdfBrushes.Gray,
//                        new Syncfusion.Drawing.PointF(e.PdfPage.GetClientSize().Width / 2 - 70, e.PdfPage.GetClientSize().Height - 30));
//                };

//                // Custom styling
//                exporter.CellExporting += (sender, e) =>
//                {
//                    // Style header row
//                    if (e.CellType == ExportCellType.HeaderCell)
//                    {
//                        e.PdfGridCell.Style.BackgroundBrush = PdfBrushes.LightGray;
//                        e.PdfGridCell.Style.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold);
//                    }

//                    // Style alternate rows
//                    if (e.CellType == ExportCellType.RecordCell && e.RowIndex % 2 == 0)
//                    {
//                        e.PdfGridCell.Style.BackgroundBrush = new PdfSolidBrush(Syncfusion.Drawing.Color.FromArgb(240, 248, 255)); // AliceBlue
//                    }

//                    // Special handling for status column
//                    if (e.CellType == ExportCellType.RecordCell && e.ColumnName == "StatusDisplay")
//                    {
//                        var status = e.CellValue?.ToString();
//                        if (status == "Active")
//                        {
//                            e.PdfGridCell.Style.BackgroundBrush = new PdfSolidBrush(Syncfusion.Drawing.Color.FromArgb(144, 238, 144)); // Light green
//                        }
//                        else if (status == "Inactive")
//                        {
//                            e.PdfGridCell.Style.BackgroundBrush = new PdfSolidBrush(Syncfusion.Drawing.Color.FromArgb(255, 182, 193)); // Light pink
//                        }
//                    }
//                };

//                // Export to PDF
//                var document = exporter.ExportToPdf(dataGrid, options);

//                // Save the document
//                string filePath = Path.Combine(FileSystem.CacheDirectory, "PermissionsReport.pdf");
//                using (var fileStream = new FileStream(filePath, FileMode.Create))
//                {
//                    document.Save(fileStream);
//                }

//                return filePath;
//            }
//            catch (Exception ex)
//            {
//                throw new Exception($"PDF export failed: {ex.Message}", ex);
//            }
//        }
//    }
//}