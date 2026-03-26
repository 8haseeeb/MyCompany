import React, { useState, useEffect } from 'react';
import api from '../services/api';
import './ServiceStatusAlert.css';

const gatewayBaseForHint = (import.meta.env.VITE_API_BASE_URL || 'http://localhost:5089').replace(/\/$/, '');

const ServiceStatusAlert = () => {
    const [downServices, setDownServices] = useState([]);
    const [isVisible, setIsVisible] = useState(false);
    const [promoHealthy, setPromoHealthy] = useState(false);

    const checkHealth = async () => {
        try {
            const response = await api.get(`/api/v1/health?t=${Date.now()}`);
            const data = response.data;
            const statusText = String(data?.status ?? '').toLowerCase();
            const serviceName = data?.service || 'Promotions API';
            const isOk =
                response.status === 200 &&
                (statusText === 'healthy' || (serviceName && statusText !== 'unhealthy'));

            if (isOk) {
                setDownServices([]);
                setIsVisible(false);
                setPromoHealthy(true);
                console.log('Health OK:', data);
            } else {
                setPromoHealthy(false);
                setDownServices([{ service: `${serviceName} (unhealthy)`, status: data?.status || 'Unknown' }]);
                setIsVisible(true);
            }
        } catch (error) {
            console.error('Health check failed:', error);
            setPromoHealthy(false);
            setDownServices([{ service: 'Promotions API (offline or unreachable via gateway)', status: 'Down' }]);
            setIsVisible(true);
        }
    };

    useEffect(() => {
        checkHealth();
        const interval = setInterval(checkHealth, 5000);
        return () => clearInterval(interval);
    }, []);

    return (
        <>
            {isVisible && (
                <div className="service-alert-banner">
                    <div className="alert-content">
                        <span className="alert-icon">⚠️</span>
                        <div className="alert-message">
                            <strong>Service alert:</strong>{' '}
                            {downServices.length > 0 ? (
                                <span>
                                    The following services are currently down:{' '}
                                    <strong>{downServices.map((s) => s.service).join(', ')}</strong>. Functional issues
                                    may occur.                                     Test Promotions health (no login required):{' '}
                                    <code className="service-health-code">{gatewayBaseForHint}/api/v1/health</code>.
                                </span>
                            ) : (
                                <span>System maintenance or connectivity issue detected.</span>
                            )}
                        </div>
                        <button type="button" className="refresh-btn" onClick={checkHealth}>
                            Check again
                        </button>
                    </div>
                </div>
            )}
        </>
    );
};

export default ServiceStatusAlert;
