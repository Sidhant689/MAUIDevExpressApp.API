using MAUIDevExpressApp.Shared.DTOs;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.UI.Services.Reporting
{
    public class PermissionReportService
    {
        public async Task<string> GeneratePermissionReportAsync(ObservableCollection<PermissionDTO> permissions)
        {
            // Create a new PDF document
            using (PdfDocument document = new PdfDocument())
            {
                // Add a page
                PdfPage page = document.Pages.Add();

                // Get the page graphics
                PdfGraphics graphics = page.Graphics;

                // Set up fonts
                PdfStandardFont headerFont = new PdfStandardFont(PdfFontFamily.Helvetica, 18, PdfFontStyle.Bold);
                PdfStandardFont subHeaderFont = new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold);
                PdfStandardFont normalFont = new PdfStandardFont(PdfFontFamily.Helvetica, 10);

                // Add title
                graphics.DrawString("Permissions Report", headerFont, PdfBrushes.DarkBlue,
                    new Syncfusion.Drawing.PointF(page.Size.Width / 2 - 80, 20));

                // Add generation date
                graphics.DrawString($"Generated: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}", normalFont,
                    PdfBrushes.Black, new Syncfusion.Drawing.PointF(page.Size.Width - 250, 50));

                // Add summary
                graphics.DrawString($"Total Permissions: {permissions.Count}", subHeaderFont,
                    PdfBrushes.Black, new Syncfusion.Drawing.PointF(10, 80));

                graphics.DrawString($"Active Permissions: {permissions.Count(p => p.IsActive)}", normalFont,
                    PdfBrushes.Black, new Syncfusion.Drawing.PointF(10, 100));

                graphics.DrawString($"Inactive Permissions: {permissions.Count(p => !p.IsActive)}", normalFont,
                    PdfBrushes.Black, new Syncfusion.Drawing.PointF(10, 120));

                // Create PDF grid
                PdfGrid grid = new PdfGrid();

                // Add columns
                grid.Columns.Add(6);
                grid.Headers.Add(1);

                // Set header style
                PdfGridRow header = grid.Headers[0];
                header.Cells[0].Value = "ID";
                header.Cells[1].Value = "Name";
                header.Cells[2].Value = "Module";
                header.Cells[3].Value = "Action";
                header.Cells[4].Value = "Description";
                header.Cells[5].Value = "Status";

                // Style the header
                header.Style.BackgroundBrush = PdfBrushes.LightGray;
                header.Style.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold);

                // Add rows
                foreach (var permission in permissions)
                {
                    PdfGridRow row = grid.Rows.Add();
                    row.Cells[0].Value = permission.Id.ToString();
                    row.Cells[1].Value = permission.Name;
                    row.Cells[2].Value = permission.Module;
                    row.Cells[3].Value = permission.Action;
                    row.Cells[4].Value = permission.Description;
                    row.Cells[5].Value = permission.StatusDisplay;

                    // Apply color to status cell based on active state
                    if (permission.IsActive)
                    {
                        row.Cells[5].Style.BackgroundBrush = new PdfSolidBrush(Syncfusion.Drawing.Color.FromArgb(144, 238, 144)); // Light green
                    }
                    else
                    {
                        row.Cells[5].Style.BackgroundBrush = new PdfSolidBrush(Syncfusion.Drawing.Color.FromArgb(255, 182, 193)); // Light pink
                    }
                }

                // Set grid style
                grid.Style.Font = normalFont;
                grid.Style.CellPadding = new PdfPaddings(5, 5, 5, 5);

                // Set column widths
                grid.Columns[0].Width = 40;
                grid.Columns[1].Width = 100;
                grid.Columns[2].Width = 80;
                grid.Columns[3].Width = 80;
                grid.Columns[4].Width = 150;
                grid.Columns[5].Width = 60;

                // Draw the grid
                grid.Draw(page, new Syncfusion.Drawing.PointF(10, 150));

                // Add footer
                graphics.DrawString("© " + DateTime.Now.Year + " - Permissions Report",
                                    new PdfStandardFont(PdfFontFamily.Courier, 8),
                                    PdfBrushes.Gray,
                                    new Syncfusion.Drawing.PointF(page.Size.Width / 2 - 70, page.Size.Height - 30));

                // Save the document to file
                string filePath = Path.Combine(FileSystem.CacheDirectory, "PermissionsReport.pdf");

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    document.Save(stream);
                }

                return filePath;
            }
        }
    }
}