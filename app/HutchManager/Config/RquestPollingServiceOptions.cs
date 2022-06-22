namespace HutchManager.OptionsModels
{
    public class RquestPollingServiceOptions
    {
        /// <summary>
        /// Polling interval in seconds
        /// for fetching queries from the RQUEST Task API
        /// </summary>
        public int QueryPollingInterval { get; set; } = 5;
    }
}
