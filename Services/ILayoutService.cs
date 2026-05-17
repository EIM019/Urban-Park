using System.Security.Claims;
using CarparkManagementSystem.ViewModels.Layouts;

namespace CarparkManagementSystem.Services;

public interface ILayoutService
{
    Task<LayoutsIndexViewModel> GetLayoutsIndexAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default);

    Task<SiteLayoutViewModel?> GetSiteLayoutAsync(
        int siteId,
        DateTime? startDateTime,
        DateTime? endDateTime,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default);

    Task<LayoutEditViewModel?> GetLayoutEditorAsync(int siteId, CancellationToken cancellationToken = default);

    Task<ServiceResult> UpdateLayoutAsync(LayoutEditViewModel model, CancellationToken cancellationToken = default);

    Task<SpaceLendingFormViewModel?> BuildLendingFormAsync(int siteId, ClaimsPrincipal user, CancellationToken cancellationToken = default);
Task<ServiceResult> CreateSpaceLendingAsync(SpaceLendingFormViewModel model, ClaimsPrincipal user, CancellationToken cancellationToken = default);
Task<ServiceResult> RevertLendingAsync(int lendingId, ClaimsPrincipal user, CancellationToken cancellationToken = default);
Task<ServiceResult> SetInactiveNoteAsync(InactiveNoteViewModel model, ClaimsPrincipal user, CancellationToken cancellationToken = default);
Task ProcessExpiredLendingsAsync(CancellationToken cancellationToken = default);
}
