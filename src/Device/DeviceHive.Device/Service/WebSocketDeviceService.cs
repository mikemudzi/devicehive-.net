﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using WebSocket4Net;
using log4net;

namespace DeviceHive.Device
{
    /// <summary>
    /// Provides access for devices to WebSockets DeviceHive API (/device endpoint)
    /// </summary>
    public class WebSocketDeviceService : IDisposable
    {
        #region Private fields

        private readonly WebSocket _webSocket;

        private bool _isAuthenticated = false;
        private bool _isConnected = false;

        private readonly EventWaitHandle _cancelWaitHandle = new ManualResetEvent(false);
        private readonly EventWaitHandle _authWaitHandle = new ManualResetEvent(false);

        private readonly Dictionary<string, RequestInfo> _requests =
            new Dictionary<string, RequestInfo>();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="serviceUrl">URL of the DeviceHive web sockets service.</param>
        /// <param name="deviceGuid">Device GUID to authenticate.</param>
        /// <param name="deviceKey">Device key for authentication.</param>
        public WebSocketDeviceService(string serviceUrl, string deviceGuid = null, string deviceKey = null)
        {
            Timeout = 30000;

            _webSocket = new WebSocket(serviceUrl) { EnableAutoSendPing = false };
            _webSocket.MessageReceived += (s, e) => Task.Factory.StartNew(() => HandleMessage(e.Message));
            _webSocket.Opened += (s, e) => Task.Factory.StartNew(() => Authenticate(deviceGuid, deviceKey));
            _webSocket.Closed += (s, e) =>
            {
                _cancelWaitHandle.Set();
                _isConnected = _isAuthenticated = false;
                OnConnectionClosed();
            };
        }
        
        #endregion

        #region Events

        /// <summary>
        /// Fires when new command is inserted for active subscription
        /// </summary>
        public event EventHandler<CommandEventArgs> CommandInserted;

        /// <summary>
        /// Fires <see cref="CommandInserted"/> event.
        /// </summary>
        /// <param name="eventArgs">CommandEventArgs object.</param>
        protected void OnCommandInserted(CommandEventArgs eventArgs)
        {
            var handler = CommandInserted;
            if (handler != null)
                handler(this, eventArgs);
        }

        /// <summary>
        /// Fires when connection is closed
        /// </summary>
        public event EventHandler ConnectionClosed;

