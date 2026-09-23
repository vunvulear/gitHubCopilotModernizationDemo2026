# Real-Time Admin Notification System

This project now includes a real-time notification system that alerts administrators whenever entity operations (create, update, delete) are performed in the system.

## Overview

The notification system uses an **Azure Service Bus queue** as the underlying technology to provide reliable, real-time notifications to administrators. The application authenticates to Azure Service Bus using **Managed Identity** (via `DefaultAzureCredential`) - no connection string or shared access key is stored anywhere in the app.

## Features

- **Real-time notifications**: Admins receive immediate notifications when entities are modified
- **Entity coverage**: Monitors Students, Courses, Instructors, and Departments
- **Operation tracking**: Tracks CREATE, UPDATE, and DELETE operations
- **Admin-only**: Only users with administrator role receive notifications
- **Non-intrusive UI**: Notifications appear in the top-right corner with auto-dismiss
- **Reliable delivery**: Uses an Azure Service Bus queue for guaranteed message delivery and buffering

## How It Works

### Backend Components

1. **NotificationService**: Handles Azure Service Bus operations for sending/receiving messages
2. **BaseController**: Base class that all controllers inherit from to send notifications
3. **Notification Model**: Entity to represent notification data
4. **NotificationsController**: API endpoints for retrieving notifications

### Frontend Components

1. **notifications.css**: Styling for notification UI elements
2. **notifications.js**: JavaScript polling system that checks for new notifications
3. **Layout integration**: Admin-only inclusion of notification assets

### Technology Stack

- **Azure Service Bus**: Message queue technology (`Azure.Messaging.ServiceBus`), authenticated with Managed Identity
- **Entity Framework**: Data access for notification persistence
- **ASP.NET MVC**: Web framework
- **JavaScript/jQuery**: Frontend polling and UI updates
- **Bootstrap**: UI styling

## Configuration

The notification system is configured in `Web.config`:

```xml
<appSettings>
    <add key="AzureServiceBus:FullyQualifiedNamespace" value="<your-namespace>.servicebus.windows.net"/>
    <add key="AzureServiceBus:NotificationQueueName" value="contosouniversity-notifications"/>
</appSettings>
```

Replace the `AzureServiceBus:FullyQualifiedNamespace` placeholder with the provisioned Azure Service Bus namespace (see `infra/infra-config.md` once the platform-engineer task has provisioned it). No secret material (connection strings, shared access keys) is stored in configuration.

## Queue Details

- **Queue Name**: `contosouniversity-notifications` (configurable via `AzureServiceBus:NotificationQueueName`)
- **Queue Type**: Azure Service Bus queue, provisioned ahead of time by the platform-engineer/infrastructure task
- **Permissions**: The application's Managed Identity is granted the minimum required Azure RBAC roles (Azure Service Bus Data Sender + Azure Service Bus Data Receiver) on the queue - no broad "Everyone: Full Control" ACL, unlike the previous MSMQ implementation
- **Message Format**: JSON serialized notification objects (message body), notification label carried in the message `Subject`

## Usage

### For Administrators

1. Log in with an administrator account
2. Navigate to **Notifications** in the main menu to view the dashboard
3. Perform any CRUD operation on entities (Students, Courses, Instructors, Departments)
4. Watch for notifications appearing in the top-right corner
5. Notifications auto-dismiss after 1 minute or can be manually closed

### For Developers

To add notification support to a new controller:

1. Inherit from `BaseController` instead of `Controller`
2. Remove the private `SchoolContext db` declaration (handled by base class)
3. Call `SendEntityNotification()` after successful save operations:

```csharp
// Example: After creating a student
db.Students.Add(student);
db.SaveChanges();
SendEntityNotification("Student", student.ID.ToString(), EntityOperation.CREATE);
```

## Notification Types

- **CREATE**: Green notification for entity creation
- **UPDATE**: Blue notification for entity updates  
- **DELETE**: Orange notification for entity deletion

## System Requirements

- An Azure Service Bus namespace/queue provisioned ahead of time, and a Managed Identity (or developer credential for local runs) granted Azure Service Bus Data Sender/Receiver on it
- .NET Framework 4.8
- SQL Server (for Entity Framework)

## Testing the System

1. Access the **Notifications** dashboard from the admin menu
2. Click on any of the "Create new..." buttons provided
3. Complete a create/edit/delete operation
4. Observe the notification appearing in the top-right corner

## Troubleshooting

### Common Issues

1. **Azure Service Bus namespace not configured**: Set `AzureServiceBus:FullyQualifiedNamespace` in `Web.config` (or via Azure App Configuration) to the provisioned namespace
2. **Authentication/authorization failures**: Ensure the app's Managed Identity (or the developer's local credential) has been granted the Azure Service Bus Data Sender/Receiver roles on the queue
3. **No notifications appearing**: Check browser console for JavaScript errors
4. **Queue not found**: Verify the queue name matches `AzureServiceBus:NotificationQueueName` and that the queue has been provisioned

### Development Notes

- Notifications are sent asynchronously and won't block main operations if Azure Service Bus is unreachable
- Failed notification sends are logged to debug output but don't affect user operations
- JavaScript polling occurs every 5 seconds
- Maximum of 5 notifications are displayed simultaneously

## Architecture Benefits

- **Decoupled**: The Service Bus queue ensures notifications don't affect main application performance
- **Reliable**: Messages persist in the queue even if the web application restarts
- **Scalable**: Works across multiple app instances/servers without a single point of failure (unlike a local MSMQ private queue)
- **Maintainable**: Clear separation between notification logic and business logic
- **Secure**: Managed Identity authentication with least-privilege Azure RBAC roles - no broad queue ACLs or secrets in configuration

## Future Enhancements

Potential improvements for production use:

1. **SignalR integration**: Real-time push notifications instead of polling
2. **Email notifications**: Send email alerts for critical operations
3. **Notification persistence**: Store notifications in database for audit trail
4. **User preferences**: Allow admins to configure notification types
5. **Batch operations**: Group related notifications to reduce noise
6. **Advanced filtering**: Filter notifications by entity type or operation
