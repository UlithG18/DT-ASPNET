using ClosedXML.Excel;
using DT_ASPNET.Domain.Reservations;

namespace DT_ASPNET.Application.Reservations;

public interface IReportService
{
    Task<byte[]> GenerateExcelAsync(Guid ownerId, Guid? propertyId, DateTime? from, DateTime? to);
}

public class ReportService(IReservationRepository reservations) : IReportService
{
    public async Task<byte[]> GenerateExcelAsync(
        Guid ownerId, Guid? propertyId, DateTime? from, DateTime? to)
    {
        var data = await reservations.GetByOwnerAsync(ownerId, propertyId, from, to);

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Reservas");

        // Encabezados
        var headers = new[]
        {
            "ID Reserva", "Inmueble", "Ciudad",
            "Huésped", "Email Huésped",
            "Check-in", "Check-out", "Noches",
            "Precio/Noche", "Total"
        };

        for (var i = 0; i < headers.Length; i++)
        {
            ws.Cell(1, i + 1).Value = headers[i];
            ws.Cell(1, i + 1).Style.Font.Bold = true;
            ws.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#2563EB");
            ws.Cell(1, i + 1).Style.Font.FontColor = XLColor.White;
        }

        // Datos
        for (var i = 0; i < data.Count; i++)
        {
            var r = data[i];
            var row = i + 2;

            ws.Cell(row, 1).Value  = r.Id.ToString();
            ws.Cell(row, 2).Value  = r.Property.Title;
            ws.Cell(row, 3).Value  = r.Property.City;
            ws.Cell(row, 4).Value  = r.Guest.FullName;
            ws.Cell(row, 5).Value  = r.Guest.Email;
            ws.Cell(row, 6).Value  = r.CheckIn.ToString("yyyy-MM-dd HH:mm");
            ws.Cell(row, 7).Value  = r.CheckOut.ToString("yyyy-MM-dd HH:mm");
            ws.Cell(row, 8).Value  = r.TotalNights;
            ws.Cell(row, 9).Value  = r.PricePerNight;
            ws.Cell(row, 10).Value = r.TotalAmount;

            ws.Cell(row, 9).Style.NumberFormat.Format  = "$#,##0.00";
            ws.Cell(row, 10).Style.NumberFormat.Format = "$#,##0.00";
        }

        // Totales
        var lastRow = data.Count + 2;
        ws.Cell(lastRow, 9).Value  = "TOTAL";
        ws.Cell(lastRow, 9).Style.Font.Bold = true;
        ws.Cell(lastRow, 10).Value = data.Sum(r => r.TotalAmount);
        ws.Cell(lastRow, 10).Style.Font.Bold = true;
        ws.Cell(lastRow, 10).Style.NumberFormat.Format = "$#,##0.00";

        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }
}