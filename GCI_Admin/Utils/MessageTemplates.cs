using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public static class MessageTemplates
    {
        public static string GenerateOtpMessage(string appName, string otpCode, int validMinutes = 10)
        {
            return $"Hi! {appName} here. Your verification code is: {otpCode}\n" +
                   $"It will expire in {validMinutes} minutes. Do not share this code with anyone.";
        }

        public static string TitheThankYou(string memberName, string amount, string churchName)
        {
            return $"Dear {memberName}, thank you for your faithful tithe of KES {amount} to {churchName}. " +
                   "Your generosity blesses many lives. May God continue to prosper you!";
        }

        public static string RCPsPledgeConfirmation(string memberName, string pledgeAmount, string rcpName)
        {
            return $"Hello {memberName}, we have received your pledge of KES {pledgeAmount} for the {rcpName} program. " +
                   "Thank you for your commitment and support!";
        }

        public static string RCPsRedemptionConfirmation(string memberName, decimal redeemedAmount, decimal balanceAmount, string rcpName)
        {
            if (balanceAmount <= 0)
            {
                return $"Hi {memberName}, you have successfully completed your {rcpName} pledge with a final payment of KES {redeemedAmount:N0}.\n" +
                       "We sincerely appreciate your commitment and support. May God richly bless you!";
            }

            return $"Hi {memberName}, you have successfully redeemed KES {redeemedAmount:N0} from your {rcpName} pledge.\n" +
                   $"Your remaining balance is KES {balanceAmount:N0}.\n" +
                   "Thank you for your continued commitment!";
        }

        public static string BenevolenceJoin(string memberName, string churchName = "GCI Central")
        {
            return $"Hello {memberName},\n\nWelcome to the {churchName} Benevolence program. " +
                   "You are now part of a caring community that stands together in times of need.\n\n" +
                   "This cover ensures that you and your loved ones are supported when it matters most. " +
                   "Thank you for choosing to be part of this family.";
        }

        public static string BenevolencePayment(string memberName, string amount)
        {
            return $"Hi {memberName}, we have received your benevolence payment of KES {amount}. " +
                   "Thank you for your kindness in supporting those in need!";
        }
    }
}
