using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.DependencyInjection;

namespace FilipunkBlog.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}
