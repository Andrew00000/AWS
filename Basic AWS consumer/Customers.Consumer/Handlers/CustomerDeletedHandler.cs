using Customers.Consumer.Messages;
using MediatR;

namespace Customers.Consumer.Handlers
{
    public class CustomerDeletedHandler : IRequestHandler<CustomerDeleted>
    {
        private readonly ILogger<CustomerDeletedHandler> logger;

        public CustomerDeletedHandler(ILogger<CustomerDeletedHandler> logger)
        {
            this.logger = logger;
        }
        public Task<Unit> Handle(CustomerDeleted request, CancellationToken cancellationToken)
        {
            logger.LogInformation(request.Id.ToString());
            return Unit.Task;
        }
    }
}
