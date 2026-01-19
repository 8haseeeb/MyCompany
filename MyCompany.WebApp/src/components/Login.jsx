import React, { useState } from 'react';
import api from '../services/api';
import { Mail, Lock, LogIn, Loader2 } from 'lucide-react';
import './Auth.css';

const Login = ({ setToken, setUserName, onToggle }) => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError('');
        console.log("Submit Clicked. Data:", { Email: email });

        try {
            // Clear old data just in case
            localStorage.removeItem('token');

            const response = await api.post('/api/auth/login', {
                Email: email,
                Password: password
            });

            console.log("API Success. Data:", response.data);

            // Robust token extraction
            const token = response.data.accessToken ||
                response.data.AccessToken ||
                response.data.access_token;

            if (token) {
                console.log("Token acquired. Saving...");
                localStorage.setItem('token', token);

                const userName = response.data.userName || response.data.UserName || "User";
                localStorage.setItem('userName', userName);

                setToken(token);
                if (setUserName) setUserName(userName);

                // No alert needed if it redirects correctly, but adding log
                console.log("Transitioning to Dashboard...");
            } else {
                console.warn("Token not found in response schema!");
                setError("Login worked, but the server didn't send a token correctly.");
            }
        } catch (err) {
            console.error("API Error Object:", err);
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
            <div className="login-card-v2">
                <div className="login-header-v2">
                    <div className="brand-logo-v2">
                        <span className="logo-promo">Promo</span>
                        <span className="logo-express">EXPRESS</span>
                    </div>
                    <h2>Welcome Back!</h2>
                    <p>Enter your details to manage your promotions</p>
                </div>

                <div className="social-login-section">
                    <button className="google-btn">
                        <img src="https://www.gstatic.com/firebasejs/ui/2.0.0/images/auth/google.svg" alt="G" />
                        Sign in with Google
                    </button>
                    <div className="social-icons-row">
                        <div className="social-circle fb"><span className="social-inner">f</span></div>
                        <div className="social-circle ig"><span className="social-inner">📸</span></div>
                    </div>
                </div>

                {error && <div className="error-badge">{error}</div>}

                <form onSubmit={handleSubmit} className="login-form-v2">
                    <div className="input-group-v2">
                        <label>Email *</label>
                        <input
                            type="email"
                            placeholder="Email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            className={error ? 'error-border' : ''}
                            required
                        />
                        {error && <span className="field-error">Valid email is required</span>}
                    </div>

                    <div className="input-group-v2">
                        <label>Password *</label>
                        <input
                            type="password"
                            placeholder="Password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                        />
                    </div>

                    <div className="forgot-password">
                        <span className="forgot-link">Forgot Password?</span>
                    </div>

                    <button type="submit" disabled={loading} className="login-btn-v2">
                        {loading ? 'Processing...' : 'Login'}
                    </button>

                    <div className="login-footer-v2">
                        <p>Don't have an account? <span onClick={onToggle} className="signup-link-btn">Create Account</span></p>
                    </div>
                </form>
            </div>

            <div className="truck-illustration">
                {/* Visual element representing the truck line art */}
            </div>
        </div>
    );
};

export default Login;
