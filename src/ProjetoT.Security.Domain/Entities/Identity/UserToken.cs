using System;
using Microsoft.AspNetCore.Identity;

namespace ProjetoT.Security.Domain.Entities.Identity
{
    public class UserToken : IdentityUserToken<Guid> { }
}