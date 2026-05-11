namespace RetailsEcosystem.Customer.Shared.Enums
{
    public enum PaymentStatus
    {
        Pending         = 0,  // order created, no payment attempt yet
        AwaitingPayment = 1,  // VNPay URL built, browser redirected
        Paid            = 2,  // IPN confirmed success
        Failed          = 3,  // IPN confirmed failure
        Cancelled       = 4,  // user cancelled on VNPay (rc=24)
        Expired         = 5,  // vnp_ExpireDate passed without IPN
        Abandoned       = 6,  // AwaitingPayment > 30 min with no resolution
    }
}
