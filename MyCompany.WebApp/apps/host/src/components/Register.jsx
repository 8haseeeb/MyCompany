import React, { useState } from 'react';
import api from '../services/api';
import { Loader2 } from 'lucide-react';
import dotsCircle from '../assets/dots-circle.png';
import dotsSquare from '../assets/dots-square.png';
import './Auth.css';

/**
 * Public signup: no JWT stored; backend always creates role User. Redirects to login on success.
 */
const Register = ({ onSwitchToLogin }) => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [username, setUsername] = useState('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError('');
        setSuccess('');

        try {
            await api.post('/api/v1/auth/register', {
                userName: username,
                email,
                password
            });
            setSuccess('Account created. Please sign in with your email and password.');
            setTimeout(() => {
                onSwitchToLogin?.();
            }, 1500);
        } catch (err) {
            const body = err.response?.data;
            setError(
                body?.message ||
                    body?.error ||
                    'Registration failed. Please check the health status above.'
            );
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="login-page-wrapper">
            <div className="login-container">
                <img src={dotsSquare} alt="" className="xtel-dots-square" />
                <img src={dotsCircle} alt="" className="xtel-dots-circle" />
                <div className="registration-header">
                    <div className="brand-logo-v2">
                        <span className="logo-promo">Promo</span>
                        <span className="logo-express">EXPRESS</span>
                    </div>
                    <h2>Create Your Account</h2>
                    <p>Join the future of promotion management</p>
                </div>

                <div className="login-card-v2">
                    {error && <div className="error-badge-v2">{error}</div>}
                    {success && (
                        <div
                            className="error-badge-v2"
                            style={{ background: '#dcfce7', color: '#166534', borderColor: '#22c55e' }}
                        >
                            {success}
                        </div>
                    )}

                    <form onSubmit={handleSubmit} className="login-form-v2" noValidate>
                        <div className="input-group-v2">
                            <label>Username *</label>
                            <input
                                type="text"
                                placeholder="Username"
                                value={username}
                                onChange={(e) => setUsername(e.target.value)}
                                required
                            />
                        </div>

                        <div className="input-group-v2">
                            <label>Email Address *</label>
                            <input
                                type="email"
                                placeholder="Email"
                                value={email}
                                onChange={(e) => setEmail(e.target.value)}
                                required
                            />
                        </div>

                        <div className="input-group-v2">
                            <label>Password *</label>
                            <input
                                type="password"
                                placeholder="••••••••"
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                                required
                            />
                        </div>

                        <p style={{ fontSize: '13px', color: '#64748b', marginTop: '4px' }}>
                            New accounts are created as <strong>User</strong> (view-only). An administrator can change your role if needed.
                        </p>

                        <button type="submit" disabled={loading} className="login-btn-v2" style={{ marginTop: '10px' }}>
                            {loading ? <Loader2 className="animate-spin" /> : 'Sign Up'}
                        </button>

                        <div className="login-footer-v2" style={{ marginTop: '24px' }}>
                            Already have an account?{' '}
                            <span onClick={() => onSwitchToLogin?.()} className="signup-link-v2">
                                Sign In
                            </span>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    );
};

export default Register;
