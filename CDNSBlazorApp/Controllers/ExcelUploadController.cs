using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
                        successCount++;
                    else if (error != null)
                        errors.Add($"Row {row}: {error}");
                }

                await _context.SaveChangesAsync();

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
                var instituation = new Instituation
                {
                    InstituationId = worksheet.Cells[row, 1].Value?.ToString() ?? "CDNS",
                    InstituationName = worksheet.Cells[row, 2].Value?.ToString() ?? "",
                    ContactEmail = worksheet.Cells[row, 3].Value?.ToString(),
                    CreatedAt = DateTime.TryParse(worksheet.Cells[row, 4].Value?.ToString(), out var createdAt) ? createdAt : DateTime.Now,
                    CreatedAtValueDate = DateTime.TryParse(worksheet.Cells[row, 5].Value?.ToString(), out var createdDate) ? createdDate : DateTime.Now.Date
                };

                _context.Instituations.Add(instituation);
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
                var instrument = new Instrument
                {
                    InstrumentId = worksheet.Cells[row, 1].Value?.ToString() ?? "",
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
                var series = new Series
                {
                    SerieId = worksheet.Cells[row, 1].Value?.ToString() ?? "",
                    SerieTraNo = int.TryParse(worksheet.Cells[row, 2].Value?.ToString(), out var traNo) ? traNo : 1,
                    SerieAmt = decimal.TryParse(worksheet.Cells[row, 3].Value?.ToString(), out var amt) ? amt : 0,
                    SerieCurrency = worksheet.Cells[row, 4].Value?.ToString() ?? "PKR",
                    CdReorgGrp = worksheet.Cells[row, 5].Value?.ToString() ?? "4"
                };

                _context.Series.Add(series);
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
                var seriesPattern = new SeriesPattern
                {
                    SeriePatId = worksheet.Cells[row, 1].Value?.ToString() ?? "",
                    SeriePatTraNo = int.TryParse(worksheet.Cells[row, 2].Value?.ToString(), out var traNo) ? traNo : 1,
                    CdPatType = worksheet.Cells[row, 3].Value?.ToString(),
                    CdPeriodicity = worksheet.Cells[row, 4].Value?.ToString(),
                    CgPeriodicity = worksheet.Cells[row, 5].Value?.ToString(),
                    DFirstPayment = DateTime.TryParse(worksheet.Cells[row, 6].Value?.ToString(), out var firstPay) ? firstPay : null,
                    DLastPayment = DateTime.TryParse(worksheet.Cells[row, 7].Value?.ToString(), out var lastPay) ? lastPay : null,
                    Amt = decimal.TryParse(worksheet.Cells[row, 8].Value?.ToString(), out var amt) ? amt : null,
                    Percentage = int.TryParse(worksheet.Cells[row, 9].Value?.ToString(), out var pct) ? pct : 100
                };

                _context.SeriesPatterns.Add(seriesPattern);
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
                var subscription = new Subscription
                {
                    SubscriptionId = worksheet.Cells[row, 1].Value?.ToString() ?? "",
                    SubscriptionIdTraNo = int.TryParse(worksheet.Cells[row, 2].Value?.ToString(), out var traNo) ? traNo : 1,
                    TrDate = DateTime.TryParse(worksheet.Cells[row, 3].Value?.ToString(), out var trDate) ? trDate : DateTime.Now,
                    ReceivedDate = DateTime.TryParse(worksheet.Cells[row, 4].Value?.ToString(), out var recDate) ? recDate : DateTime.Now,
                    CdTransactionType = worksheet.Cells[row, 5].Value?.ToString(),
                    LocalExchangeDate = DateTime.TryParse(worksheet.Cells[row, 6].Value?.ToString(), out var exchDate) ? exchDate : null,
                    CuBase = worksheet.Cells[row, 7].Value?.ToString() ?? "PKR",
                    ReceiptsAmount = decimal.TryParse(worksheet.Cells[row, 8].Value?.ToString(), out var amt) ? amt : 0,
                    CdExecMode = worksheet.Cells[row, 9].Value?.ToString() ?? "1"
                };

                _context.Subscriptions.Add(subscription);
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
                var payment = new Payment
                {
                    PaymentId = worksheet.Cells[row, 1].Value?.ToString() ?? "",
                    PaymentTraNo = int.TryParse(worksheet.Cells[row, 2].Value?.ToString(), out var traNo) ? traNo : 1,
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
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
