import React, { useState, useEffect } from 'react';
import {
    PieChart, Pie, Cell, BarChart, Bar, XAxis, YAxis,
    CartesianGrid, Tooltip, Legend, ResponsiveContainer
} from 'recharts';
import {
    Tag, Users, MapPin, Briefcase, TrendingUp,
    RefreshCw, ChevronDown, CheckCircle
} from 'lucide-react';
import { promotionService } from '../services/promotionService';
import { customerService } from '../services/customerService';
import './Dashboard.css';

const Dashboard = () => {
    const [stats, setStats] = useState({
        promotions: 0,
        participants: 0,
        deliveryPoints: 0,
        customers: 0
    });
    const [isLoading, setIsLoading] = useState(false);

    useEffect(() => {
        fetchDashboardData();
    }, []);

    const fetchDashboardData = async () => {
        setIsLoading(true);
        try {
            const [promos, parts, points, custs, dashboardMetrics] = await Promise.all([
                promotionService.getPromotionHistory(),
                promotionService.getParticipants(),
                promotionService.getDeliveryPoints(),
                customerService.getCustomers(),
                promotionService.getDashboardStats()
            ]);

            setStats({
                promotions: promos?.length || 0,
                participants: parts?.length || 0,
                deliveryPoints: points?.length || 0,
                customers: custs?.length || 0,
                activePromotions: dashboardMetrics?.activeActions || 0,
                activityTrend: dashboardMetrics?.activityTrend || []
            });
        } catch (error) {
            console.error("Error fetching dashboard stats:", error);
        } finally {
            setIsLoading(false);
        }
    };

    const distributionData = [
        { name: 'Promotions', value: stats.promotions, color: '#a855f7' },
        { name: 'Participants', value: stats.participants, color: '#3b82f6' },
        { name: 'Delivery Points', value: stats.deliveryPoints, color: '#f97316' },
        { name: 'Customers', value: stats.customers, color: '#eab308' },
    ];

    return (
        <div className="dashboard-container fade-in">
            <div className="dashboard-header">

                <div className="header-actions">
                    <div className="time-select">
                        <span>This Month</span>
                        <ChevronDown size={16} />
                    </div>
                    <button className="refresh-btn" onClick={fetchDashboardData} disabled={isLoading}>
                        <RefreshCw size={16} className={isLoading ? 'spin' : ''} />
                        Refresh
                    </button>
                </div>
            </div>

            <div className="stats-grid">
                <div className="stat-card purple">
                    <div className="stat-icon"><Tag size={20} /></div>
                    <div className="stat-info">
                        <span className="stat-label">Total Promotions</span>
                        <span className="stat-value">{stats.promotions}</span>
                        <span className="stat-sub">↑ 12% vs last month</span>
                    </div>
                </div>

                <div className="stat-card blue">
                    <div className="stat-icon"><Users size={20} /></div>
                    <div className="stat-info">
                        <span className="stat-label">Participants</span>
                        <span className="stat-value">{stats.participants}</span>
                        <span className="stat-sub">87% Success Rate</span>
                    </div>
                </div>

                <div className="stat-card orange">
                    <div className="stat-icon"><MapPin size={20} /></div>
                    <div className="stat-info">
                        <span className="stat-label">Delivery Points</span>
                        <span className="stat-value">{stats.deliveryPoints}</span>
                        <span className="stat-sub">Live across regions</span>
                    </div>
                </div>

                <div className="stat-card yellow">
                    <div className="stat-icon"><Briefcase size={20} /></div>
                    <div className="stat-info">
                        <span className="stat-label">Total Customers</span>
                        <span className="stat-value">{stats.customers}</span>
                        <span className="stat-sub">Active Relations</span>
                    </div>
                </div>

                <div className="stat-card pink">
                    <div className="stat-icon"><CheckCircle size={20} /></div>
                    <div className="stat-info">
                        <span className="stat-label">Active Promotions</span>
                        <span className="stat-value">{stats.activePromotions || 0}</span>
                        <span className="stat-sub">Live in stores</span>
                    </div>
                </div>
            </div>

            <div className="charts-container">
                <div className="chart-wrapper pie-chart-box">
                    <h3 className="chart-title">Data Distribution</h3>
                    <div className="chart-content">
                        <ResponsiveContainer width="100%" height={300}>
                            <PieChart>
                                <Pie
                                    data={distributionData}
                                    innerRadius={60}
                                    outerRadius={100}
                                    paddingAngle={5}
                                    dataKey="value"
                                >
                                    {distributionData.map((entry, index) => (
                                        <Cell key={`cell-${index}`} fill={entry.color} />
                                    ))}
                                </Pie>
                                <Tooltip />
                                <Legend verticalAlign="bottom" height={36} />
                            </PieChart>
                        </ResponsiveContainer>
                    </div>
                </div>

                <div className="chart-wrapper bar-chart-box">
                    <h3 className="chart-title">Activity Trend (30 Days)</h3>
                    <div className="chart-content">
                        <ResponsiveContainer width="100%" height={300}>
                            <BarChart data={stats.activityTrend || []}>
                                <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="#f1f5f9" />
                                <XAxis dataKey="dateLabel" axisLine={false} tickLine={false} />
                                <YAxis axisLine={false} tickLine={false} />
                                <Tooltip cursor={{ fill: '#f8fafc' }} />
                                <Legend verticalAlign="bottom" height={36} />
                                <Bar dataKey="active" fill="#3b82f6" radius={[4, 4, 0, 0]} />
                                <Bar dataKey="pending" fill="#10b981" radius={[4, 4, 0, 0]} />
                                <Bar dataKey="failed" fill="#ef4444" radius={[4, 4, 0, 0]} />
                            </BarChart>
                        </ResponsiveContainer>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Dashboard;
