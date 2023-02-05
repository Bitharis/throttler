using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskScheduler.Core
{
    /// <summary>
    /// Represents a result that can be either a value of type T or an error.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    public sealed class Result<T>
    {
        /// <summary>
        /// Initializes a new instance of the Result class with a specified value.
        /// </summary>
        /// <param name="value">The result value.</param>
        /// <exception cref="ArgumentNullException">Thrown if the `value` parameter is `null`.</exception>
        private Result(T value)
        {
            Value = value ?? throw new ArgumentNullException($"{nameof(value)} cannot be null.");
        }

        /// <summary>
        /// Initializes a new instance of the Result class with a specified error.
        /// </summary>
        /// <param name="ex">The error that caused the result to be an error.</param>
        private Result(Exception ex)
        {
            Exception = ex;
        }


        /// <summary>
        /// Gets the result value.
        /// </summary>
        public T? Value { get; }

        /// <summary>
        /// Gets a value indicating whether the result is an error.
        /// </summary>
        public bool IsError => Value == null;

        /// <summary>
        /// Gets the error message, if any, associated with the error result.
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// Gets the exception, if any, associated with the error result.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Creates a new Result instance with a successful value.
        /// </summary>
        /// <param name="r">The successful value of the Result.</param>
        /// <returns>A new Result instance with a successful value.</returns>
        public static Result<T> CreateSuccessfulResult(T r)
        {
            return new Result<T>(r);
        }

        /// <summary>
        /// Creates a new Result instance with an error caused by the specified exception.
        /// </summary>
        /// <param name="ex">The exception that caused the error.</param>
        /// <returns>A new Result instance with an error caused by the specified exception.</returns>
        public static Result<T> FromException(Exception ex)
        {
            return new Result<T>(ex);
        }

        /// <summary>
        /// Creates a new Result instance with an error caused by the specified exception and error message.
        /// </summary>
        /// <param name="ex">The exception that caused the error.</param>
        /// <param name="errorMessage">The error message associated with the error.</param>
        /// <returns>A new Result instance with an error caused by the specified exception and error message.</returns>
        public static Result<T> FromException(Exception ex, string errorMessage)
        {
            return new Result<T>(ex) { ErrorMessage = errorMessage};
        }
    }

}
