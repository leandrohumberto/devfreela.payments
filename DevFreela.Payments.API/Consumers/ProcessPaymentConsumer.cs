using DevFreela.Payments.API.Model;
using DevFreela.Payments.API.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace DevFreela.Payments.API.Consumers
{
    public class ProcessPaymentConsumer : BackgroundService
    {
        private const string PAYMENTS_QUEUE = "Payments";
        private const string PAYMENTS_APPROVED_QUEUE = "PaymentsApproved";
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceProvider _serviceProvider;

        public ProcessPaymentConsumer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            var connectionFactory = new ConnectionFactory()
            {
                HostName= "localhost",
            };

            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: PAYMENTS_QUEUE,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _channel.QueueDeclare(
                queue: PAYMENTS_APPROVED_QUEUE,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
             var consumer = new EventingBasicConsumer(_channel);
            
            consumer.Received += async (sender, eventArgs) =>
            {
                var byteArray = eventArgs.Body.ToArray();
                var paymentInfoJson = Encoding.UTF8.GetString(byteArray);
                var paymentInfo = JsonSerializer.Deserialize<PaymentInfoInputModel>(paymentInfoJson);

                if (paymentInfo != null && await ProcessPayment(paymentInfo, stoppingToken))
                {
                    var paymentApproved = new PaymentApprovedIntegrationEvent(paymentInfo.IdProject);
                    var paymenyApprovedJSon = JsonSerializer.Serialize(paymentApproved);
                    var paymentApprovedBytes = Encoding.UTF8.GetBytes(paymenyApprovedJSon);

                    _channel.BasicPublish(
                        exchange: "",
                        routingKey: PAYMENTS_APPROVED_QUEUE,
                        basicProperties: null,
                        body: paymentApprovedBytes);
                }
                else
                {
                    // Publicar mensagem em uma fila de erros
                }

                _channel.BasicAck(eventArgs.DeliveryTag, false);
            };

            _channel.BasicConsume(
                queue: PAYMENTS_QUEUE,
                autoAck: false,
                consumer: consumer);

            return Task.CompletedTask;
        }

        private async Task<bool> ProcessPayment(PaymentInfoInputModel paymentInfo, CancellationToken cancellationToken = default)
        {
            using var scope = _serviceProvider.CreateScope();
            var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();

            if (paymentService != null)
            {
                return await paymentService.Process(paymentInfo, cancellationToken);
            }

            return false;
        }
    }
}
