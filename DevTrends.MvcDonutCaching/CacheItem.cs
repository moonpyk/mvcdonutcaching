﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DevTrends.MvcDonutCaching
{
    [Serializable, DataContract]
    public class CacheItem
    {
        /// <summary>
        /// Gets or sets content type.
        /// </summary>
        /// <value>
        /// The content type.
        /// </value>
        [DataMember(Order = 1)]
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the content to be cached.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        [DataMember(Order = 2)]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the headers to be cached.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        [DataMember(Order = 3)]
        public KeyValuePair<string, string>[] CachedHeaders { get; set; }
    }
}
