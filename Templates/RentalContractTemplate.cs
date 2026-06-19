using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Veloco.DTOs.Rental;

namespace Veloco.Templates;

public static class RentalContractTemplate
{
    static RentalContractTemplate()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public static byte[] Generate(RentalContractDto data)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(50);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header()
                    .Text("VELOCE RENTAL CONTRACT")
                    .FontSize(22)
                    .Bold()
                    .AlignCenter();

                page.Content().PaddingVertical(20).Column(column =>
                {
                    column.Item().Text($"Booking #{data.BookingId}").FontSize(16).Bold();
                    column.Item().LineHorizontal(0.5f);

                    column.Item().Text("CUSTOMER INFORMATION").FontSize(14).Bold();
                    column.Item().Text($"Name: {data.CustomerName}");
                    column.Item().Text($"Email: {data.CustomerEmail}");
                    column.Item().Text($"Phone: {data.CustomerPhone}");
                    column.Item().Text("");

                    column.Item().Text("VEHICLE INFORMATION").FontSize(14).Bold();
                    column.Item().Text($"Vehicle: {data.CarBrand} {data.CarModel} ({data.CarYear})");
                    column.Item().Text("");

                    column.Item().Text("RENTAL DETAILS").FontSize(14).Bold();
                    column.Item().Text($"Start Date: {data.StartDate:dd/MM/yyyy}");
                    column.Item().Text($"End Date: {data.EndDate:dd/MM/yyyy}");
                    column.Item().Text($"Total Days: {data.TotalDays}");
                    column.Item().Text("");

                    column.Item().Text("PAYMENT DETAILS").FontSize(14).Bold();
                    column.Item().Text($"Total Price: ${data.TotalPrice:F2}");
                    column.Item().Text($"Deposit Paid: ${data.DepositPaid:F2}");
                    column.Item().Text("");

                    column.Item().Text("TERMS & CONDITIONS").FontSize(14).Bold();
                    column.Item().Text("1. Vehicle must be returned in the same condition as received.");
                    column.Item().Text("2. Fuel must be returned at the same level as pickup.");
                    column.Item().Text("3. Any damage must be reported immediately.");
                    column.Item().Text("4. Late return fee: $50 per hour.");
                    column.Item().Text("5. Mileage limit: 200 km/day (additional $0.50/km).");
                    column.Item().Text("6. Smoking in the vehicle is strictly prohibited.");
                    column.Item().Text("7. Driver must be over 21 years old and hold a valid license.");
                    column.Item().Text("");

                    column.Item().Text("SIGNATURES").FontSize(14).Bold();
                    column.Item().Text($"Customer: {data.CustomerName} (Signed on {data.GeneratedAt:dd/MM/yyyy})");
                    column.Item().Text($"Veloce Representative: {data.DealershipName}");
                });

                page.Footer()
                    .AlignCenter()
                    .Text($"Generated on {data.GeneratedAt:yyyy-MM-dd HH:mm} | Thank you for choosing Veloce!");
            });
        });

        return document.GeneratePdf();
    }
}