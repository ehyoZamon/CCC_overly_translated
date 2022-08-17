using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
namespace Cynteract.OnlineDatabase
{
    public class StatusResponse:ServerResponse
    {
        public HttpStatusCode statusCode;

        public StatusResponse(HttpStatusCode statusCode)
        {
            this.statusCode = statusCode;
        }
    }
}