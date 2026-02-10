import React, { useState, Suspense, lazy } from 'react';
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

function App() {
  const [token, setToken] = useState(localStorage.getItem('token'));
  const [userName, setUserName] = useState(localStorage.getItem('userName') || 'User');
  const [userRole, setUserRole] = useState(localStorage.getItem('userRole') || 'User');
  const [isRegistering, setIsRegistering] = useState(false);
  const [currentView, setCurrentView] = useState('dashboard');
  const [isSidebarCollapsed, setIsSidebarCollapsed] = useState(false);
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('userName');
    localStorage.removeItem('userRole');
    setToken(null);
    setUserName('User');
    setUserRole('User');
  };

  const renderView = () => {
    return (
      <ErrorBoundary>
        <Suspense fallback={<div className="loading-mfe">Loading Module...</div>}>
          {(() => {
            switch (currentView) {
              case 'dashboard': return <Dashboard userRole={userRole} />;
              case 'promotions': return <Promotions userRole={userRole} />;
              case 'customer_relation': return <CustomerRelation userRole={userRole} />;
              case 'participant': return <Participant userRole={userRole} />;
              case 'delivery_point': return <DeliveryPoint userRole={userRole} />;
              case 'products': return <Products userRole={userRole} />;
              default: return <Dashboard userRole={userRole} />;
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
            <Register onToggle={() => setIsRegistering(false)} />
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
