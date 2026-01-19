import React, { useState } from 'react';
import api from '../services/api';
import { Mail, Lock, User, UserPlus, Loader2, ArrowLeft } from 'lucide-react';
import './Auth.css';

const Register = ({ onToggle }) => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [username, setUsername] = useState('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError('');

        try {
            await api.post('/api/auth/register', {
                userName: username,
                email,
                password
            });
            alert('Registration Successful! Please login.');
            onToggle();
        } catch (err) {
            setError(err.response?.data?.error || 'Registration failed.');
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
                    <h2>Create Your Account</h2>
                    <p>Join the future of promotion management</p>
                </div>

                {error && <div className="error-badge">{error}</div>}

                <form onSubmit={handleSubmit} className="login-form-v2">
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

                    <button type="submit" disabled={loading} className="login-btn-v2">
                        {loading ? 'Registering...' : 'Sign Up'}
                    </button>

                    <div className="login-footer-v2">
                        <p>Already have an account? <span onClick={onToggle} className="signup-link-btn">Sign In</span></p>
                    </div>
                </form>
            </div>

            <div className="truck-illustration"></div>
        </div>
    );
};

export default Register;
