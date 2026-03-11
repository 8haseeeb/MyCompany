import React, { useState, useEffect } from 'react';
import api from '../services/api';
import './ServiceStatusAlert.css';

const ServiceStatusAlert = () => {
    const [downServices, setDownServices] = useState([]);
    const [isVisible, setIsVisible] = useState(false);

    const checkHealth = async () => {
        try {
            const response = await api.get(`/api/Health?t=${new Date().getTime()}`);
            const data = response.data;
            console.log("✅ Health Response:", data);

            // If we get a response, services are healthy
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
        const interval = setInterval(checkHealth, 5000); // Check every 5 seconds
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
