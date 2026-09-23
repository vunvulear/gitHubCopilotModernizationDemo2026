# Setup and Testing Guide for Notification System

## Prerequisites

1. **Provision an Azure Service Bus queue**:
   - Ensure an Azure Service Bus namespace and a queue (default name `contosouniversity-notifications`) have been provisioned (see the infrastructure/platform-engineer task and `infra/infra-config.md` when present).
   - Grant the application's identity (Managed Identity in Azure, or your own developer identity for local runs via `az login` / Visual Studio sign-in) the **Azure Service Bus Data Sender** and **Azure Service Bus Data Receiver** roles on the namespace or queue.

2. **Configure the namespace**:
   - Set `AzureServiceBus:FullyQualifiedNamespace` in `Web.config` to `<your-namespace>.servicebus.windows.net`.
   - Optionally override `AzureServiceBus:NotificationQueueName` if you provisioned the queue under a different name.

## Building the Project

1. Open the solution in Visual Studio
2. Restore NuGet packages if prompted
3. Build the solution (Ctrl+Shift+B)

## Testing the Notification System

### Step 1: Run the Application
1. Press F5 to start debugging
2. The application will launch in your default browser

### Step 2: Login as Administrator
1. The application uses Microsoft Entra ID authentication
2. Ensure your account is in the administrator role
3. You should see "Administrator" label next to your username

### Step 3: Access Notification Dashboard
1. Click "Notifications" in the main navigation menu
2. This page explains the notification system and provides test links

### Step 4: Test Notifications
1. **Create a Student**:
   - Click "Students" → "Create New"
   - Fill in the form and submit
   - Watch for a green notification in the top-right corner

2. **Edit a Student**:
   - Go to Students list, click "Edit" on any student
   - Make changes and save
   - Watch for a blue notification

3. **Delete a Student**:
   - Go to Students list, click "Delete" on any student
   - Confirm deletion
   - Watch for an orange notification

4. **Test Other Entities**:
   - Repeat the same process for Courses, Instructors, and Departments
   - Each operation should trigger appropriate notifications

### Step 5: Verify the Azure Service Bus Queue
1. Open the [Azure Portal](https://portal.azure.com) and navigate to the provisioned Service Bus namespace
2. Open the `contosouniversity-notifications` queue (or your configured queue name)
3. Check the "Active message count" / "Overview" metrics to confirm messages are being enqueued and dequeued as notifications are sent/viewed

## Troubleshooting

### No Notifications Appearing
1. **Check Browser Console**: Press F12 and look for JavaScript errors
2. **Check Network Tab**: Verify calls to `/Notifications/GetNotifications` are happening
3. **Check the Service Bus queue**: Verify the queue exists and has messages in the Azure Portal

### Azure Service Bus Errors
1. **Authentication/Authorization failures (401/403)**:
   - Verify the app's Managed Identity (or your local developer credential) has been granted the **Azure Service Bus Data Sender** / **Azure Service Bus Data Receiver** roles on the namespace/queue
   - Locally, ensure you are signed in via `az login` or Visual Studio so `DefaultAzureCredential` can resolve a developer credential

2. **Queue/namespace not found**:
   - Ensure `AzureServiceBus:FullyQualifiedNamespace` in `Web.config` matches the provisioned namespace
   - Ensure the queue name matches `AzureServiceBus:NotificationQueueName` (default `contosouniversity-notifications`)

3. **Credential unavailable**:
   - Ensure you're running on Azure (Managed Identity) or have Azure CLI / Visual Studio signed in for local development

### JavaScript Not Loading
1. **Admin Role Check**: Ensure you're logged in as administrator
2. **File Paths**: Verify `notifications.js` and `notifications.css` files exist
3. **Browser Cache**: Clear cache and refresh

## Configuration Notes

- **Namespace/Queue**: Configured in `Web.config` as `AzureServiceBus:FullyQualifiedNamespace` / `AzureServiceBus:NotificationQueueName`
- **Polling Interval**: JavaScript checks for new notifications every 5 seconds
- **Auto-dismiss**: Notifications automatically disappear after 1 minute (60 seconds)
- **Max Notifications**: Maximum of 5 notifications shown simultaneously

## Production Considerations

For production deployment:

1. **Provisioning**: Ensure the Azure Service Bus namespace/queue is provisioned via the platform's IaC (Bicep/Terraform) before deployment
2. **Permissions**: Grant the App Service/hosting environment's Managed Identity the minimum required roles (Azure Service Bus Data Sender/Receiver) - avoid broader roles such as Owner/Contributor on the namespace
3. **Monitoring**: Monitor queue length and message processing via Azure Monitor / Service Bus metrics
4. **Resiliency**: Service Bus provides built-in message persistence and at-least-once delivery; no additional backup strategy is required
5. **Scaling**: A single Service Bus namespace/queue can be shared safely across multiple app instances - no centralized MSMQ server needed

## Development Tips

- Notifications are designed to be non-blocking - Azure Service Bus failures won't break main operations
- Debug output shows notification send/receive operations
- Use notification dashboard to understand system behavior
- Test with multiple admin users to verify isolation
