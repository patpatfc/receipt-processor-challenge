using ReceiptProcessor.Models;
using ReceiptProcessor.Controller;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace ReceiptProcessor.Tests
{
    public class ReceiptProcessorTests
    {
        private static ReceiptProcessorController receiptProcessorController = new ReceiptProcessorController();
        private static int curId = -1;

        [Fact]
        public void ProcessReceipts_Succesfully_Returns_ReceiptID()
        {
            ProcessReceiptsInput processReceiptsInput = new ProcessReceiptsInput();
            processReceiptsInput.retailer = "Target";
            processReceiptsInput.purchaseDate = "2022-01-01";
            processReceiptsInput.purchaseTime = "13:01";
            processReceiptsInput.items = new Item[2];
            processReceiptsInput.items[0] = new Item("Mountain Dew 12PK", "6.49");
            processReceiptsInput.items[1] = new Item("Emils Cheese Pizza", "12.25");
            processReceiptsInput.total = "35.35";

            // Assert
            ActionResult<ProcessReceiptsOutput> result = Make_Process_Receipt_Call(processReceiptsInput);
            Assert.NotNull(result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            ProcessReceiptsOutput output = (ProcessReceiptsOutput)okResult.Value;
            Assert.NotNull(output);
            Assert.Equal(curId, output.id);
        }

        [Fact]
        public void Succesfully_Returns_Points_By_ID()
        {
            ProcessReceiptsInput processReceiptsInput = new ProcessReceiptsInput();
            processReceiptsInput.retailer = "Target";
            processReceiptsInput.purchaseDate = "2022-01-01";
            processReceiptsInput.purchaseTime = "14:01";

            processReceiptsInput.items = new Item[5];
            processReceiptsInput.items[0] = new Item("Mountain Dew 12PK", "6.49");
            processReceiptsInput.items[1] = new Item("Emils Cheese Pizza", "12.25");
            processReceiptsInput.items[2] = new Item("Knorr Creamy Chicken", "1.26");
            processReceiptsInput.items[3] = new Item("Doritos Nacho Cheese", "3.35");
            processReceiptsInput.items[4] = new Item("   Klarbrunn 12-PK 12 FL OZ  ", "12.00");

            processReceiptsInput.total = "35.00";

            ActionResult<ProcessReceiptsOutput> res = Make_Process_Receipt_Call(processReceiptsInput);
            var okRes = res.Result as OkObjectResult;

            // Assert
            ActionResult<GetPoints> result = receiptProcessorController.GetPoints(((ProcessReceiptsOutput)okRes.Value).id);
            Assert.NotNull(result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            GetPoints output = (GetPoints)okResult.Value;
            Assert.NotNull(output);
            Assert.Equal(113, output.points);
        }

        private ActionResult<ProcessReceiptsOutput> Make_Process_Receipt_Call(ProcessReceiptsInput processReceiptsInput)
        {
            curId++;
            return receiptProcessorController.ProcessReceipt(processReceiptsInput);
        }
    }
}
