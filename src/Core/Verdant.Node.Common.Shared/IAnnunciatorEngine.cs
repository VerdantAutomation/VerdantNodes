
namespace Verdant.Node.Common.Shared
{
    interface IAnnunciatorEngine
    {
        /// <summary>
        /// The duration of each step in the annunciator patterns.
        /// A '-' is two steps in duration, whereas a '.' or ',' are one step in duration.
        /// </summary>
        /// <param name="ms">Step duration in milliseconds</param>
        /// <returns></returns>
        int PatternStepDuration(int ms);

        /// <summary>
        /// This class is meant to be used internally by IAnnunciatorArray.
        /// Use dashes, dots and commas to establish a pattern, or null/emptystring to
        /// disable a pattern.
        /// </summary>
        /// <param name="array">The array to be controlled</param>
        /// <param name="iAnnunciator">The lamp to be controlled</param>
        /// <param name="pattern">The on/off pattern</param>
        void SetAnnunciatorPattern(IAnnunciatorArray array, int iAnnunciator, string pattern);
    }
}
