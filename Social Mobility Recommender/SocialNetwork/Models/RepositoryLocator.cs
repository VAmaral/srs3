using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SocialNetwork.Models;

namespace SocialNetwork.Models
{
    public class RepositoryLocator
    {
        private readonly static IUserRepository repository = new UserRepository();

        public static IUserRepository GetRepository() { return repository; }

    }
}