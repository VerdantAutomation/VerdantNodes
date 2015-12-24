namespace Verdant.Node.Common
{
    interface IAnnunciatorArray : IEnumeratedDevice
    {
        /// <summary>
        /// The number of annunciator lights in this array
        /// </summary>
        int NumberOfAnnunciators { get; }

        /// <summary>
        /// Set the pattern of flashes for an annunciator lamp.
        /// '-', dots '.' and commas ',' establish a flashing pattern
        /// For instance, "-,.,," will turn the lamp on for two steps,
        /// then off for one step then on for one step and the off for two
        /// steps, which appears as a dash-dot pattern. In general, for
        /// implementors of IAnnunciatorArray, this method
        /// can defer to an IAnnunciatorEngine implementation.
        /// </summary>
        /// <param name="iAnnunciator">Which lamp to control</param>
        /// <param name="pattern">A pattern of '1' (for continuous on) or dashes and dots and pauses, or null or empty string to turn
        /// the lamp off and disable patterns</param>
        void SetAnnunciatorPattern(int iAnnunciator, string pattern);

        /// <summary>
        /// Turn an annunciator lamp on or off.  Note that if a pattern is active,
        /// this will not interrupt the pattern, it will just overwrite the current
        /// state.  On the next pattern step, the lamp state will be re-set.
        /// To have this state 'stick', be sure to set the annunciator pattern
        /// to 'null'.
        /// </summary>
        /// <param name="iAnnunciator">Which annunciator lamp</param>
        /// <param name="state">True for on, false for off</param>
        void SetAnnunciatorState(int iAnnunciator, bool state);
    }
}
