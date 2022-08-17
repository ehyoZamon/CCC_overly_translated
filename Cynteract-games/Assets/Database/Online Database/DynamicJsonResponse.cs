using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicJsonResponse : ServerResponse
{
    public dynamic content;

    public DynamicJsonResponse(JObject content)
    {
        this.content = content;
    }
}
