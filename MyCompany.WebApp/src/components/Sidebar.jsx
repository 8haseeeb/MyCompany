import React, { useState } from 'react';
import { LayoutDashboard, Tag, Users, ChevronLeft, ChevronRight, LogOut, ChevronDown, ChevronUp, User, MapPin } from 'lucide-react';
import './Sidebar.css';

const Sidebar = ({ isCollapsed, setIsCollapsed, currentView, setView, onLogout }) => {
    const [expandedMenus, setExpandedMenus] = useState(['customer_relation']); // Default open like screenshot

    const menuItems = [
        { id: 'dashboard', label: 'Dashboard', icon: <LayoutDashboard size={20} /> },
        { id: 'promotions', label: 'Promotions', icon: <Tag size={20} /> },
        {
            id: 'customer_relation',
            label: 'Customer Relation',
            icon: <Users size={20} />,
            subItems: [
                { id: 'customer_relation', label: 'Manage Customer', icon: <Users size={16} /> },
                { id: 'participant', label: 'Participant', icon: <User size={16} /> },
                { id: 'delivery_point', label: 'Delivery Point', icon: <MapPin size={16} /> }
            ]
        },
    ];

    const toggleMenu = (id) => {
        setExpandedMenus(prev =>
            prev.includes(id) ? prev.filter(m => m !== id) : [...prev, id]
        );
    };

    const handleItemClick = (item) => {
        if (item.subItems) {
            toggleMenu(item.id);
        } else {
            setView(item.id);
        }
    };

    return (
        <aside className={`sidebar ${isCollapsed ? 'collapsed' : ''}`}>
            <div className="sidebar-header">
                {!isCollapsed && (
                    <div className="brand">
                        <span className="brand-xtel">XTEL</span>
                        <span className="brand-promo">PROMO</span>
                    </div>
                )}
                <button className="toggle-btn" onClick={() => setIsCollapsed(!isCollapsed)}>
                    {isCollapsed ? <ChevronRight size={16} /> : <ChevronLeft size={16} />}
                </button>
            </div>

            <nav className="nav-menu">
                {menuItems.map((item) => {
                    const isExpanded = expandedMenus.includes(item.id);
                    const hasSubItems = item.subItems && item.subItems.length > 0;

                    return (
                        <div key={item.id} className="menu-group">
                            <button
                                onClick={() => handleItemClick(item)}
                                className={`nav-item ${(currentView === item.id || (hasSubItems && isExpanded)) ? 'active' : ''}`}
                                title={isCollapsed ? item.label : ''}
                            >
                                <span className="menu-icon">{item.icon}</span>
                                <span className="nav-label">{item.label}</span>
                                {!isCollapsed && hasSubItems && (
                                    <span className="expand-icon">
                                        {isExpanded ? <ChevronUp size={16} /> : <ChevronDown size={16} />}
                                    </span>
                                )}
                            </button>

                            {!isCollapsed && hasSubItems && isExpanded && (
                                <div className="submenu fade-in">
                                    {item.subItems.map((sub) => (
                                        <button
                                            key={sub.id}
                                            onClick={() => setView(sub.id)}
                                            className={`submenu-item ${currentView === sub.id ? 'active' : ''}`}
                                        >
                                            <span className="menu-icon sub-icon">{sub.icon}</span>
                                            <span className="nav-label">{sub.label}</span>
                                        </button>
                                    ))}
                                </div>
                            )}
                        </div>
                    );
                })}
            </nav>

            <div className="sidebar-footer">
                <button
                    onClick={onLogout}
                    className="nav-item logout-item"
                    title={isCollapsed ? "Sign Out" : ""}
                >
                    <span className="menu-icon"><LogOut size={20} /></span>
                    <span className="nav-label">Sign Out</span>
                </button>
            </div>
        </aside>
    );
};

export default Sidebar;