        /// <summary>
        /// Fires <see cref="ConnectionClosed"/> event.
        /// </summary>
        protected void OnConnectionClosed()
        {
            var handler = ConnectionClosed;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The number of miliseconds to wait before the request times out.
        /// The default value is 30,000 milliseconds (30 seconds)
        /// </summary>
        public int Timeout { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Open WebSocket connection and authenticate user.
        /// </summary>
        public void Open()
        {
            _isAuthenticated = false;
            _isConnected = false;

            if (_webSocket.State == WebSocketState.Closing)
            {
                WaitHandle.WaitAny(new WaitHandle[] {_cancelWaitHandle});
            }
            else if (_webSocket.State == WebSocketState.Open)
            {
                _cancelWaitHandle.Reset();
                _webSocket.Close();
                WaitHandle.WaitAny(new WaitHandle[] {_cancelWaitHandle});
            }

            _cancelWaitHandle.Reset();
            _authWaitHandle.Reset();

            _webSocket.Open();
            WaitHandle.WaitAny(new WaitHandle[] {_authWaitHandle, _cancelWaitHandle}, Timeout);

            if (!_isConnected)
                throw new DeviceServiceException("WebSocket connection error");
        }

        /// <summary>
        /// Close WebSocket connection.
        /// </summary>
        public void Close()
        {
            if (_webSocket.State == WebSocketState.Open)
                _webSocket.Close();
        }

        /// <summary>
        /// Gets device from the DeviceHive network.
        /// </summary>
        /// <param name="deviceGuid">Device unique identifier.</param>
        /// <param name="deviceKey">Device private key.</param>
        /// <returns><see cref="Device"/> object from DeviceHive.</returns>
        public Device GetDevice(string deviceGuid, string deviceKey)
        {
            if (!_isConnected)
                Open();

            var res = SendRequest("device/get", deviceGuid, deviceKey);
            var deviceJson = (JObject)res["device"];
            return Deserialize<Device>(deviceJson);
        }

        /// <summary>
        /// Registers a device.
        /// </summary>
        /// <param name="device">Device object.</param>
        public void RegisterDevice(Device device)
        {
            if (!_isConnected)
                Open();

            var deviceJson = Serialize(device);
            SendRequest("device/save", device.Id, device.Key, new JProperty("device", deviceJson));
        }

        /// <summary>
        /// Update a existing device.
        /// </summary>
        /// <param name="device">Device object.</param>
        public void UpdateDevice(Device device)
        {
            if (!_isConnected)
                Open();

            var deviceJson = Serialize(device, NullValueHandling.Ignore);
            SendRequest("device/save", device.Id, device.Key, new JProperty("device", deviceJson));
        }

        /// <summary>
        /// Sends new device notification to the service.
        /// </summary>
        /// <param name="notification">A <see cref="Notification"/> object</param>
        /// <param name="deviceGuid">Optional device unique identifier.</param>
        /// <param name="deviceKey">Optional device key.</param>
        /// <returns>The <see cref="Notification"/> object with updated identifier and timestamp.</returns>
        public Notification SendNotification(Notification notification, string deviceGuid = null, string deviceKey = null)
        {
            if (!_isConnected)
                Open();

            var res = SendRequest("notification/insert", deviceGuid, deviceKey,
                new JProperty("notification", Serialize(notification)));
            var notificationJson = (JObject)res["notification"];
            return Deserialize<Notification>(notificationJson);
        }

        /// <summary>
        /// Updates a device command status and result.
        /// </summary>
        /// <param name="command">A <see cref="Command"/> object to be updated.</param>
        /// <param name="deviceGuid">Optional device unique identifier.</param>
        /// <param name="deviceKey">Optional device key.</param>
        public void UpdateCommand(Command command, string deviceGuid = null, string deviceKey = null)
        {
            if (!_isConnected)
                Open();

            SendRequest("command/update", deviceGuid, deviceKey,
                new JProperty("commandId", command.Id),
                new JProperty("command", Serialize(command, NullValueHandling.Ignore)));
        }

        /// <summary>
        /// Subscribe to device commands.
        /// </summary>
        /// <param name="deviceGuid">Optional device unique identifier.</param>
        /// <param name="deviceKey">Optional device key.</param>
        public void SubscribeToCommands(string deviceGuid = null, string deviceKey = null)
        {
            if (!_isConnected)
                Open();

            SendRequest("command/subscribe", deviceGuid, deviceKey);
        }

        /// <summary>
        /// Unsubscribe from device commands.
        /// </summary>
        /// <param name="deviceGuid">Optional device unique identifier.</param>
        /// <param name="deviceKey">Optional device key.</param>
        public void UnsubscribeFromCommands(string deviceGuid = null, string deviceKey = null)
        {
            if (!_isConnected)
                Open();

            SendRequest("command/unsubscribe", deviceGuid, deviceKey);
        }

        #endregion

        #region Private methods

        private void Authenticate(string deviceGuid, string deviceKey)
        {
            if (deviceGuid == null)
            {
                _isConnected = true;
                _isAuthenticated = false;
                _authWaitHandle.Set();
                return;
            }

            try
            {
                SendRequest("authenticate", deviceGuid, deviceKey);
                _isAuthenticated = true;
                _isConnected = true;
            }
            catch (Exception e)
            {
                _isAuthenticated = false;
                _isConnected = false;
                LogManager.GetLogger(GetType()).Error("Web socket authentication error", e);
            }

            _authWaitHandle.Set();
        }

        private JObject SendRequest(string action, string deviceGuid, string deviceKey, params JProperty[] args)
        {
            var requestId = Guid.NewGuid().ToString();
            var requestInfo = new RequestInfo();
            _requests.Add(requestId, requestInfo);

            var commonProperties = new List<JProperty>()
            {
                new JProperty("action", action),
                new JProperty("requestId", requestId)
            };

            if (deviceGuid != null)
                commonProperties.Add(new JProperty("deviceId", deviceGuid));

            if (deviceKey != null)
                commonProperties.Add(new JProperty("deviceKey", deviceKey));

            var requestJson = new JObject(commonProperties.Concat(args).Cast<object>().ToArray());
            _webSocket.Send(requestJson.ToString());

            WaitHandle.WaitAny(new[] {requestInfo.WaitHandle, _cancelWaitHandle}, Timeout);

            if (requestInfo.Result == null)
                throw new DeviceServiceException("WebSocket connection was unexpectly closed");

            var status = (string)requestInfo.Result["status"];
            if (status == "error")
                throw new DeviceServiceException((string) requestInfo.Result["error"]);

            return requestInfo.Result;
        }

        private void HandleMessage(string message)
        {
            var json = JObject.Parse(message);

            // handle server notifications
            var action = (string)json["action"];
            switch (action)
            {
                case "command/insert":
                    HandleCommandInsert(json);
                    return;
            }

            // handle responses to client requests
            var requestId = (string) json["requestId"];

            RequestInfo requestInfo;
            if (_requests.TryGetValue(requestId, out requestInfo))
                requestInfo.Result = json;
        }

        private void HandleCommandInsert(JObject json)
        {
            var deviceGuid = (string)json["deviceGuid"];
            var commandJson = (JObject)json["command"];
            var command = Deserialize<Command>(commandJson);
            OnCommandInserted(new CommandEventArgs(deviceGuid, command));
        }

        private static JObject Serialize<T>(T obj, NullValueHandling nullValueHandling = NullValueHandling.Include) where T : class
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            var serializer = new JsonSerializer();
            serializer.NullValueHandling = nullValueHandling;
            serializer.ContractResolver = new JsonContractResolver();
            return JObject.FromObject(obj, serializer);
        }

        private static T Deserialize<T>(JObject json)
        {
            var serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.ContractResolver = new JsonContractResolver();
            return json.ToObject<T>(serializer);
        }

        #endregion

        #region JsonContractResolver class

        private class JsonContractResolver : CamelCasePropertyNamesContractResolver
        {
            #region DefaultContractResolver Members

            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var property = base.CreateProperty(member, memberSerialization);
                if (property.DeclaringType == typeof(Notification) && property.PropertyName == "name")
                {
                    property.PropertyName = "notification";
                }
                if (property.DeclaringType == typeof(Command) && property.PropertyName == "name")
                {
                    property.PropertyName = "command";
                }
                return property;
            }
            #endregion
        }

        #endregion

        #region RequestInfo class

        private class RequestInfo
        {
            private readonly EventWaitHandle _eventWaitHandle = new ManualResetEvent(false);

            private JObject _result;

            public JObject Result
            {
                get { return _result; }
                set
                {
                    _result = value;
                    _eventWaitHandle.Set();
                }
            }

            public WaitHandle WaitHandle
            {
                get { return _eventWaitHandle; }
            }
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Close();
        }

        #endregion
    }
}