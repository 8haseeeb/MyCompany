/**
 * Decode JWT payload (no signature verification — for UI claims only).
 */
export function parseJwtPayload(token) {
    if (!token || typeof token !== 'string') return null;
    try {
        const base64Url = token.split('.')[1];
        if (!base64Url) return null;
        let base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const pad = base64.length % 4;
        if (pad) base64 += '='.repeat(4 - pad);
        const jsonPayload = decodeURIComponent(
            window
                .atob(base64)
                .split('')
                .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
                .join('')
        );
        return JSON.parse(jsonPayload);
    } catch {
        return null;
    }
}

/** .NET ClaimTypes.Role as serialized in some JWT payloads */
const ROLE_CLAIM_MS = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';

function normalizeRoleClaim(raw) {
    if (raw == null) return '';
    if (Array.isArray(raw)) {
        const first = raw.find((x) => x != null && String(x).trim() !== '');
        return first != null ? String(first).trim() : '';
    }
    return String(raw).trim();
}

/**
 * Extract role string from decoded JWT payload (handles short "role" vs long MS claim type vs array).
 */
export function getRoleFromPayload(decoded) {
    if (!decoded || typeof decoded !== 'object') return '';
    const candidates = [
        decoded[ROLE_CLAIM_MS],
        decoded.role,
        decoded.Role,
        decoded.roles,
    ];
    for (const c of candidates) {
        const r = normalizeRoleClaim(c);
        if (r) return r;
    }
    return '';
}

/** Role claim as issued by SSO JwtTokenService (ClaimTypes.Role → often "role" in JSON). */
export function getRoleFromAccessToken(token) {
    const decoded = parseJwtPayload(token);
    const fromClaims = getRoleFromPayload(decoded);
    if (fromClaims) return fromClaims;
    return 'User';
}

/** Update localStorage + notify host UI after refresh (no import from api.js). */
export function persistRoleFromAccessToken(accessToken, roleOverride) {
    const fromOverride =
        roleOverride != null && String(roleOverride).trim() !== ''
            ? String(roleOverride).trim()
            : '';
    const role = fromOverride || getRoleFromAccessToken(accessToken);
    localStorage.setItem('userRole', role);
    window.dispatchEvent(new CustomEvent('auth-role-updated', { detail: { role } }));
    return role;
}
