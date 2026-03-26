/**
 * Role comes from JWT (claim) or localStorage; normalize for UI checks.
 * Backend is source of truth; Admin-only mutations return 403 for User role.
 */
export function canEditContent(userRole) {
    const r = String(userRole ?? '').trim().toLowerCase();
    return r === 'admin';
}
