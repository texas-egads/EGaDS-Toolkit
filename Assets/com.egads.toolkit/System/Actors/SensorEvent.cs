namespace egads.system.actors
{
    /// <summary>
    /// Enumeration representing different sensor events for actors.
    /// </summary>
    public enum SensorEvent
    {
        /// <summary>
        /// Event indicating that an actor has been detected by the sensor.
        /// </summary>
        ActorDetected,

        /// <summary>
        /// Event indicating that an actor has left the sensor's detection area.
        /// </summary>
        ActorLeft
    }
}
