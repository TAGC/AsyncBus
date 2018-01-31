namespace AsyncBus
{
    /// <summary>
    /// Used to configure and return a new asynchronous message bus.
    /// </summary>
    /// <remarks>
    /// At present, no configuration is possible for the bus. This class exists to support future versions
    /// of this project that may allow for configuration, but more importantly to ensure clients access the
    /// bus through an interface and not a direct class.
    /// </remarks>
    public static class BusSetup
    {
        /// <summary>
        /// Creates and returns a new asynchronous message bus.
        /// </summary>
        /// <returns>A new bus.</returns>
        public static IBus CreateBus() => new Bus();
    }
}