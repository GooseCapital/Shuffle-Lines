using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuffleLines
{
    class Account
    {
        public readonly string Username;
        public readonly string Password;
        public readonly string Password2;

        public Account(string username, string password, string password2)
        {
            Username = username;
            Password = password;
            Password2 = password2;
        }
    }
}
