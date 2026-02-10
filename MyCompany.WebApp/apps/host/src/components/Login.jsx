import React, { useState } from 'react';
import api from '../services/api';
import { Mail, Lock, LogIn, Loader2, Eye, EyeOff, ArrowRight } from 'lucide-react';
import xtelLogo from '../assets/xtel-logo.png';
import dotsCircle from '../assets/dots-circle.png';
import dotsSquare from '../assets/dots-square.png';
import './Auth.css';

const Login = ({ setToken, setUserName, setUserRole, onToggle }) => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [showPassword, setShowPassword] = useState(false);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const [fieldErrors, setFieldErrors] = useState({});

    const validateForm = () => {
        const errors = {};

        // Email validation
        if (!email) {
            errors.email = "Email is required";
        } else if (!/\S+@\S+\.\S+/.test(email)) {
            errors.email = "Please enter a valid email address (e.g. name@example.com)";
        }

        // Password validation
        if (!password) {
            errors.password = "Password is required";
        } else if (password.length < 6) {
            errors.password = "Password must be at least 6 characters";
        }

        setFieldErrors(errors);
        return Object.keys(errors).length === 0;
    };

    const parseJwt = (token) => {
        try {
            const base64Url = token.split('.')[1];
            const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
            const jsonPayload = decodeURIComponent(window.atob(base64).split('').map(function (c) {
                return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
            }).join(''));

            return JSON.parse(jsonPayload);
        } catch (e) {
            return null;
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (!validateForm()) return;

        setLoading(true);
        setError('');

        try {
            const response = await api.post('/api/auth/login', {
                Email: email,
                Password: password
            });

            const token = response.data.accessToken ||
                response.data.AccessToken ||
                response.data.access_token;

            if (token) {
                localStorage.setItem('token', token);
                const userName = response.data.userName || response.data.UserName || "User";
                localStorage.setItem('userName', userName);

                // Decode Role from Token
                const decoded = parseJwt(token);
                // Look for standard .NET claim name for roles
                const role = decoded?.["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ||
                    decoded?.role ||
                    "User";

                localStorage.setItem('userRole', role);

                setToken(token);
                if (setUserName) setUserName(userName);
                if (setUserRole) setUserRole(role);
            } else {
                setError("Login worked, but the server didn't send a token correctly.");
            }
        } catch (err) {
            const msg = err.response?.data?.message ||
                err.response?.data?.error ||
                "Service Unavailable. Please check the health status above.";
            setError(msg);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="login-page-wrapper">

            <div className="login-container">
                <img src={dotsSquare} alt="" className="xtel-dots-square" />
                <img src={dotsCircle} alt="" className="xtel-dots-circle" />
                <div className="brand-logo-xtel">
                    <img src={xtelLogo} alt="XTEL Logo" className="xtel-logo-img" />
                    <span className="logo-express">PROMO</span>
                </div>


                <div className="login-card-v2">
                    {error && <div className="error-badge-v2">{error}</div>}

                    <form onSubmit={handleSubmit} className="login-form-v2" noValidate>
                        <div className={`input-group-v2 ${fieldErrors.email ? 'error' : ''}`}>
                            <label>Email</label>
                            <input
                                type="email"
                                value={email}
                                onChange={(e) => {
                                    setEmail(e.target.value);
                                    if (fieldErrors.email) setFieldErrors(prev => ({ ...prev, email: null }));
                                }}
                            />
                            {fieldErrors.email && <span className="error-text-v2">{fieldErrors.email}</span>}
                        </div>

                        <div className={`input-group-v2 ${fieldErrors.password ? 'error' : ''}`}>
                            <label>Password</label>
                            <div style={{ position: 'relative' }}>
                                <input
                                    type={showPassword ? "text" : "password"}
                                    value={password}
                                    onChange={(e) => {
                                        setPassword(e.target.value);
                                        if (fieldErrors.password) setFieldErrors(prev => ({ ...prev, password: null }));
                                    }}
                                />
                                <div
                                    className="password-toggle-v2"
                                    onClick={() => setShowPassword(!showPassword)}
                                    style={{ bottom: fieldErrors.password ? '22px' : '12px' }}
                                >
                                    {showPassword ? <EyeOff size={18} /> : <Eye size={18} />}
                                </div>
                            </div>
                            {fieldErrors.password && <span className="error-text-v2">{fieldErrors.password}</span>}
                        </div>

                        <div className="form-extras-v2">
                            <label className="remember-me-v2">
                                <input type="checkbox" />
                                Remember me
                            </label>
                            <span className="forgot-link-v2">Forgot password?</span>
                        </div>

                        <button type="submit" disabled={loading} className="login-btn-v2">
                            {loading ? <Loader2 className="animate-spin" /> : (
                                <>
                                    Login <ArrowRight size={18} />
                                </>
                            )}
                        </button>

                        <div className="divider-v2">or</div>

                        <div className="login-footer-v2">
                            Don't have an account? <span onClick={onToggle} className="signup-link-v2">Sign up</span>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    );
};

export default Login;
