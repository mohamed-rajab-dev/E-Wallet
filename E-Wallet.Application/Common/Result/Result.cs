using System;
using System.Collections.Generic;
using System.Text;

namespace E_Wallet.Application.Common.Result
{
    public class Result
    {
        public bool IsSuccess { get; private set; }
        public string Message { get; private set; } = null!;
        public Dictionary<string, List<string>>? Errors { get; private set; }
        public dynamic? Data { get; private set; }

        public static Result Success(string message, dynamic? data = null)
        {
            return new Result
            {
                IsSuccess = true,
                Message = message,
                Data = data
            };
        }

        public static Result Failure(string message, Dictionary<string, List<string>>? errors = null)
        {
            return new Result
            {
                IsSuccess = false,
                Message = message,
                Errors = errors
            };
        }
    }
}
