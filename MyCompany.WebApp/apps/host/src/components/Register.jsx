import React, { useState } from 'react';
import api from '../services/api';
import { Mail, Lock, User, UserPlus, Loader2, ArrowLeft } from 'lucide-react';
import dotsCircle from '../assets/dots-circle.png';
import dotsSquare from '../assets/dots-square.png';
import './Auth.css';

const Register = ({ onToggle }) => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [username, setUsername] = useState('');
    const [role, setRole] = useState('User');
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
                password,
                role
            });
            alert('Registration Successful! Please login.');
            onToggle();
        } catch (err) {
            setError(err.response?.data?.error || 'Registration failed. Please check the health status above.');
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

                        <div className="input-group-v2">
                            <label>Role</label>
                            <select
                                value={role}
                                onChange={(e) => setRole(e.target.value)}
                                className="role-select-v2"
                                style={{
                                    width: '100%',
                                    padding: '12px',
                                    borderRadius: '8px',
                                    border: '1px solid #e2e8f0',
                                    backgroundColor: 'white',
                                    marginTop: '8px',
                                    fontSize: '14px'
                                }}
                            >
                                <option value="User">User</option>
                                <option value="Admin">Admin</option>
                            </select>
                        </div>

                        <button type="submit" disabled={loading} className="login-btn-v2" style={{ marginTop: '10px' }}>
                            {loading ? <Loader2 className="animate-spin" /> : 'Sign Up'}
                        </button>

                        <div className="login-footer-v2" style={{ marginTop: '24px' }}>
                            Already have an account? <span onClick={onToggle} className="signup-link-v2">Sign In</span>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    );
};

export default Register;
