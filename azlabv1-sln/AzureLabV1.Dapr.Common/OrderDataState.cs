namespace AzureLabV1.Dapr.Common
{
    public record OrderDataState
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Quantity { get; set; }
        public bool Processed { get; set; }
    }
}