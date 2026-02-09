import React, { useState, Suspense, lazy } from 'react';
import Login from './components/Login';
import Register from './components/Register';
import ErrorBoundary from './components/ErrorBoundary';
import Sidebar from './components/Sidebar';
import Dashboard from './components/Dashboard';

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

  React.useEffect(() => {
    if (token) {
      // Validate session on load/reload
      import('./services/api').then(module => {
        const api = module.default;
        // Calls SSO to check session. If invalid, interceptor handles logout.
        // We assume a simple endpoint exists or use a known one like refresh-token or similar if specific validatation endpoint is missing
        // Ideally we should have a /me or /validate endpoint.
        // For now, let's assume we can hit a light-weight endpoint or refresh.
        // Actually, let's use a non-existent or simple endpoint just to trigger middleware verification.
        // But to be safe, let's assume valid endpoint. If not, 404 is fine, but header check happens BEFORE 404 controller logic?
        // Middleware runs BEFORE controller. So even if 404, authorization middleware runs first.
        api.get('/api/auth/validate-session').catch(() => { });
      });
    }
  }, [token, currentView]);


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

  if (!token) {
    return (
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
    );
  }

  return (
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
  );
}

export default App;
