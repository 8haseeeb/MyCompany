import React, { useState, useEffect } from 'react';
import api from '../services/api';
import './ServiceStatusAlert.css';

const ServiceStatusAlert = () => {
    const [downServices, setDownServices] = useState([]);
    const [isVisible, setIsVisible] = useState(false);

    const checkHealth = async () => {
        try {
            const response = await api.get(`/api/gateway/health?t=${new Date().getTime()}`);
            const data = response.data;
            console.log("✅ Gateway Health Response:", data);

            if (data.status === 'Unhealthy') {
                const results = data.services || [];
                const failed = results.filter(s => s.status === 'Down' || s.status === 'Unhealthy');
                setDownServices(failed);
                setIsVisible(true);
            } else {
                setDownServices([]);
                setIsVisible(false);
            }
        } catch (error) {
            console.error("❌ Gateway Health Connection Error:", error);
            // This happens if the Gateway itself is unreachable
            setDownServices([{ service: 'API Gateway (Offline)', status: 'Down' }]);
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
