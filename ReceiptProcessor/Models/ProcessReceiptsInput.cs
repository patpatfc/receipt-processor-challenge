using System.ComponentModel.DataAnnotations;

namespace ReceiptProcessor.Models
{
    public class ProcessReceiptsInput
    {

        [Required]
        public string retailer { get; set; }

        [Required]
        public string purchaseDate { get; set; }

        [Required]
        public string purchaseTime { get; set; }

        [Required]
        public Item[] items { get; set; }

        [Required]
        public string total { get; set; }
    }

    public class Item
    {

        public Item(string shortDescription, string price)
        {
            this.shortDescription = shortDescription;
            this.price = price;
        }
        
        [Required]
        public string shortDescription { get; set; }

        [Required]
        public string price { get; set; }
    }
}
