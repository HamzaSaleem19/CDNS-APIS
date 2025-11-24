using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using CDNSBlazorApp.Data;
using CDNSBlazorApp.Models;

namespace CDNSBlazorApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager")]
    public class ExcelUploadController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExcelUploadController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("instituation")]
        public async Task<IActionResult> UploadInstituation(IFormFile file)
        {
            return await ProcessExcelFile(file, "Instituation", ProcessInstituationRow);
        }

        [HttpPost("instrument")]
        public async Task<IActionResult> UploadInstrument(IFormFile file)
        {
            return await ProcessExcelFile(file, "Instrument", ProcessInstrumentRow);
        }

        [HttpPost("series")]
        public async Task<IActionResult> UploadSeries(IFormFile file)
        {
            return await ProcessExcelFile(file, "Series", ProcessSeriesRow);
        }

        [HttpPost("seriespattern")]
        public async Task<IActionResult> UploadSeriesPattern(IFormFile file)
        {
            return await ProcessExcelFile(file, "SeriesPattern", ProcessSeriesPatternRow);
        }

        [HttpPost("subscription")]
        public async Task<IActionResult> UploadSubscription(IFormFile file)
        {
            return await ProcessExcelFile(file, "Subscription", ProcessSubscriptionRow);
        }

        [HttpPost("payment")]
        public async Task<IActionResult> UploadPayment(IFormFile file)
        {
            return await ProcessExcelFile(file, "Payment", ProcessPaymentRow);
        }

        private async Task<IActionResult> ProcessExcelFile(IFormFile file, string entityType, Func<ExcelWorksheet, int, Task<(bool success, string? error)>> processRow)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file uploaded" });

            if (!file.FileName.EndsWith(".xlsx") && !file.FileName.EndsWith(".xls"))
                return BadRequest(new { message = "Please upload a valid Excel file" });

            try
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);

                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets[0];

                if (worksheet.Dimension == null)
                    return BadRequest(new { message = "The Excel file is empty" });

                int rowCount = worksheet.Dimension.Rows;
                int successCount = 0;
                var errors = new List<string>();

                // Start from row 2 (skip header)
                for (int row = 2; row <= rowCount; row++)
                {
                    var (success, error) = await processRow(worksheet, row);
                    if (success)
                    {
                        // Save changes after each row to prevent tracking and concurrency issues
                        try
                        {
                            await _context.SaveChangesAsync();
                            successCount++;
                        }
                        catch (Exception ex)
                        {
                            errors.Add($"Row {row}: Failed to save - {ex.Message}");
                        }
                    }
                    else if (error != null)
                        errors.Add($"Row {row}: {error}");
                }

                return Ok(new
                {
                    message = $"Successfully uploaded {successCount} {entityType} records",
                    totalRows = rowCount - 1,
                    successCount,
                    errorCount = errors.Count,
                    errors
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error processing file", error = ex.Message });
            }
        }

        private async Task<(bool success, string? error)> ProcessInstituationRow(ExcelWorksheet worksheet, int row)
        {
            try
            {
                // Skip empty rows - check if the ID column has a value
                var idValue = worksheet.Cells[row, 1].Value?.ToString();
                if (string.IsNullOrWhiteSpace(idValue))
                {
                    return (true, null); // Skip this row silently
                }

                // Validate required fields
                var instituationName = worksheet.Cells[row, 2].Value?.ToString();
                if (string.IsNullOrWhiteSpace(instituationName))
                {
                    return (false, "Instituation Name is required (Column 2)");
                }

                var instituationId = idValue;

                // Check if instituation already exists
                var existingInstituation = await _context.Instituations.FindAsync(instituationId);

                if (existingInstituation != null)
                {
                    // Update existing record
                    existingInstituation.InstituationName = instituationName;
                    existingInstituation.ContactEmail = worksheet.Cells[row, 3].Value?.ToString();
                    existingInstituation.CreatedAt = DateTime.TryParse(worksheet.Cells[row, 4].Value?.ToString(), out var createdAt) ? createdAt : existingInstituation.CreatedAt;
                    existingInstituation.CreatedAtValueDate = DateTime.TryParse(worksheet.Cells[row, 5].Value?.ToString(), out var createdDate) ? createdDate : existingInstituation.CreatedAtValueDate;

                    _context.Instituations.Update(existingInstituation);
                }
                else
                {
                    // Add new record
                    var instituation = new Instituation
                    {
                        InstituationId = instituationId,
                        InstituationName = instituationName,
                        ContactEmail = worksheet.Cells[row, 3].Value?.ToString(),
                        CreatedAt = DateTime.TryParse(worksheet.Cells[row, 4].Value?.ToString(), out var createdAt) ? createdAt : DateTime.Now,
                        CreatedAtValueDate = DateTime.TryParse(worksheet.Cells[row, 5].Value?.ToString(), out var createdDate) ? createdDate : DateTime.Now.Date
                    };

                    _context.Instituations.Add(instituation);
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        private async Task<(bool success, string? error)> ProcessInstrumentRow(ExcelWorksheet worksheet, int row)
        {
            try
            {
                // Skip empty rows
                var idValue = worksheet.Cells[row, 1].Value?.ToString();
                if (string.IsNullOrWhiteSpace(idValue))
                {
                    return (true, null);
                }

                // Validate required fields
                var instrumentName = worksheet.Cells[row, 2].Value?.ToString();
                if (string.IsNullOrWhiteSpace(instrumentName))
                {
                    return (false, "Instrument Name is required (Column 2)");
                }

                var instituationId = worksheet.Cells[row, 24].Value?.ToString() ?? "CDNS";

                // Validate foreign key - check if Instituation exists
                var instituationExists = await _context.Instituations.AnyAsync(i => i.InstituationId == instituationId);
                if (!instituationExists)
                {
                    return (false, $"Instituation '{instituationId}' does not exist (Column 24). Please upload Instituations first.");
                }

                var instrumentId = idValue;

                // Check if instrument already exists
                var existingInstrument = await _context.Instruments.FindAsync(instrumentId);

                if (existingInstrument != null)
                {
                    // Update existing record
                    existingInstrument.Name = worksheet.Cells[row, 2].Value?.ToString() ?? "";
                    existingInstrument.LongName = worksheet.Cells[row, 3].Value?.ToString();
                    existingInstrument.OriginalInstId = worksheet.Cells[row, 4].Value?.ToString();
                    existingInstrument.IsinCode = worksheet.Cells[row, 5].Value?.ToString();
                    existingInstrument.CdDurCat = worksheet.Cells[row, 6].Value?.ToString();
                    existingInstrument.CdLoStatus = worksheet.Cells[row, 7].Value?.ToString();
                    existingInstrument.DSign = DateTime.TryParse(worksheet.Cells[row, 8].Value?.ToString(), out var dSign) ? dSign : null;
                    existingInstrument.DFinalMaturity = DateTime.TryParse(worksheet.Cells[row, 9].Value?.ToString(), out var dFinal) ? dFinal : null;
                    existingInstrument.Amt = decimal.TryParse(worksheet.Cells[row, 11].Value?.ToString(), out var amt) ? amt : 0;
                    existingInstrument.AmtDiscounted = decimal.TryParse(worksheet.Cells[row, 12].Value?.ToString(), out var amtDisc) ? amtDisc : 0;
                    existingInstrument.CuBase = worksheet.Cells[row, 13].Value?.ToString() ?? "PKR";
                    existingInstrument.AmtNet = decimal.TryParse(worksheet.Cells[row, 14].Value?.ToString(), out var amtNet) ? amtNet : 0;
                    existingInstrument.CdDebtSource = worksheet.Cells[row, 15].Value?.ToString();
                    existingInstrument.CdDebtType = worksheet.Cells[row, 16].Value?.ToString();
                    existingInstrument.CdInstrumentType = worksheet.Cells[row, 17].Value?.ToString();
                    existingInstrument.CdDebtSecIntrType = worksheet.Cells[row, 18].Value?.ToString();
                    existingInstrument.CdReorgGrp = worksheet.Cells[row, 19].Value?.ToString() ?? "4";
                    existingInstrument.CdLoPrp = worksheet.Cells[row, 20].Value?.ToString();
                    existingInstrument.CdEconSect = worksheet.Cells[row, 21].Value?.ToString();
                    existingInstrument.CdSourceModule = worksheet.Cells[row, 22].Value?.ToString() ?? "1";
                    existingInstrument.CdUserCode1 = worksheet.Cells[row, 23].Value?.ToString() ?? "1";
                    existingInstrument.InstituationId = worksheet.Cells[row, 24].Value?.ToString() ?? "CDNS";
                    existingInstrument.CreditorId = int.TryParse(worksheet.Cells[row, 25].Value?.ToString(), out var credId) ? credId : null;
                    existingInstrument.DebtorId = int.TryParse(worksheet.Cells[row, 26].Value?.ToString(), out var debtId) ? debtId : null;

                    _context.Instruments.Update(existingInstrument);
                }
                else
                {
                    // Add new record
                    var instrument = new Instrument
                    {
                        InstrumentId = instrumentId,
                        Name = worksheet.Cells[row, 2].Value?.ToString() ?? "",
                        LongName = worksheet.Cells[row, 3].Value?.ToString(),
                        OriginalInstId = worksheet.Cells[row, 4].Value?.ToString(),
                        IsinCode = worksheet.Cells[row, 5].Value?.ToString(),
                        CdDurCat = worksheet.Cells[row, 6].Value?.ToString(),
                        CdLoStatus = worksheet.Cells[row, 7].Value?.ToString(),
                        DSign = DateTime.TryParse(worksheet.Cells[row, 8].Value?.ToString(), out var dSign) ? dSign : null,
                        DFinalMaturity = DateTime.TryParse(worksheet.Cells[row, 9].Value?.ToString(), out var dFinal) ? dFinal : null,
                        Amt = decimal.TryParse(worksheet.Cells[row, 11].Value?.ToString(), out var amt) ? amt : 0,
                        AmtDiscounted = decimal.TryParse(worksheet.Cells[row, 12].Value?.ToString(), out var amtDisc) ? amtDisc : 0,
                        CuBase = worksheet.Cells[row, 13].Value?.ToString() ?? "PKR",
                        AmtNet = decimal.TryParse(worksheet.Cells[row, 14].Value?.ToString(), out var amtNet) ? amtNet : 0,
                        CdDebtSource = worksheet.Cells[row, 15].Value?.ToString(),
                        CdDebtType = worksheet.Cells[row, 16].Value?.ToString(),
                        CdInstrumentType = worksheet.Cells[row, 17].Value?.ToString(),
                        CdDebtSecIntrType = worksheet.Cells[row, 18].Value?.ToString(),
                        CdReorgGrp = worksheet.Cells[row, 19].Value?.ToString() ?? "4",
                        CdLoPrp = worksheet.Cells[row, 20].Value?.ToString(),
                        CdEconSect = worksheet.Cells[row, 21].Value?.ToString(),
                        CdSourceModule = worksheet.Cells[row, 22].Value?.ToString() ?? "1",
                        CdUserCode1 = worksheet.Cells[row, 23].Value?.ToString() ?? "1",
                        InstituationId = worksheet.Cells[row, 24].Value?.ToString() ?? "CDNS",
                        CreditorId = int.TryParse(worksheet.Cells[row, 25].Value?.ToString(), out var credId) ? credId : null,
                        DebtorId = int.TryParse(worksheet.Cells[row, 26].Value?.ToString(), out var debtId) ? debtId : null
                    };

                    _context.Instruments.Add(instrument);
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        private async Task<(bool success, string? error)> ProcessSeriesRow(ExcelWorksheet worksheet, int row)
        {
            try
            {
                // Skip empty rows
                var idValue = worksheet.Cells[row, 1].Value?.ToString();
                if (string.IsNullOrWhiteSpace(idValue))
                {
                    return (true, null);
                }

                var serieId = idValue;

                // Validate TraNo is a valid integer
                if (!int.TryParse(worksheet.Cells[row, 2].Value?.ToString(), out var serieTraNo))
                {
                    return (false, "TraNo must be a valid integer (Column 2)");
                }

                // Validate foreign key - check if Instrument exists
                var instrumentExists = await _context.Instruments.AnyAsync(i => i.InstrumentId == serieId);
                if (!instrumentExists)
                {
                    return (false, $"Instrument '{serieId}' does not exist (Column 1). Please upload Instruments first.");
                }

                // Check if series already exists
                var existingSeries = await _context.Series.FindAsync(serieId, serieTraNo);

                if (existingSeries != null)
                {
                    // Update existing record
                    existingSeries.SerieAmt = decimal.TryParse(worksheet.Cells[row, 3].Value?.ToString(), out var amt) ? amt : 0;
                    existingSeries.SerieCurrency = worksheet.Cells[row, 4].Value?.ToString() ?? "PKR";
                    existingSeries.CdReorgGrp = worksheet.Cells[row, 5].Value?.ToString() ?? "4";

                    _context.Series.Update(existingSeries);
                }
                else
                {
                    // Add new record
                    var series = new Series
                    {
                        SerieId = serieId,
                        SerieTraNo = serieTraNo,
                        SerieAmt = decimal.TryParse(worksheet.Cells[row, 3].Value?.ToString(), out var amt) ? amt : 0,
                        SerieCurrency = worksheet.Cells[row, 4].Value?.ToString() ?? "PKR",
                        CdReorgGrp = worksheet.Cells[row, 5].Value?.ToString() ?? "4"
                    };

                    _context.Series.Add(series);
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        private async Task<(bool success, string? error)> ProcessSeriesPatternRow(ExcelWorksheet worksheet, int row)
        {
            try
            {
                // Skip empty rows
                var idValue = worksheet.Cells[row, 1].Value?.ToString();
                if (string.IsNullOrWhiteSpace(idValue))
                {
                    return (true, null);
                }

                var seriePatId = idValue;

                // Validate TraNo is a valid integer
                if (!int.TryParse(worksheet.Cells[row, 2].Value?.ToString(), out var seriePatTraNo))
                {
                    return (false, "TraNo must be a valid integer (Column 2)");
                }

                // Validate foreign key - check if Instrument exists
                var instrumentExists = await _context.Instruments.AnyAsync(i => i.InstrumentId == seriePatId);
                if (!instrumentExists)
                {
                    return (false, $"Instrument '{seriePatId}' does not exist (Column 1). Please upload Instruments first.");
                }

                // Check if series pattern already exists
                var existingPattern = await _context.SeriesPatterns.FindAsync(seriePatId, seriePatTraNo);

                if (existingPattern != null)
                {
                    // Update existing record
                    existingPattern.CdPatType = worksheet.Cells[row, 3].Value?.ToString();
                    existingPattern.CdPeriodicity = worksheet.Cells[row, 4].Value?.ToString();
                    existingPattern.CgPeriodicity = worksheet.Cells[row, 5].Value?.ToString();
                    existingPattern.DFirstPayment = DateTime.TryParse(worksheet.Cells[row, 6].Value?.ToString(), out var firstPay) ? firstPay : null;
                    existingPattern.DLastPayment = DateTime.TryParse(worksheet.Cells[row, 7].Value?.ToString(), out var lastPay) ? lastPay : null;
                    existingPattern.Amt = decimal.TryParse(worksheet.Cells[row, 8].Value?.ToString(), out var amt) ? amt : null;
                    existingPattern.Percentage = int.TryParse(worksheet.Cells[row, 9].Value?.ToString(), out var pct) ? pct : 100;

                    _context.SeriesPatterns.Update(existingPattern);
                }
                else
                {
                    // Add new record
                    var seriesPattern = new SeriesPattern
                    {
                        SeriePatId = seriePatId,
                        SeriePatTraNo = seriePatTraNo,
                        CdPatType = worksheet.Cells[row, 3].Value?.ToString(),
                        CdPeriodicity = worksheet.Cells[row, 4].Value?.ToString(),
                        CgPeriodicity = worksheet.Cells[row, 5].Value?.ToString(),
                        DFirstPayment = DateTime.TryParse(worksheet.Cells[row, 6].Value?.ToString(), out var firstPay) ? firstPay : null,
                        DLastPayment = DateTime.TryParse(worksheet.Cells[row, 7].Value?.ToString(), out var lastPay) ? lastPay : null,
                        Amt = decimal.TryParse(worksheet.Cells[row, 8].Value?.ToString(), out var amt) ? amt : null,
                        Percentage = int.TryParse(worksheet.Cells[row, 9].Value?.ToString(), out var pct) ? pct : 100
                    };

                    _context.SeriesPatterns.Add(seriesPattern);
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        private async Task<(bool success, string? error)> ProcessSubscriptionRow(ExcelWorksheet worksheet, int row)
        {
            try
            {
                // Skip empty rows
                var idValue = worksheet.Cells[row, 1].Value?.ToString();
                if (string.IsNullOrWhiteSpace(idValue))
                {
                    return (true, null);
                }

                var subscriptionId = idValue;

                // Validate TraNo is a valid integer
                if (!int.TryParse(worksheet.Cells[row, 2].Value?.ToString(), out var subscriptionIdTraNo))
                {
                    return (false, "TraNo must be a valid integer (Column 2)");
                }

                // Validate foreign key - check if Instrument exists
                var instrumentExists = await _context.Instruments.AnyAsync(i => i.InstrumentId == subscriptionId);
                if (!instrumentExists)
                {
                    return (false, $"Instrument '{subscriptionId}' does not exist (Column 1). Please upload Instruments first.");
                }

                // Check if subscription already exists
                var existingSubscription = await _context.Subscriptions.FindAsync(subscriptionId, subscriptionIdTraNo);

                if (existingSubscription != null)
                {
                    // Update existing record
                    existingSubscription.TrDate = DateTime.TryParse(worksheet.Cells[row, 3].Value?.ToString(), out var trDate) ? trDate : DateTime.Now;
                    existingSubscription.ReceivedDate = DateTime.TryParse(worksheet.Cells[row, 4].Value?.ToString(), out var recDate) ? recDate : DateTime.Now;
                    existingSubscription.CdTransactionType = worksheet.Cells[row, 5].Value?.ToString();
                    existingSubscription.LocalExchangeDate = DateTime.TryParse(worksheet.Cells[row, 6].Value?.ToString(), out var exchDate) ? exchDate : null;
                    existingSubscription.CuBase = worksheet.Cells[row, 7].Value?.ToString() ?? "PKR";
                    existingSubscription.ReceiptsAmount = decimal.TryParse(worksheet.Cells[row, 8].Value?.ToString(), out var amt) ? amt : 0;
                    existingSubscription.CdExecMode = worksheet.Cells[row, 9].Value?.ToString() ?? "1";

                    _context.Subscriptions.Update(existingSubscription);
                }
                else
                {
                    // Add new record
                    var subscription = new Subscription
                    {
                        SubscriptionId = subscriptionId,
                        SubscriptionIdTraNo = subscriptionIdTraNo,
                        TrDate = DateTime.TryParse(worksheet.Cells[row, 3].Value?.ToString(), out var trDate) ? trDate : DateTime.Now,
                        ReceivedDate = DateTime.TryParse(worksheet.Cells[row, 4].Value?.ToString(), out var recDate) ? recDate : DateTime.Now,
                        CdTransactionType = worksheet.Cells[row, 5].Value?.ToString(),
                        LocalExchangeDate = DateTime.TryParse(worksheet.Cells[row, 6].Value?.ToString(), out var exchDate) ? exchDate : null,
                        CuBase = worksheet.Cells[row, 7].Value?.ToString() ?? "PKR",
                        ReceiptsAmount = decimal.TryParse(worksheet.Cells[row, 8].Value?.ToString(), out var amt) ? amt : 0,
                        CdExecMode = worksheet.Cells[row, 9].Value?.ToString() ?? "1"
                    };

                    _context.Subscriptions.Add(subscription);
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        private async Task<(bool success, string? error)> ProcessPaymentRow(ExcelWorksheet worksheet, int row)
        {
            try
            {
                // Skip empty rows
                var idValue = worksheet.Cells[row, 1].Value?.ToString();
                if (string.IsNullOrWhiteSpace(idValue))
                {
                    return (true, null);
                }

                var paymentId = idValue;

                // Validate TraNo is a valid integer
                if (!int.TryParse(worksheet.Cells[row, 2].Value?.ToString(), out var paymentTraNo))
                {
                    return (false, "TraNo must be a valid integer (Column 2)");
                }

                // Validate foreign key - check if Instrument exists
                var instrumentExists = await _context.Instruments.AnyAsync(i => i.InstrumentId == paymentId);
                if (!instrumentExists)
                {
                    return (false, $"Instrument '{paymentId}' does not exist (Column 1). Please upload Instruments first.");
                }

                // Check if payment already exists
                var existingPayment = await _context.Payments.FindAsync(paymentId, paymentTraNo);

                if (existingPayment != null)
                {
                    // Update existing record
                    existingPayment.SchPaymentDate = DateTime.TryParse(worksheet.Cells[row, 3].Value?.ToString(), out var schDate) ? schDate : DateTime.Now;
                    existingPayment.MadeDate = DateTime.TryParse(worksheet.Cells[row, 4].Value?.ToString(), out var madeDate) ? madeDate : DateTime.Now;
                    existingPayment.ReceivedDate = DateTime.TryParse(worksheet.Cells[row, 5].Value?.ToString(), out var recDate) ? recDate : DateTime.Now;
                    existingPayment.LocalExchRateDate = DateTime.TryParse(worksheet.Cells[row, 6].Value?.ToString(), out var exchDate) ? exchDate : null;
                    existingPayment.CdPaymentMode = worksheet.Cells[row, 7].Value?.ToString() ?? "1";
                    existingPayment.Amount = decimal.TryParse(worksheet.Cells[row, 8].Value?.ToString(), out var amt) ? amt : 0;
                    existingPayment.CuBase = worksheet.Cells[row, 9].Value?.ToString() ?? "PKR";
                    existingPayment.CdTransactionType = worksheet.Cells[row, 10].Value?.ToString();
                    existingPayment.CdAmountDiff = worksheet.Cells[row, 11].Value?.ToString() ?? "1";

                    _context.Payments.Update(existingPayment);
                }
                else
                {
                    // Add new record
                    var payment = new Payment
                    {
                        PaymentId = paymentId,
                        PaymentTraNo = paymentTraNo,
                        SchPaymentDate = DateTime.TryParse(worksheet.Cells[row, 3].Value?.ToString(), out var schDate) ? schDate : DateTime.Now,
                        MadeDate = DateTime.TryParse(worksheet.Cells[row, 4].Value?.ToString(), out var madeDate) ? madeDate : DateTime.Now,
                        ReceivedDate = DateTime.TryParse(worksheet.Cells[row, 5].Value?.ToString(), out var recDate) ? recDate : DateTime.Now,
                        LocalExchRateDate = DateTime.TryParse(worksheet.Cells[row, 6].Value?.ToString(), out var exchDate) ? exchDate : null,
                        CdPaymentMode = worksheet.Cells[row, 7].Value?.ToString() ?? "1",
                        Amount = decimal.TryParse(worksheet.Cells[row, 8].Value?.ToString(), out var amt) ? amt : 0,
                        CuBase = worksheet.Cells[row, 9].Value?.ToString() ?? "PKR",
                        CdTransactionType = worksheet.Cells[row, 10].Value?.ToString(),
                        CdAmountDiff = worksheet.Cells[row, 11].Value?.ToString() ?? "1"
                    };

                    _context.Payments.Add(payment);
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
