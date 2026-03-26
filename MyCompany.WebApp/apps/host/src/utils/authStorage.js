import api from '../services/api';
import { getRoleFromAccessToken, getRoleFromPayload, parseJwtPayload } from './jwtUtils';

/**
 * Persists tokens and default Authorization header. Returns { token, userName, role } or null.
 */
export function applyAuthResponse(responseData) {
    const token =
        responseData.accessToken ||
        responseData.AccessToken ||
        responseData.access_token;
    if (!token) return null;

    localStorage.setItem('token', token);
    localStorage.setItem('accessToken', token);

    const refreshToken =
        responseData.refreshToken ||
        responseData.RefreshToken ||
        responseData.refresh_token;
    if (refreshToken) {
        localStorage.setItem('refreshToken', refreshToken);
    }

    const userName = responseData.userName || responseData.UserName || 'User';
    localStorage.setItem('userName', userName);

    const roleFromApi = (responseData.role ?? responseData.Role ?? '').toString().trim();
    const roleFromJwt = getRoleFromAccessToken(token);
    const role = roleFromApi || roleFromJwt;
    localStorage.setItem('userRole', role);

    if (import.meta.env.DEV) {
        const payload = parseJwtPayload(token);
        console.debug('[auth] login role', {
            role,
            fromApiBody: Boolean(roleFromApi),
            fromJwt: roleFromJwt,
            jwtRoleFromClaims: payload ? getRoleFromPayload(payload) : null,
            rawPayloadKeys: payload ? Object.keys(payload) : [],
        });
    }

    api.defaults.headers.common['Authorization'] = `Bearer ${token}`;

    return { token, userName, role };
}
