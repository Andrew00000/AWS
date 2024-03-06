using Customers.Consumer.Messages;
using MediatR;

namespace Customers.Consumer.Handlers
{
    public class CustomerUpdatedHandler : IRequestHandler<CustomerUpdated>
    {
        private readonly ILogger<CustomerUpdatedHandler> logger;

        public CustomerUpdatedHandler(ILogger<CustomerUpdatedHandler> logger)
        {
            this.logger = logger;
        }
        public Task<Unit> Handle(CustomerUpdated request, CancellationToken cancellationToken)
        {
            logger.LogInformation(request.GitHubUsername);
            return Unit.Task;
        }
    }
}
