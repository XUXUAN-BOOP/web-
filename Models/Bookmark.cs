using System;
using System.Collections.Generic;

namespace NetFavorite.Models;

public partial class Bookmark
{
    public Guid Bookmark_Id { get; set; }

    public string Bookmark_Address { get; set; } = null!;

    public string? Bookmark_Title { get; set; }

    public DateTime Bookmark_CreateTime { get; set; }
}
