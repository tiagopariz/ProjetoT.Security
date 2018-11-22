using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ProjetoT.Security.Domain.Entities.OAuth
{
    public class Scope
    {
        private static List<Scope> _AllScopes = new List<Scope>();
        public static IReadOnlyList<Scope> AllScopes => _AllScopes;

        public readonly string Name;
        public readonly string Description;

        private Scope(string name, string description)
        {
            if (NameInScopes(name))
            {
                throw new DuplicateNameException($"Tried to add an OAuthScope with the name {name}, but this name already existed");
            }

            this.Name = name;
            this.Description = description;
            _AllScopes.Add(this);
        }

        public static bool NameInScopes(string name)
        {
            return _AllScopes.Any(x => x.Name == name);
        }

        public static Scope GetScope(string name)
        {
            return _AllScopes.First(x => x.Name == name);
        }

        public static readonly Scope UserReadEmail = new Scope("user-read-email", "Permission to know your email address");
        public static readonly Scope UserReadBirthdate = new Scope("user-read-birthdate", "Permission to know your birthdate");
        public static readonly Scope UserModifyEmail = new Scope("user-modify-email", "Permission to change your email address");
        public static readonly Scope UserModifyBirthdate = new Scope("user-modify-birthdate", "Permission to change your birthdate");
    }
}
