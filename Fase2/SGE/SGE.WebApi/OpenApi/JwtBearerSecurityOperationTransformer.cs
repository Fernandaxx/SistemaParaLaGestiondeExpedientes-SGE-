using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace SGE.WebApi.OpenApi;

public sealed class JwtBearerSecurityOperationTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        var metadata = context.Description.ActionDescriptor.EndpointMetadata;
        var requiereAutorizacion = metadata.OfType<IAuthorizeData>().Any();
        var permiteAnonimo = metadata.OfType<IAllowAnonymous>().Any();

        if (!requiereAutorizacion || permiteAnonimo) return Task.CompletedTask;

        operation.Security ??= [];
        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", context.Document, externalResource: null)] = []
        });

        return Task.CompletedTask;
    }
}
