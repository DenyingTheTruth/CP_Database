using Microsoft.AspNetCore.Mvc;

namespace forest_report_api.Models
{
    public class ResponseResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public object Value { get; set; }

        public ResponseResult(bool status, string message, object value = null)
        {
            IsSuccess = status;
            Message = message;
            Value = value;
        }
    }
}