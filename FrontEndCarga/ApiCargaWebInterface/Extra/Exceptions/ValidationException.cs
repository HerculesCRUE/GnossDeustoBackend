using ApiCargaWebInterface.ViewModels;
using System;

namespace ApiCargaWebInterface.Extra.Exceptions
{
    public class ValidationException : Exception
    {
        public ShapeReportModel Report { get; set; }
        public ValidationException()
        {
        }

        public ValidationException(ShapeReportModel report)
        {
            Report = report;
        }

        public ValidationException(string message)
            : base(message)
        {
        }

        public ValidationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
