﻿using System;
using System.Collections.Generic;
using Serilog.Events;

namespace SerilogExt.Extensions.Hosting
{
    /// <summary>
    /// A container that receives properties added to a diagnostic context.
    /// </summary>
    public sealed class DiagnosticContextCollector : IDisposable
    {
        readonly IDisposable _chainedDisposable;
        readonly object _propertiesLock = new object();
        Dictionary<string, LogEventProperty> _properties = new Dictionary<string, LogEventProperty>();

        /// <summary>
        /// Construct a <see cref="DiagnosticContextCollector"/>.
        /// </summary>
        /// <param name="chainedDisposable">An object that will be disposed to signal completion/disposal of
        /// the collector.</param>
        public DiagnosticContextCollector(IDisposable chainedDisposable)
        {
            _chainedDisposable = chainedDisposable ?? throw new ArgumentNullException(nameof(chainedDisposable));
        }

        /// <summary>
        /// Add the property to the context.
        /// </summary>
        /// <param name="property">The property to add.</param>
        public void AddOrUpdate(LogEventProperty property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));

            lock (_propertiesLock)
            {
                if (_properties == null) return;
                _properties[property.Name] = property;
            }
        }

        /// <summary>
        /// Complete the context and retrieve the properties added to it, if any. This will
        /// stop collection and remove the collector from the original execution context and
        /// any of its children.
        /// </summary>
        /// <param name="properties">The collected properties, or null if no collection is active.</param>
        /// <returns>True if properties could be collected.</returns>
        public bool TryComplete(out IEnumerable<LogEventProperty> properties)
        {
            lock (_propertiesLock)
            {
                properties = _properties?.Values;
                _properties = null;
                Dispose();
                return properties != null;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _chainedDisposable.Dispose();
        }
    }
}
