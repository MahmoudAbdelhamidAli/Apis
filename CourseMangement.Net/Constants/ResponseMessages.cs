using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseMangement.Net.Constants
{
    public class ResponseMessages
    {
        public const string UserAlreadyExists = "User already exists";
        public const string UserCreatedSuccessfully = "User created successfully, and email confirmation sent";
        public const string EmailConfirmedSuccessfully = "Email confirmed successfully";
        public const string UserNotFound = "User not found";
        public const string PasswordResetSuccessfully = "Password reset successfully";
        public const string PasswordResetLinkSent = "Password reset link sent to your email";
        public const string UnauthorizedAccess = "User is not authenticated.";
        public const string FailedToGenerateResetLink = "Failed to generate the reset password link.";
        public const string InvalidModelState = "Invalid model state.";
        public const string UserDeletedSuccessfully = "User deleted successfully";
        public const string EmailSendingFailed = "Failed to send email.";

        public const string InvalidUserNameOrPassword = "Invalid username or password";

        public const string CourseAddedSuccessfully = "Course added successfully";

        public const string InternalServerError = "Internal server error";

        // Course
        public const string CourseNotFound = "Course not found";
        public const string CourseAlreadyEnrolled = "Course already enrolled";
        public const string CourseEnrolledSuccessfully = "Course enrolled successfully";

    }
}