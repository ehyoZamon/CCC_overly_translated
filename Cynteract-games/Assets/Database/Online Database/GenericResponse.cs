using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericResponse <T>:ServerResponse
{
    public T content;

    public GenericResponse(T content)
    {
        this.content = content;
    }
}
