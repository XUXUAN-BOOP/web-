using System;
using System.Collections.Generic;

namespace NetFavorite.Models;

public partial class LoginUser
{
    public Guid LoginUser_Id { get; set; }

    public string LoginUser_Account { get; set; } = null!;

    public string LoginUser_Password { get; set; } = null!;

    public string LoginUser_Role { get; set; } = null!;

    public string? LoginUser_Salt { get; set; }
}
