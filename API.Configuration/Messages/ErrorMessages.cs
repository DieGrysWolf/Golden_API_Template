using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Configuration.Messages
{
    public static class ErrorMessages
    {
        public static class Generic
        {
            public static string InvalidBody = "Invalid Body";
            public static string RequestFailed = "Request Failed";
        }

        public static class NotFound
        {
            public static string AccountNotFound = "Account Not Found";
            public static string ProfileNotFound = "Profile Not Found";
        }

        // Houses all possible types
        public static class Types
        {
            public static string NotFound = "Not Found";
            public static string BadRequest = "Bad Request";
        }
    }
}
