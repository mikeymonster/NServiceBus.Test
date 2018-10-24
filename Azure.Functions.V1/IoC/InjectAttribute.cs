using System;
using Microsoft.Azure.WebJobs.Description;

namespace Azure.Functions.V1.IoC
{
    [AttributeUsage(AttributeTargets.Parameter)]
    [Binding]
    public class InjectAttribute : Attribute
    {
    }
}
