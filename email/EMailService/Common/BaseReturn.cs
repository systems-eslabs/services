using System;

namespace mailLibrary
{
    /// <summary>
    /// A generic class for all the service operation
    /// </summary>
    /// <typeparam name="T">Specifies the type of elements in the BaseReturn object</typeparam>
    public sealed class BaseReturn<T>
    {
        /// <summary>
        /// Code for the success or failure
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Message represent the code.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// To tell UI if the call to the service is successful or not
        /// </summary>
        public bool Success { get; set; }

        public Exception Exception { get; set; }

        /// <summary>
        /// Store the data returned by the service
        /// </summary>
        public T Data { get; set; }


    }

}