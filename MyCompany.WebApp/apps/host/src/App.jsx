import React, { useState, useEffect, Suspense, lazy } from 'react';
import api from './services/api';
import Login from './components/Login';
import Register from './components/Register';
import ErrorBoundary from './components/ErrorBoundary';
import Sidebar from './components/Sidebar';
import Dashboard from './components/Dashboard';
import ServiceStatusAlert from './components/ServiceStatusAlert';

const Promotions = lazy(() => import('promotions_app/Promotions'));
const CustomerRelation = lazy(() => import('promotions_app/CustomerRelation'));
const Participant = lazy(() => import('promotions_app/Participant'));
const DeliveryPoint = lazy(() => import('promotions_app/DeliveryPoint'));
const Products = lazy(() => import('promotions_app/Products'));

import './App.css';
import { canEditContent } from './utils/rbac';

function App() {
  const [token, setToken] = useState(() => localStorage.getItem('token') || localStorage.getItem('accessToken'));
  const [userName, setUserName] = useState(localStorage.getItem('userName') || 'User');
  const [userRole, setUserRole] = useState(localStorage.getItem('userRole') || 'User');
  const [isRegistering, setIsRegistering] = useState(false);
  const [currentView, setCurrentView] = useState('dashboard');
  const [isSidebarCollapsed, setIsSidebarCollapsed] = useState(false);
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

  useEffect(() => {
    const stored = localStorage.getItem('token') || localStorage.getItem('accessToken');
    if (stored) {
      if (!api.defaults.headers.common['Authorization']) {
        api.defaults.headers.common['Authorization'] = `Bearer ${stored}`;
      }
      setToken((t) => t || stored);
    }
  }, []);

  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('userName');
    localStorage.removeItem('userRole');
    delete api.defaults.headers.common['Authorization'];
    setToken(null);
    setUserName('User');
    setUserRole('User');
  };

  const canEdit = canEditContent(userRole);

  useEffect(() => {
    const onRoleUpdated = (e) => {
      const next = e.detail?.role;
      if (typeof next === 'string' && next) setUserRole(next);
    };
    window.addEventListener('auth-role-updated', onRoleUpdated);
    return () => window.removeEventListener('auth-role-updated', onRoleUpdated);
  }, []);

  const renderView = () => {
    return (
      <ErrorBoundary>
        <Suspense fallback={<div className="loading-mfe">Loading Module...</div>}>
          {(() => {
            switch (currentView) {
              case 'dashboard': return <Dashboard userRole={userRole} canEdit={canEdit} />;
              case 'promotions': return <Promotions canEdit={canEdit} />;
              case 'customer_relation': return <CustomerRelation canEdit={canEdit} />;
              case 'participant': return <Participant canEdit={canEdit} />;
              case 'delivery_point': return <DeliveryPoint canEdit={canEdit} />;
              case 'products': return <Products canEdit={canEdit} />;
              default: return <Dashboard userRole={userRole} canEdit={canEdit} />;
            }
          })()}
        </Suspense>
      </ErrorBoundary>
    );
  };

  return (
    <>
      <ServiceStatusAlert />
      {!token ? (
        <div className="auth-wrapper">
          {isRegistering ? (
            <Register onSwitchToLogin={() => setIsRegistering(false)} />
          ) : (
            <Login
              setToken={setToken}
              setUserName={setUserName}
              setUserRole={setUserRole}
              onToggle={() => setIsRegistering(true)}
            />
          )}
        </div>
      ) : (
        <div className="app-wrapper">
          <Sidebar
            isCollapsed={isSidebarCollapsed}
            setIsCollapsed={setIsSidebarCollapsed}
            currentView={currentView}
            setView={setCurrentView}
            onLogout={handleLogout}
            isMobileOpen={isMobileMenuOpen}
            setIsMobileOpen={setIsMobileMenuOpen}
            userRole={userRole}
            canEdit={canEdit}
          />

          <div className={`main-content ${isSidebarCollapsed ? 'collapsed' : 'expanded'}`}>
            <header className="app-header">
              <button
                className="mobile-menu-toggle"
                onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
              >
                <span className="hamburger-bar"></span>
                <span className="hamburger-bar"></span>
                <span className="hamburger-bar"></span>
              </button>
              <div className="header-title">
                {currentView === 'promotions' ? 'Create Promotion' : currentView.split('_').map(w => w.charAt(0).toUpperCase() + w.slice(1)).join(' ')}
              </div>

              <div className="header-actions">
                <div className="user-profile">
                  <div className="avatar">
                    <img src={`https://api.dicebear.com/7.x/avataaars/svg?seed=${userName}`} alt={userName} />
                  </div>
                  <span className="user-name">{userName}</span>
                </div>
              </div>
            </header>

            <main className="page-content">
              {renderView()}
            </main>
          </div>
        </div>
      )}
    </>
  );
}

export default App;
