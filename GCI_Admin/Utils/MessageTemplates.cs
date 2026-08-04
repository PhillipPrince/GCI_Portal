using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public static class MessageTemplates
    {
        public static string GenerateOtpMessage(string userName, string otpCode, int validMinutes = 10)
        {
            return $"Dear {userName},\n\n" +
                   $"Your verification code is: {otpCode}\n" +
                   $"This code will expire in {validMinutes} minutes.\n\n" +
                   $"Please do not share this code with anyone.\n\n" +
                   $"Blessings,\nGCI Central";
        }

        public static string GenerateCollectionVerificationOtpMessage(string userName, string otpCode, int validMinutes = 10)
        {
            return $"Dear {userName},\n\n" +
                   $"Please use the following OTP to verify the collection: {otpCode}\n" +
                   $"This OTP will expire in {validMinutes} minutes.\n\n" +
                   $"Do not share this code with anyone.\n\n" +
                   $"Blessings,\nGCI Central";
        }

        public static string TitheThankYou(string memberName, string amount, string churchName = "GCI Central")
        {
            return $"Dear {memberName},\n\n" +
                   $"Thank you for your faithful tithe of KES {amount} to {churchName}.\n\n" +
                   $"Your generosity blesses many lives. May God continue to prosper you!\n\n" +
                   $"Blessings,\n{churchName}";
        }

        public static string RCPsPledgeConfirmation(string memberName, string pledgeAmount, string rcpName, string churchName = "GCI Central")
        {
            return $"Dear {memberName},\n\n" +
                   $"We have received your pledge of KES {pledgeAmount} for the {rcpName} program.\n\n" +
                   $"Thank you for your commitment and support!\n\n" +
                   $"Blessings,\n{churchName}";
        }

        public static string RCPsRedemptionConfirmation(string memberName, decimal redeemedAmount, decimal? balanceAmount, string rcpName, string churchName = "GCI Central")
        {
            if (balanceAmount <= 0 || balanceAmount == null)
            {
                return $"Dear {memberName},\n\n" +
                       $"You have successfully completed your {rcpName} pledge with a final Collection of KES {redeemedAmount:N0}.\n\n" +
                       $"We sincerely appreciate your commitment and support. May God richly bless you!\n\n" +
                       $"Blessings,\n{churchName}";
            }

            return $"Dear {memberName},\n\n" +
                   $"You have successfully redeemed KES {redeemedAmount:N0} from your {rcpName} pledge.\n" +
                   $"Your remaining balance is KES {balanceAmount:N0}.\n\n" +
                   $"Thank you for your continued commitment!\n\n" +
                   $"Blessings,\n{churchName}";
        }

        public static string BenevolenceJoin(string memberName, string churchName = "GCI Central")
        {
            return $"Dear {memberName},\n\n" +
                   $"Welcome to the {churchName} Benevolence program.\n\n" +
                   $"You are now part of a caring community that stands together in times of need.\n\n" +
                   $"This cover ensures that you and your loved ones are supported when it matters most.\n\n" +
                   $"Thank you for choosing to be part of this family.\n\n" +
                   $"Blessings,\n{churchName}";
        }

        public static string BenevolenceCollection(string memberName, string amount, string churchName = "GCI Central")
        {
            return $"Dear {memberName},\n\n" +
                   $"We have received your benevolence Collection of KES {amount}.\n\n" +
                   $"Thank you for your kindness in supporting those in need!\n\n" +
                   $"Blessings,\n{churchName}";
        }

        public static string CareRequestPicked(string memberName, string churchName = "GCI Central")
        {
            return $"Dear {memberName},\n\n" +
                   $"We've received your care request and want you to know that someone from our team is already working on it.\n\n" +
                   $"You will be contacted shortly for support. If you need immediate assistance, please feel free to call us on 0725255941.\n\n" +
                   $"You are not alone. Thank you for reaching out to the Church.\n\n" +
                   $"Blessings,\n{churchName}";
        }

        public static string EventCollectionConfirmation(
            string memberName,
            string eventName,
            decimal amount,
            string receiptNumber,
            string churchName = "GCI Central")
        {
            return $"Dear {memberName},\n\n" +
                   $"Your Collection of KES {amount:N0} for \"{eventName}\" has been successfully received.\n\n" +
                   $"M-Pesa Receipt: {receiptNumber}\n\n" +
                   $"Thank you for your participation. We look forward to seeing you!\n\n" +
                   $"Blessings,\n{churchName}";
        }

        public static string AccountUpdatedMessage(string memberName, string appName, string password, string churchName = "GCI Central")
        {
            return $"Dear {memberName},\n\n" +
                   $"Your {appName} account details have been updated successfully.\n\n" +
                   $"You can now login using your password: {password}\n\n" +
                   $"Do not share your password with anyone.\n\n" +
                   $"Blessings,\n{churchName}";
        }

        public static string GetSignatureRequestMessage(
            string memberName,
            string meetingType,
            DateTime meetingDate,
            string otpCode,
            int validMinutes = 10,
            string churchName = "GCI Central")
        {
            return $"Dear {memberName},\n\n" +
                   $"You have been selected to verify and sign the {meetingType} collection record held on {meetingDate:dddd, MMMM dd, yyyy}.\n\n" +
                   $"Your One-Time Password (OTP) is: {otpCode}\n" +
                   $"This code is valid for {validMinutes} minutes.\n\n" +
                   $"Please do NOT share this code with anyone. It is required to confirm your signature for this record.\n\n" +
                   $"Blessings,\n{churchName}";
        }

        public static string WelcomeNewMember(string memberName, string churchName = "GCI Central")
        {
            return $"Dear {memberName},\n\n" +
                   $"Welcome to {churchName} family!\n\n" +
                   $"We are delighted to have you join our community. Your presence is a blessing to us all.\n\n" +
                   $"Feel free to reach out to us if you need any assistance or have any questions.\n\n" +
                   $"Blessings,\n{churchName}";
        }

        public static string MinistryAppointmentConfirmation(string memberName, string ministryName, string position, string churchName = "GCI Central")
        {
            return $"Dear {memberName},\n\n" +
                   $"Congratulations! You have been appointed as {position} in the {ministryName} ministry.\n\n" +
                   $"We believe in your ability to serve and make a difference. May God guide you in this new role.\n\n" +
                   $"Blessings,\n{churchName}";
        }
    }
}