namespace project_backend.DTOs.RequestDTO
{
    public class AddNewItem
    {

        public string Name { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public string Created_By { get; set; }
    }
}
