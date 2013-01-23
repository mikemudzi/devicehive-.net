﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceHive.Client
{
    /// <summary>
    /// Declares methods to connect a client with the DeviceHive service.
    /// Using this class, clients can get information about networks and devices, receive notification and send commands.
    /// </summary>
    public interface IClientService
    {
        /// <summary>
        /// Gets API info.
        /// </summary>
        /// <returns><see cref="ApiInfo"/> object.</returns>
        Task<ApiInfo> GetApiInfoAsync();

        /// <summary>
        /// Gets a list of networks.
        /// </summary>
        /// <returns>A list of <see cref="Network"/> objects.</returns>
        Task<List<Network>> GetNetworksAsync();

        /// <summary>
        /// Gets a list of devices of the specific network.
        /// </summary>
        /// <param name="networkId">Network identifier.</param>
        /// <returns>A list of <see cref="Device"/> objects that belongs to the specified network.</returns>
        Task<List<Device>> GetDevicesAsync(int networkId);
        
        /// <summary>
        /// Gets a list of devices of all networks.
        /// </summary>
        /// <returns>A list of <see cref="Device"/> objects.</returns>
        Task<List<Device>> GetDevicesAsync();

        /// <summary>
        /// Gets information about device.
        /// </summary>
        /// <param name="deviceId">Device unique identifier.</param>
        /// <returns><see cref="Device"/> object.</returns>
        Task<Device> GetDeviceAsync(Guid deviceId);

        /// <summary>
        /// Gets a list of equipment in a device class.
        /// </summary>
        /// <param name="deviceClassId">Device class identifier.</param>
        /// <returns>A list of <see cref="Equipment"/> objects for the specified device class.</returns>
        Task<List<Equipment>> GetEquipmentAsync(int deviceClassId);

        /// <summary>
        /// Gets a list of device equipment states.
        /// These objects provide information about the current state of device equipment.
        /// </summary>
        /// <param name="deviceId">Device unique identifier.</param>
        /// <returns>A list of <see cref="DeviceEquipmentState"/> objects.</returns>
        Task<List<DeviceEquipmentState>> GetEquipmentStateAsync(Guid deviceId);

        /// <summary>
        /// Gets a list of notifications generated by the device.
        /// </summary>
        /// <param name="deviceId">Device unique identifier.</param>
        /// <param name="start">Notifications start date (inclusive, optional).</param>
        /// <param name="end">Notifications end date (inclusive, optional).</param>
        /// <returns>A list of <see cref="Notification"/> objects.</returns>
        Task<List<Notification>> GetNotificationsAsync(Guid deviceId, DateTime? start = null, DateTime? end = null);

        /// <summary>
        /// Polls device notification from the service.
        /// </summary>
        /// <param name="callback">Callback action used to process notifications.</param>
        /// <param name="deviceId">Device unique identifier.</param>
        /// <param name="timestamp">Last received notification timestamp.</param>
        /// <param name="token">Cancellation token used to cancel polling operation.</param>
        Task PollNotifications(Action<List<Notification>> callback, Guid deviceId, DateTime? timestamp, CancellationToken token);

        /// <summary>
        /// Polls device notification from the service.
        /// </summary>
        /// <param name="callback">Callback action used to process notifications.</param>
        /// <param name="deviceIds">List of device unique identifiers.</param>
        /// <param name="timestamp">Last received notification timestamp.</param>
        /// <param name="token">Cancellation token used to cancel the polling operation.</param>
        Task PollNotifications(Action<List<DeviceNotification>> callback, Guid[] deviceIds, DateTime? timestamp, CancellationToken token);

        /// <summary>
        /// Gets information about a notification generated by the device.
        /// </summary>
        /// <param name="deviceId">Device unique identifier.</param>
        /// <param name="id">Notification identifier.</param>
        /// <returns>The <see cref="Notification"/> object.</returns>
        Task<Notification> GetNotificationAsync(Guid deviceId, int id);

        /// <summary>
        /// Gets a list of commands sent to the device.
        /// </summary>
        /// <param name="deviceId">Device unique identifier.</param>
        /// <param name="start">Commands start date (inclusive, optional).</param>
        /// <param name="end">Commands end date (inclusive, optional).</param>
        /// <returns>A list of <see cref="Command"/> objects.</returns>
        Task<List<Command>> GetCommandsAsync(Guid deviceId, DateTime? start = null, DateTime? end = null);
        
        /// <summary>
        /// Polls a list of commands sent to the service.
        /// </summary>
        /// <param name="callback">Callback action used to process commands.</param>
        /// <param name="deviceId">Device unique identifier.</param>
        /// <param name="timestamp">Last received command timestamp.</param>
        /// <param name="token">Cancellation token used to cancel polling operation.</param>
        Task PollCommands(Action<List<Command>> callback, Guid deviceId, DateTime? timestamp, CancellationToken token);
        
        /// <summary>
        /// Gets information about a command sent to the device.
        /// </summary>
        /// <param name="deviceId">Device unique identifier.</param>
        /// <param name="id">Command identifier.</param>
        /// <returns>The <see cref="Command"/> object.</returns>
        Task<Command> GetCommandAsync(Guid deviceId, int id);

        /// <summary>
        /// Sends new command to the device.
        /// </summary>
        /// <param name="deviceId">Device unique identifier.</param>
        /// <param name="command">A <see cref="Command"/> object to be sent.</param>
        /// <returns>The <see cref="Command"/> object with updated identifier and timestamp.</returns>
        Task<Command> SendCommandAsync(Guid deviceId, Command command);

        /// <summary>
        /// Waits for a command to be handled by the device.
        /// </summary>
        /// <param name="deviceId">Device unique identifier.</param>
        /// <param name="id">Command identifier.</param>
        /// <param name="token">Cancellation token used to cancel the polling operation.</param>
        /// <returns>The <see cref="Command"/> object with status and result fields.</returns>
        Task<Command> WaitCommandAsync(Guid deviceId, int id, CancellationToken token);
    }
}