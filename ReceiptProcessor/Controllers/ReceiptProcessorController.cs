using ReceiptProcessor.Models;
using Microsoft.AspNetCore.Mvc;

namespace ReceiptProcessor.Controller
{
    [ApiController]
    public class ReceiptProcessorController : ControllerBase
    {

        // dictionary to store the points for each receipt id
        private static Dictionary<long, int> _dictionary = new Dictionary<long, int>();
        
        // unique id to assign to each receipt
        private static long id = -1;

        [HttpPost("/receipts/process")]
        public ActionResult<ProcessReceiptsOutput> ProcessReceipt(ProcessReceiptsInput processReceiptsDto)
        {
            int points = 0;

            // One point for every alphanumeric character in the retailer name.
            points += processReceiptsDto.retailer.Count(char.IsLetterOrDigit);

            // 50 points if the total is a round dollar amount with no cents.
            if (processReceiptsDto.total.Contains(".00"))
                points += 50;

            // 25 points if the total is evenly divisible by 0.25.
            if (double.Parse(processReceiptsDto.total) % 0.25 == 0)
                points += 25;
            
            // 5 points for every two items on the receipt
            points += 5 * (processReceiptsDto.items.Length / 2);
            
            // If the trimmed length of the item description is a multiple of 3, the
            // price multiplied by 0.2 and round up to the nearest integer is earned
            foreach (Item i in processReceiptsDto.items)
                if (i.shortDescription.Trim().Length % 3 == 0)
                    points += (int) Math.Ceiling(double.Parse(i.price) * 0.2);
            
            // 6 points if the day of the month is odd
            if (int.Parse(processReceiptsDto.purchaseDate.Split("-")[2]) % 2 == 1)
                points += 6;
            
            // 10 points if the purchase time is between 2:00 PM and 4:00 PM
            if (int.Parse(processReceiptsDto.purchaseTime.Split(":")[0]) >= 14 && int.Parse(processReceiptsDto.purchaseTime.Split(":")[0]) < 16)
                points += 10;
            
            // assign unique id to receipt
            id++;
            _dictionary[id] = points;

            // return id and OK status
            return Ok(new ProcessReceiptsOutput(id));
        }

        [HttpGet("/receipts/{id}/points")]
        public ActionResult<GetPoints> GetPoints(long id)
        {
            // if the id exists in the dictionary, return the points else return not found
            if (_dictionary.ContainsKey(id)) {
                return Ok(new GetPoints(_dictionary[id]));
            }
            return NotFound();
        }

    }
}
