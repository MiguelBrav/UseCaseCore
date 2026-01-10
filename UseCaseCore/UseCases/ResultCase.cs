using System;
using System.Collections.Generic;
using System.Text;

namespace UseCaseCore.UseCases
{
    public class ResultCase<T>
    {
        public bool Success { get; }
        public T Value { get; }          
        public string Error { get; }     
        public int StatusCode { get; }
        public string ResponseMessage { get; }  
        public string CreatedLocation { get; }

        private ResultCase(bool success, T value, string error, int statusCode = 200, string responseMessage = "", string createdLocation = "")
        {
            Success = success;
            Value = value;
            Error = error;
            StatusCode = statusCode;
            ResponseMessage = responseMessage;
            CreatedLocation = createdLocation;
        }

        public static ResultCase<T> Ok(T value, string message = "")
            => new ResultCase<T>(true, value, "", 200, message);

        public static ResultCase<T> Created(T value, string location, string message = "")
            => new ResultCase<T>(true, value, "", 201, message, location);

        public static ResultCase<T> NoContent(string message = "")
            => new ResultCase<T>(true, default(T), "", 204, message);

        public static ResultCase<T> Fail(string error, string message = "")
            => new ResultCase<T>(false, default(T), error, 400, message);

        public static ResultCase<T> NotFound(string message = "")
            => new ResultCase<T>(false, default(T), "Not Found", 404, message);

        public static ResultCase<T> ServerError(string message = "")
            => new ResultCase<T>(false, default(T), "Server Error", 500, message);

        public static ResultCase<T> Custom(int statusCode, T value = default(T), string error = "", string message = "", string location = "")
            => new ResultCase<T>(statusCode >= 200 && statusCode < 300, value, error, statusCode, message, location);
    }
}
