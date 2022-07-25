namespace HutchManager.OptionsModels
{
    public class ActivitySourcePollingOptions
    {
        /// <summary>
        /// Polling interval in seconds
        /// for fetching jobs from Activity Sources
        /// </summary>
        public int PollingInterval { get; set; } = 5;
    }
}
