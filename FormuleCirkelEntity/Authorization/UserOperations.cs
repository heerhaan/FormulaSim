using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace FormuleCirkelEntity.Areas.Identity.Authorization
{
    public static class UserOperations
    {
        public static OperationAuthorizationRequirement ManageSim =
            new OperationAuthorizationRequirement { Name = Constants.ManageSimOperationName };
    }

    public class Constants
    {
        public static readonly string ManageSimOperationName = "ManageSim";

        public static readonly string OwnerId = "c03e3774-0ea8-4c1c-befa-0da4ac985283";
    }
}
