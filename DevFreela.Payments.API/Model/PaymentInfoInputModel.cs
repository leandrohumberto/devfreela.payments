namespace DevFreela.Payments.API.Model
{
    public class PaymentInfoInputModel
    {
        public PaymentInfoInputModel(int idProject, string creditCardNumber, string cvv, string expiresAt, string fullName, decimal amount)
        {
            if (string.IsNullOrWhiteSpace(creditCardNumber)) 
            {
                throw new ArgumentException($"'{nameof(creditCardNumber)}' cannot be null or whitespace.", nameof(creditCardNumber));
            }

            if (string.IsNullOrWhiteSpace(cvv))
            {
                throw new ArgumentException($"'{nameof(cvv)}' cannot be null or whitespace.", nameof(cvv));
            }

            if (string.IsNullOrWhiteSpace(expiresAt))
            {
                throw new ArgumentException($"'{nameof(expiresAt)}' cannot be null or whitespace.", nameof(expiresAt));
            }

            if (string.IsNullOrWhiteSpace(fullName))
            {
                throw new ArgumentException($"'{nameof(fullName)}' cannot be null or whitespace.", nameof(fullName));
            }

            if (amount <= 0.0M)
            {
                throw new ArgumentException($"'{nameof(amount)}' cannot be less than or equal to zero.", nameof(amount));
            }

            IdProject = idProject;
            CreditCardNumber = creditCardNumber;
            Cvv = cvv;
            ExpiresAt = expiresAt;
            FullName = fullName;
            Amount = amount;
        }

        public int IdProject { get; private set; }
        public string CreditCardNumber { get; private set; }
        public string Cvv { get; private set; }
        public string ExpiresAt { get; private set; }
        public string FullName { get; private set; }
        public decimal Amount { get; private set; }
    }
}
