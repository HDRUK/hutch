namespace HutchManager.Config
{
    /// <summary>
    /// Options for the RQuest Task Api
    /// </summary>
    public class RQuestTaskApiOptions
    {
        /// <summary>
        /// <para>The Base Path of the Task API endpoints, which will be added to an RQuest Activity Source's Host URI</para>
        ///
        /// <para>
        /// e.g.</para>
        /// <para>Host URI configured as `https://my-rquest.com:12345`</para>
        /// <para>EndpointBase of `bcos-rest/task`</para>
        ///
        /// <para>Actual requests will go to `https://my-rquest.com:12345/bcos-rest/task/&lt;endpoint&gt;`</para>
        /// </summary>
        public string EndpointBase { get; set; } = "bcos-rest/api/task";

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
