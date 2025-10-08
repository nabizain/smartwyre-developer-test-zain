using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services
{
    public class RebateService : IRebateService
    {
        private readonly IRebateDataStore _rebateDataStore;
        private readonly IProductDataStore _productDataStore;

        //Dependency Injection: The RebateDataStore and ProductDataStore are now injected through the
        //constructor.
        //This allows for easier testing and decouples the service from the data store implementations.
        public RebateService(IRebateDataStore rebateDataStore, IProductDataStore productDataStore)
        {
            _rebateDataStore = rebateDataStore;
            _productDataStore = productDataStore;
        }

        public CalculateRebateResult Calculate(CalculateRebateRequest request)
        {
            var rebate = _rebateDataStore.GetRebate(request.RebateIdentifier);
            var product = _productDataStore.GetProduct(request.ProductIdentifier);
            var result = new CalculateRebateResult();

            //Early Exit for Null Checks: The checks for null rebate and product are performed early in the Calculate method. If either is null,
            //the method returns immediately, reducing nested conditions and improving readability.
            if (rebate == null || product == null)
            {
                result.Success = false;
                return result;
            }

            decimal rebateAmount = CalculateRebateAmount(rebate, product, request, result);

            if (result.Success)
            {
                _rebateDataStore.StoreCalculationResult(rebate, rebateAmount);
            }

            return result;
        }
        //Separation of Concerns: The logic for calculating the rebate amount has been moved to a separate method, CalculateRebateAmount.
        //This adheres to the Single Responsibility Principle, making the code easier to maintain and test.
        private decimal CalculateRebateAmount(Rebate rebate, Product product, CalculateRebateRequest request, CalculateRebateResult result)
        {
            decimal rebateAmount = 0m;

            //Simplified Logic: The conditions within each case of the switch statement have been streamlined.
            //This reduces redundancy and enhances clarity, making it easier to understand the flow of logic.
            switch (rebate.Incentive)
            {
                case IncentiveType.FixedCashAmount:
                    if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedCashAmount) || rebate.Amount == 0)
                    {
                        result.Success = false;
                    }
                    else
                    {
                        rebateAmount = rebate.Amount;
                        result.Success = true;
                    }
                    break;

                case IncentiveType.FixedRateRebate:
                    if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedRateRebate) ||
                        rebate.Percentage == 0 || product.Price == 0 || request.Volume == 0)
                    {
                        result.Success = false;
                    }
                    else
                    {
                        rebateAmount = product.Price * rebate.Percentage * request.Volume;
                        result.Success = true;
                    }
                    break;

                case IncentiveType.AmountPerUom:
                    if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.AmountPerUom) ||
                        rebate.Amount == 0 || request.Volume == 0)
                    {
                        result.Success = false;
                    }
                    else
                    {
                        rebateAmount = rebate.Amount * request.Volume;
                        result.Success = true;
                    }
                    break;
            }

            return rebateAmount;
        }
    }
}
