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
  const [isRegistering, setIsRegistering] = useState(false);
  const [currentView, setCurrentView] = useState('dashboard');
  const [isSidebarCollapsed, setIsSidebarCollapsed] = useState(false);

  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('userName');
    setToken(null);
    setUserName('User');
  };


  const renderView = () => {
    return (
      <ErrorBoundary>
        <Suspense fallback={<div className="loading-mfe">Loading Module...</div>}>
          {(() => {
            switch (currentView) {
              case 'dashboard': return <Dashboard />;
              case 'promotions': return <Promotions />;
              case 'customer_relation': return <CustomerRelation />;
              case 'participant': return <Participant />;
              case 'delivery_point': return <DeliveryPoint />;
              case 'products': return <Products />;
              default: return <Dashboard />;
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
          <Login setToken={setToken} setUserName={setUserName} onToggle={() => setIsRegistering(true)} />
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
      />

      <div className={`main-content ${isSidebarCollapsed ? 'collapsed' : 'expanded'}`}>
        <header className="app-header">
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
