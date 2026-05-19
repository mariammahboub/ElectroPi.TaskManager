using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Domain.Errors
{

    public static class ErrorCodes
    {
        public static class General
        {
            public const string NotFound = "GENERAL_NOT_FOUND";
            public const string ValidationFailed = "GENERAL_VALIDATION_FAILED";
            public const string Unauthorized = "GENERAL_UNAUTHORIZED";
            public const string Forbidden = "GENERAL_FORBIDDEN";
            public const string Conflict = "GENERAL_CONFLICT";
            public const string InternalError = "GENERAL_INTERNAL_ERROR";
        }

        public static class Auth
        {
            public const string InvalidCredentials = "AUTH_INVALID_CREDENTIALS";
            public const string EmailAlreadyExists = "AUTH_EMAIL_ALREADY_EXISTS";
            public const string AccountInactive = "AUTH_ACCOUNT_INACTIVE";
            public const string TokenExpired = "AUTH_TOKEN_EXPIRED";
            public const string TokenInvalid = "AUTH_TOKEN_INVALID";
        }

        public static class Project
        {
            public const string NotFound = "PROJECT_NOT_FOUND";
            public const string Forbidden = "PROJECT_FORBIDDEN";
            public const string NameConflict = "PROJECT_NAME_CONFLICT";
        }

        public static class Task
        {
            public const string NotFound = "TASK_NOT_FOUND";
            public const string Forbidden = "TASK_FORBIDDEN";
            public const string InvalidStatusTransition = "TASK_INVALID_STATUS_TRANSITION";
        }

        public static class User
        {
            public const string NotFound = "USER_NOT_FOUND";
            public const string Forbidden = "USER_FORBIDDEN";
        }
    }
}