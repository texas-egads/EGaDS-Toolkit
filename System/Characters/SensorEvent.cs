namespace egads.system.characters
{
    /// <summary>
    /// Enumeration representing different sensor events for actors.
    /// </summary>
    public enum SensorEvent
    {
        /// <summary>
        /// Event indicating that an actor has been detected by the sensor.
        /// </summary>
        CharacterDetected,

        /// <summary>
        /// Event indicating that an actor has left the sensor's detection area.
        /// </summary>
        CharacterLeft
    }
}
