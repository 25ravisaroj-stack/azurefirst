namespace product.Models
{
    public class Product
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } ="";
        public int Quantity { get; set; }
        public string ProductDescription { get; set; }="";
        public string Photo { get; set; } = "";
        public int? QntValue { get; set; }
    }
}