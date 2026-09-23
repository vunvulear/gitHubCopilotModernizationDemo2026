using System;
using System.Configuration;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using ContosoUniversity.Models;
using Newtonsoft.Json;

namespace ContosoUniversity.Services
{
    /// <summary>
    /// Azure Service Bus-backed implementation of <see cref="INotificationService"/>. Replaces the
    /// former MSMQ private queue (which required a broad "Everyone: FullControl" ACL to work at
    /// all, and does not exist on Azure hosting) with a Service Bus queue, preserving the same
    /// admin notification workflow and queue-style buffering: <see cref="SendNotification"/>
    /// enqueues, <see cref="ReceiveNotification"/> destructively dequeues one pending notification
    /// at a time (returning <c>null</c> when the queue is currently empty), exactly like the
    /// original <c>MessageQueue.Receive(TimeSpan)</c> / IOTimeout becd "C:\Users\vunvulear\source\repos\vunvulear\gitHubCopilotModernizationDemo2026\ContosoUniversity"
git checkout -B modernization
git add -A
git commit -m "Modernization"
git push -u origin modernizationhavior.
    ///
    /// Authenticates with <see cref="DefaultAzureCredential"/> (Managed Identity in Azure, developer
    /// credentials locally) - no connection string or shared access key is used, and no queue
    /// permissions are granted beyond what the app's own identity is assigned via Azure RBAC
    /// (Azure Service Bus Data Sender/Receiver), eliminating the previous "Everyone" queue ACL.
    ///
    /// The Service Bus namespace/queue name are read from application settings (see Web.config),
    /// never hardcoded. The underlying <see cref="ServiceBusClient"/>/sender/receiver are thread-safe
    /// Azure SDK types and are shared for the lifetime of the process (mirrors the
    /// AzureBlobStorageService pattern used for teaching-material storage in task 004).
    /// </summary>
    public class NotificationService : INotificationService
    {
        private static readonly Lazy<string> LazyQueueName = new Lazy<string>(ResolveQueueName);
        private static readonly Lazy<ServiceBusClient> LazyClient = new Lazy<ServiceBusClient>(CreateClient);
        private static readonly Lazy<ServiceBusSender> LazySender = new Lazy<ServiceBusSender>(() => Client.CreateSender(QueueName));
        private static readonly Lazy<ServiceBusReceiver> LazyReceiver = new Lazy<ServiceBusReceiver>(() => Client.CreateReceiver(QueueName));

        private static string QueueName => LazyQueueName.Value;
        private static ServiceBusClient Client => LazyClient.Value;
        private static ServiceBusSender Sender => LazySender.Value;
        private static ServiceBusReceiver Receiver => LazyReceiver.Value;

        private static string ResolveQueueName()
        {
            var queueName = ConfigurationManager.AppSettings["AzureServiceBus:NotificationQueueName"];
            return string.IsNullOrWhiteSpace(queueName) ? "contosouniversity-notifications" : queueName;
        }

        private static ServiceBusClient CreateClient()
        {
            var fullyQualifiedNamespace = ConfigurationManager.AppSettings["AzureServiceBus:FullyQualifiedNamespace"];
            if (string.IsNullOrWhiteSpace(fullyQualifiedNamespace))
            {
                throw new InvalidOperationException(
                    "Missing required application setting 'AzureServiceBus:FullyQualifiedNamespace'. Set it to " +
                    "the provisioned Azure Service Bus namespace (see infra/infra-config.md once the " +
                    "platform-engineer task has run) before sending or receiving admin notifications.");
            }

            return new ServiceBusClient(fullyQualifiedNamespace, new DefaultAzureCredential());
        }

        public void SendNotification(string entityType, string entityId, EntityOperation operation, string userName = null)
        {
            SendNotification(entityType, entityId, null, operation, userName);
        }

        public void SendNotification(string entityType, string entityId, string entityDisplayName, EntityOperation operation, string userName = null)
        {
            try
            {
                var notification = new Notification
                {
                    EntityType = entityType,
                    EntityId = entityId,
                    Operation = operation.ToString(),
                    Message = GenerateMessage(entityType, entityId, entityDisplayName, operation),
                    CreatedAt = DateTime.Now,
                    CreatedBy = userName ?? "System",
                    IsRead = false
                };

                // No routing key is used for admin notifications (single plain queue, no
                // topic/subscription filtering), so the message Subject is intentionally left unset
                // per the Azure Service Bus migration guidance.
                var jsonMessage = JsonConvert.SerializeObject(notification);
                var message = new ServiceBusMessage(jsonMessage);

                Sender.SendMessageAsync(message).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                // Log error but don't break the main operation
                System.Diagnostics.Debug.WriteLine($"Failed to send notification: {ex.Message}");
            }
        }

        public Notification ReceiveNotification()
        {
            try
            {
                // Mirrors the original MessageQueue.Receive(TimeSpan.FromSeconds(1)) behavior: wait
                // briefly for a pending message and return null (no throw) if the queue is empty.
                var message = Receiver.ReceiveMessageAsync(TimeSpan.FromSeconds(1)).GetAwaiter().GetResult();
                if (message == null)
                {
                    // No messages available (equivalent of the former IOTimeout case)
                    return null;
                }

                var jsonContent = message.Body.ToString();

                // Complete (remove) the message from the queue, matching the destructive-read
                // semantics of the original MessageQueue.Receive call.
                Receiver.CompleteMessageAsync(message).GetAwaiter().GetResult();

                return JsonConvert.DeserializeObject<Notification>(jsonContent);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to receive notification: {ex.Message}");
                return null;
            }
        }

        public void MarkAsRead(int notificationId)
        {
            // In a real implementation, you might want to store notifications in database as well
            // for persistence and tracking read status
        }

        private string GenerateMessage(string entityType, string entityId, string entityDisplayName, EntityOperation operation)
        {
            var displayText = !string.IsNullOrWhiteSpace(entityDisplayName) 
                ? $"{entityType} '{entityDisplayName}'" 
                : $"{entityType} (ID: {entityId})";

            switch (operation)
            {
                case EntityOperation.CREATE:
                    return $"New {displayText} has been created";
                case EntityOperation.UPDATE:
                    return $"{displayText} has been updated";
                case EntityOperation.DELETE:
                    return $"{displayText} has been deleted";
                default:
                    return $"{displayText} operation: {operation}";
            }
        }

        public void Dispose()
        {
            // The ServiceBusClient/sender/receiver are process-wide singletons shared across every
            // NotificationService/BaseController instance (each MVC request constructs a new
            // controller and, in turn, a new NotificationService), so they must not be disposed
            // per-request. Nothing instance-scoped needs cleanup here.
        }
    }
}
