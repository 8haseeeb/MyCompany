import React, { useState } from 'react';
import api from '../services/api';
import { Mail, Lock, LogIn, Loader2, Eye, EyeOff, ArrowRight } from 'lucide-react';
import './Auth.css';

const Login = ({ setToken, setUserName, onToggle }) => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [showPassword, setShowPassword] = useState(false);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    const handleSubmit = async (e) => {
        e.preventDefault();
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
                setToken(token);
                if (setUserName) setUserName(userName);
            } else {
                setError("Login worked, but the server didn't send a token correctly.");
            }
        } catch (err) {
            const msg = err.response?.data?.message ||
                err.response?.data?.error ||
                "Connection Error. Please check if Gateway is running.";
            setError(msg);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="login-page-wrapper">
            <div className="xtel-dots-circle"></div>

            <div className="login-card-v2">
                <div className="brand-logo-xtel">
                    <span className="xtel-x">X</span>TEL
                </div>

                {error && <div className="error-badge-v2">{error}</div>}

                <form onSubmit={handleSubmit} className="login-form-v2">
                    <div className="input-group-v2">
                        <label>Email</label>
                        <input
                            type="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            required
                        />
                    </div>

                    <div className="input-group-v2">
                        <label>Password</label>
                        <div style={{ position: 'relative' }}>
                            <input
                                type={showPassword ? "text" : "password"}
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                                required
                            />
                            <div
                                className="password-toggle-v2"
                                onClick={() => setShowPassword(!showPassword)}
                            >
                                {showPassword ? <EyeOff size={20} /> : <Eye size={20} />}
                            </div>
                        </div>
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
                                Login <ArrowRight size={20} />
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
    );
};

export default Login;
