namespace CarparkManagementSystem.Constants;

public static class ApplicationRoles
{
    public const string FacilitiesManager = "FacilitiesManager";
    public const string ReceptionistAdmin = "ReceptionistAdmin";
    public const string ITTechnician = "ITTechnician";

    public static readonly string[] All =
    [
        FacilitiesManager,
        ReceptionistAdmin,
        ITTechnician
    ];
}
