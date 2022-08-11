using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Helpers
{
    public static class MessageHelper
    {

        internal static readonly string RecordNotFound = "Record not found.";
        internal static readonly string RecordToBeUpdatedNotFound = "The record you're trying to update was not found.";
        internal static readonly string RecordToBeUpdatedNotValid = "The record you're trying to update is not valid.";

        internal static string IsAlreadyRegistered(string value)
        {
            return string.Format("'{0}' is already registered.", value);
        }

        internal static readonly string IsNotValid = "'{PropertyName}' is not valid.";

        internal static readonly string NoRecordFound = "No record found.";

        internal static string InvalidPasswordHashLength = "Invalid length of password hash (64 bytes expected).";

        internal static string InvalidPasswordSaltLength = "Invalid length of password salt (128 bytes expected).";

        internal static string PasswordIsInvalid = "Value cannot be empty or whitespace only string.";

    }
}
