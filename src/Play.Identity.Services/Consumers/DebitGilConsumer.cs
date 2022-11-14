using MassTransit;
using Microsoft.AspNetCore.Identity;
using Play.Identity.Contracts;
using Play.Identity.Services.Entities;
using Play.Identity.Services.Exceptions;

namespace Play.Identity.Services.Consumers;

public class DebitGilConsumer : IConsumer<DebitGil>
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly ILogger<DebitGilConsumer> logger;

    public DebitGilConsumer(UserManager<ApplicationUser> userManager, ILogger<DebitGilConsumer> logger)
    {
        this.userManager = userManager;
        this.logger = logger;
    }

    public async Task Consume(ConsumeContext<DebitGil> context)
    {
        var message = context.Message;

        logger.LogInformation(
            "Debiting {Gil} gil from User {UserId} with CorrelationId {CorrelationId}...",
            message.Gil,
            message.UserId,
            message.CorrelationId);

        var user = await userManager.FindByIdAsync(message.UserId.ToString());

        if (user is null)
            throw new UnknownUserException(message.UserId);

        if (user.MessageIds.Contains(context.MessageId.Value))
        {
            await context.Publish(new GilDebited(message.CorrelationId));
            return;
        }

        user.Gil -= message.Gil;

        if (user.Gil < 0)
        {
            logger.LogError(
                "Not enough gil to debit {Gil} gil from User {UserId} with CorrelationId {CorrelationId}.",
                message.Gil,
                message.UserId,
                message.CorrelationId);
            throw new InsufficientFoundsException(message.UserId, message.Gil);
        }

        user.MessageIds.Add(context.MessageId.Value);

        await userManager.UpdateAsync(user);

        var gilDebitedTask = context.Publish(new GilDebited(message.CorrelationId));
        var userUpdatedTask = context.Publish(new UserUpdated(user.Id, user.Email, user.Gil));

        await Task.WhenAll(userUpdatedTask, gilDebitedTask);

    }
}