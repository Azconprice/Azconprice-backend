using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController(IPaymentService paymentService) : ControllerBase
    {
        [HttpPost("order")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin,User,Worker,Company")]
        public async Task<IActionResult> CreatePaymentOrder()
        {
            try
            {
                var paymentUrl = await paymentService.CreatePaymentOrder();
                return Ok(new { PaymentUrl = paymentUrl });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "An unexpected error occurred.", Details = ex.Message });
            }
        }

        [HttpGet("callback")]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentCallback([FromQuery] string id, string status)
        {
            if (status == "FullyPaid")
                return Content(SuccessHtml, "text/html");

            return Content(FailureHtml, "text/html");
        }

        private const string SuccessHtml = @"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Payment Success</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            text-align: center;
            background: #f0fff4;
            padding: 50px;
        }
        .card {
            max-width: 400px;
            margin: auto;
            background: #fff;
            border-radius: 12px;
            box-shadow: 0 4px 8px rgba(0,0,0,0.1);
            padding: 30px;
        }
        .success {
            color: #2f855a;
            font-size: 2em;
            margin-bottom: 10px;
        }
    </style>
</head>
<body>
    <div class='card'>
        <div class='success'>✔ Payment Successful!</div>
        <p>Your order has been fully paid.</p>
    </div>
</body>
</html>";

        private const string FailureHtml = @"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Payment Failed</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            text-align: center;
            background: #fff5f5;
            padding: 50px;
        }
        .card {
            max-width: 400px;
            margin: auto;
            background: #fff;
            border-radius: 12px;
            box-shadow: 0 4px 8px rgba(0,0,0,0.1);
            padding: 30px;
        }
        .failure {
            color: #c53030;
            font-size: 2em;
            margin-bottom: 10px;
        }
    </style>
</head>
<body>
    <div class='card'>
        <div class='failure'>✘ Payment Failed</div>
        <p>Something went wrong. Please try again or contact support.</p>
    </div>
</body>
</html>";
    }
}
