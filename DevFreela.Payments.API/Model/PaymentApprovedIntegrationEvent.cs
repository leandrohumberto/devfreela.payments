namespace DevFreela.Payments.API.Model
{
    public class PaymentApprovedIntegrationEvent
    {
        public int IdProject { get; private set; }

        public PaymentApprovedIntegrationEvent(int idproject) => IdProject = idproject;
    }
}
