namespace project_backend.DTOs.ResponseDTO
{
    public class GetPurchaseResponse
    {
        public int Purchase_Id { get; set; }

        public int User_Id { get; set; }

        public string User_Name { get; set; }

        public int Item_Id { get; set; }

        public string Item_Name { get; set; }

        public decimal Price { get; set; }

        public DateTime Created_At { get; set; }
    }
}

