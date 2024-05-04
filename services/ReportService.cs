using CIS106ExceptionHandling.models;
using CIS106ExceptionHandling.services;
using Microsoft.AspNetCore.Mvc;

namespace CIS106ExceptionHandling.controllers {

    /// <summary>
    /// Controller which exposes endpoints for generating reports.
    /// </summary>
    [ApiController]
    public class ReportController: ControllerBase {

        /* This is the ReportControllers's own reference to the ReportService.
        * We don't initiliaze it here as we will let .NET provide us with a complete
        * ReportService via Constructor dependency injection below.
        */
        private readonly ReportService _reportService;

        /// <summary>
        /// Constructor for dependency injection. This constructor allows our ReportService to be injected into it by the .NET framework. 
        /// We don't have to worry about how it gets into our class, .NET takes care of that for us with its dependency injection container.
        /// </summary>
        /// <param name="reportService">The ReportService to use.</param>
        public ReportController(ReportService reportService) {
            this._reportService = reportService;
        }

        /// <summary>
        /// Generates the report for a given product
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpGet("products/{productId}/reports", Name = "GenerateProductReport")]
        public Report GenerateReport(int productId)
    {
        // Check if the productId is 999 (Super Top Secret Product)
        if (productId == 999)
        {
            throw new UnauthorizedAccessException("Super Top Secret Product. Unauthorized access.");
        }

        // Retrieve the product by productId
        var product = _productRepository.GetProduct(productId);

        // Check if the product is null (Product Not Found)
        if (product == null)
        {   
            // Return a 404 Not Found response
            throw new KeyNotFoundException($"Product with ID {productId} not found.");
        }

        // Calculate total sales past thirty days
        int totalSalesPastThirtyDays = _salesRepository.GetTotalSalesPastThirtyDays(productId);

        // Calculate daily sales past thirty days
        double dailySalesPastThirtyDays = totalSalesPastThirtyDays / 30.0;

        // Handle divide by zero scenario for product ID 1
        if (productId == 1 && dailySalesPastThirtyDays == 0)
        {
            return new Report
            {
                Product = product,
                TotalSalesPastThirtyDays = totalSalesPastThirtyDays,
                DailySalesPastThirtyDays = 0,
                GrossIncome = 0
            };
    }

    // Calculate gross income
    double grossIncome = product.Price * dailySalesPastThirtyDays;

    // Return the report
    return new Report
    {
        Product = product,
        TotalSalesPastThirtyDays = totalSalesPastThirtyDays,
        DailySalesPastThirtyDays = dailySalesPastThirtyDays,
        GrossIncome = grossIncome
    };
    }


    }

}
