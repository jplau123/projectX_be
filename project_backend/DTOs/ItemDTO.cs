namespace project_backend.DTOs
{
    public class ItemDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Amount { get; set; }
        public DateTime Created_At { get; set; }
        public string Created_By { get; set; }
        public DateTime Modified_At { get; set; }
        public string Modified_By { get; set; }
    }
}
