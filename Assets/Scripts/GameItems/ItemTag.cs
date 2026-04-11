namespace GameItems
{
    public readonly struct ItemTag
    {
        public ItemTag(string id)
        {
            Id = id ?? string.Empty;
        }

        public string Id { get; }
    }
}
