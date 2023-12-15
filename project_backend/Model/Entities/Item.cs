namespace project_backend.Model.Entities
{
    public class Item
    {
        public int Item_Id { get; set; }
        public string Item_Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime Created_At { get; set; }
        public string Created_By { get; set; }
        public DateTime Modified_At { get; set; }
        public string Modified_By { get; set; }
        public bool Is_Deleted { get; set; }

    }
}
