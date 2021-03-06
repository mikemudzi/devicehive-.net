﻿using System;

namespace DeviceHive.Data.Model
{
    /// <summary>
    /// Represents meta-information about the current API.
    /// </summary>
    public class ApiInfo
    {
        #region Public Properties

        /// <summary>
        /// API version.
        /// </summary>
        public string ApiVersion { get; set; }

        /// <summary>
        /// Current server timestamp.
        /// </summary>
        public DateTime ServerTimestamp { get; set; }

        /// <summary>
        /// WebSocket server URL.
        /// </summary>
        /// <remarks>
        /// Should be <c>null</c> if WebSockets are not supported.
        /// </remarks>
        public string WebSocketServerUrl { get; set; }

        /// <summary>
        /// REST server URL.
        /// </summary>
        public string RestServerUrl { get; set; }

        #endregion
    }
}