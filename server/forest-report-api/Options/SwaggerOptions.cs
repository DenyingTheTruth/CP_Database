namespace forest_report_api.Options
{
    public class SwaggerOptions
    {
        public string JsonRoute { get; set; }
        public string Description { get; set; }
        public string UiEndpoint { get; set; }
        public string[] Servers { get; set; }

        public bool IsDefault()
        {
            return JsonRoute == default && Description == default && UiEndpoint == default && Servers == default;
        }
    }
}