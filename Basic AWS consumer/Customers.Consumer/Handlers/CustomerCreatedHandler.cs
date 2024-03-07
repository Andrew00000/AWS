using Customers.Consumer.Messages;
using MediatR;

namespace Customers.Consumer.Handlers
{
    public class CustomerCreatedHandler : IRequestHandler<CustomerCreated>
    {
        private readonly ILogger<CustomerCreatedHandler> logger;

        public CustomerCreatedHandler(ILogger<CustomerCreatedHandler> logger)
        {
            this.logger = logger;
        }

        public Task<Unit> Handle(CustomerCreated request, CancellationToken cancellationToken)
        {
            logger.LogInformation(request.FullName);
            return Unit.Task;
        }
    }
}
