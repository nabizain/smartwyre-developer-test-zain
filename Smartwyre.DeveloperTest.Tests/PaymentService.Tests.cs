using Moq;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;
using System;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests;

public class PaymentServiceTests
{
    private readonly Mock<IRebateDataStore> _rebateDataStoreMock;
    private readonly Mock<IProductDataStore> _productDataStoreMock;
    private readonly RebateService _rebateService;

    public PaymentServiceTests()
    {
        _rebateDataStoreMock = new Mock<IRebateDataStore>();
        _productDataStoreMock = new Mock<IProductDataStore>();
        _rebateService = new RebateService(_rebateDataStoreMock.Object, _productDataStoreMock.Object);
    }

    [Fact]
    public void Calculate_ShouldReturnFailure_WhenRebateOrProductIsNull()
    {
        // Arrange
        var request = new CalculateRebateRequest { RebateIdentifier = "rebate1", ProductIdentifier = "product1" };
        _rebateDataStoreMock.Setup(x => x.GetRebate(request.RebateIdentifier)).Returns((Rebate)null);
        _productDataStoreMock.Setup(x => x.GetProduct(request.ProductIdentifier)).Returns((Product)null);

        // Act
        var result = _rebateService.Calculate(request);

        // Assert
        Assert.False(result.Success);
    }

    [Fact]
    public void Calculate_ShouldReturnSuccess_WhenValidFixedCashAmount()
    {
        // Arrange
        var request = new CalculateRebateRequest { RebateIdentifier = "rebate1", ProductIdentifier = "product1" };
        var rebate = new Rebate { Incentive = IncentiveType.FixedCashAmount, Amount = 100 };
        var product = new Product { SupportedIncentives = SupportedIncentiveType.FixedCashAmount };

        _rebateDataStoreMock.Setup(x => x.GetRebate(request.RebateIdentifier)).Returns(rebate);
        _productDataStoreMock.Setup(x => x.GetProduct(request.ProductIdentifier)).Returns(product);

        // Act
        var result = _rebateService.Calculate(request);

        // Assert
        Assert.True(result.Success);
    }

}
