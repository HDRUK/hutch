namespace HutchManager.OptionsModels
{
    /// <summary>
    /// Options for the RQUEST Task Api
    /// </summary>
    public class RquestTaskApiOptions
    {
        /// <summary>
        /// The Base Url of the Task API, not just RQUEST.
        /// </summary>
        public string BaseUrl { get; set; } = string.Empty;

        /// <summary>
        /// Queue Status Endpoint
        /// </summary>
        public string QueueStatusEndpoint { get; set; } = "queue";

        /// <summary>
        /// Fetch Query Endpoint
        /// </summary>
        public string FetchQueryEndpoint { get; set; } = "nextjob";

        /// <summary>
        /// Submit Result Endpoint
        /// </summary>
        public string SubmitResultEndpoint { get; set; } = "result";
        
        /// <summary>
        /// Username for the RQuest API. To be used in the Basic Auth header.
        /// </summary>
        public string Username { get; set; } = string.Empty;
        
        /// <summary>
        /// Password for the RQuest API. To be used in the Basic Auth header.
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}
