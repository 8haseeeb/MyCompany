import React, { useState, useEffect } from 'react';
import axios from 'axios';
import './ServiceStatusAlert.css';

const ServiceStatusAlert = () => {
    const [downServices, setDownServices] = useState([]);
    const [isVisible, setIsVisible] = useState(false);

    const checkHealth = async () => {
        // Only check health when user is logged in (has token)
        const token = localStorage.getItem('token');
        if (!token) {
            setDownServices([]);
            setIsVisible(false);
            return;
        }

        try {
            const response = await axios.get(`https://promo.azure-api.net/promotion/api/Health?t=${new Date().getTime()}`, {
                headers: { Authorization: `Bearer ${token}` }
            });
            const data = response.data;
            console.log("✅ Health Response:", data);

            setDownServices([]);
            setIsVisible(false);
        } catch (error) {
            console.error("❌ Health Connection Error:", error);
            setDownServices([{ service: 'Promotions API (Offline)', status: 'Down' }]);
            setIsVisible(true);
        }
    };

    useEffect(() => {
        checkHealth();
        const interval = setInterval(checkHealth, 5000);
        return () => clearInterval(interval);
    }, []);

    if (!isVisible) return null;

    return (
        <div className="service-alert-banner">
            <div className="alert-content">
                <span className="alert-icon">⚠️</span>
                <div className="alert-message">
                    <strong>Service Alert:</strong> {downServices.length > 0 ? (
                        <span>The following services are currently down: <strong>{downServices.map(s => s.service).join(', ')}</strong>. Functional issues may occur.</span>
                    ) : (
                        <span>System maintenance or connectivity issue detected.</span>
                    )}
                </div>
                <button className="refresh-btn" onClick={checkHealth}>Check Again</button>
            </div>
        </div>
    );
};

export default ServiceStatusAlert;
