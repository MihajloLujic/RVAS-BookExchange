namespace RVAS_BookExchange.Settings
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;

        public string UsersCollectionName { get; set; } = string.Empty;
        public string BooksCollectionName { get; set; } = string.Empty;
        public string RequestsCollectionName { get; set; } = string.Empty;
        public string ConversationsCollectionName { get; set; } = string.Empty;
    }
}
