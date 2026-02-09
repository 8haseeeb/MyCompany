import React, { useState } from 'react';
import { Search, X, Package, FileText, Users, MapPin, Building2, AlertCircle, Info } from 'lucide-react';
import './Promotions.css';
import './CustomerRelation.css';
import { promotionService } from '../services/promotionService';

const PromotionDetailView = ({ onClose }) => {
    const [idAction, setIdAction] = useState('');
    const [detailData, setDetailData] = useState(null);
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState(null);
    const [activeTab, setActiveTab] = useState('promoAction');

    // Pagination State
    const [currentPage, setCurrentPage] = useState(1);
    const [itemsPerPage, setItemsPerPage] = useState(5);

    // Reset pagination on tab change or data refresh
    React.useEffect(() => {
        setCurrentPage(1);
    }, [activeTab, detailData]);

    const handleSearch = async () => {
        if (!idAction || !idAction.trim()) {
            setError('Please enter a Promotion ID');
            return;
        }

        setIsLoading(true);
        setError(null);
        setDetailData(null);

        try {
            const data = await promotionService.getCompletePromotion(idAction.trim());
            setDetailData(data);
            setActiveTab('promoAction'); // Reset to first tab
        } catch (err) {
            console.error('Error fetching complete promotion:', err);
            setError(err.response?.data?.message || 'Failed to fetch promotion details. Please check the ID.');
        } finally {
            setIsLoading(false);
        }
    };

    const tabs = [
        { id: 'promoAction', label: 'Promo Action', icon: Info, count: detailData?.promoAction ? 1 : 0 },
        { id: 'products', label: 'Products', icon: Package, count: detailData?.products?.length || 0 },
        { id: 'details', label: 'Details', icon: Info, count: detailData?.productDetails?.length || 0 },
        { id: 'articles', label: 'Articles', icon: FileText, count: detailData?.articles?.length || 0 },
        { id: 'measures', label: 'Measure Fields', icon: FileText, count: detailData?.measureFields?.length || 0 },
        { id: 'participants', label: 'Participants', icon: Users, count: detailData?.participants?.length || 0 },
        { id: 'deliveryPoints', label: 'Delivery Points', icon: MapPin, count: detailData?.deliveryPoints?.length || 0 },
        { id: 'customers', label: 'Customers', icon: Building2, count: detailData?.customers?.length || 0 }
    ];

    const calculateStatus = (startDate, endDate) => {
        if (!startDate || !endDate) return { label: 'Unknown', class: 'unknown' };

        const now = new Date();
        const start = new Date(startDate);
        const end = new Date(endDate);

        // Reset hours for pure date comparison
        now.setHours(0, 0, 0, 0);
        start.setHours(0, 0, 0, 0);
        end.setHours(0, 0, 0, 0);

        if (now < start) {
            return { label: 'Upcoming', class: 'pending' };
        } else if (now >= start && now <= end) {
            return { label: 'Active', class: 'completed' };
        } else {
            return { label: 'Expired', class: 'expired' };
        }
    };

    const renderPromoHeader = () => {
        if (!detailData?.promoAction) return null;

        const promo = detailData.promoAction;
        const status = calculateStatus(promo.dteStartSellIn, promo.dteEndSellIn);

        return (
            <div className="promo-detail-header">
                <div className="promo-header-card">
                    <div className="promo-header-title">
                        <h2>Promotion #{promo.idAction}</h2>
                        <span className={`status-badge ${status.class}`}>
                            {status.label}
                        </span>
                    </div>
                    <div className="promo-header-details">
                        <div className="detail-item">
                            <span className="detail-label">Name:</span>
                            <span className="detail-value">{promo.name || '-'}</span>
                        </div>
                        <div className="detail-item">
                            <span className="detail-label">Code Div:</span>
                            <span className="detail-value">{promo.codDiv || '-'}</span>
                        </div>
                        <div className="detail-item">
                            <span className="detail-label">Start Sell In:</span>
                            <span className="detail-value">{promo.dteStartSellIn ? new Date(promo.dteStartSellIn).toLocaleDateString() : '-'}</span>
                        </div>
                        <div className="detail-item">
                            <span className="detail-label">End Sell In:</span>
                            <span className="detail-value">{promo.dteEndSellIn ? new Date(promo.dteEndSellIn).toLocaleDateString() : '-'}</span>
                        </div>
                    </div>
                </div>
            </div>
        );
    };

    const renderTabContent = () => {
        if (!detailData) return null;

        switch (activeTab) {
            case 'promoAction':
                return renderTable(
                    detailData.promoAction ? [detailData.promoAction] : [],
                    ['ID', 'Name', 'Code Div', 'Start Sell In', 'End Sell In', 'Document Key', 'Level Participants'],
                    (p) => [
                        p.idAction,
                        p.name,
                        p.codDiv,
                        p.dteStartSellIn ? new Date(p.dteStartSellIn).toLocaleDateString() : '-',
                        p.dteEndSellIn ? new Date(p.dteEndSellIn).toLocaleDateString() : '-',
                        p.documentKey || '-',
                        p.levParticipants ?? '-'
                    ]
                );

            case 'products':
                return renderTable(
                    detailData.products,
                    ['ID', 'Cod Product', 'Cod Display', 'Code Div', 'Qty Estimated', 'Discount 1', 'Discount 2'],
                    (p) => [
                        p.idAction,
                        p.codProduct,
                        p.codDisplay || '-',
                        p.codDiv,
                        p.qtyEstimated ?? 0,
                        p.perceDiscount1 ?? 0,
                        p.perceDiscount2 ?? 0
                    ]
                );

            case 'details':
                return renderTable(
                    detailData.productDetails,
                    ['ID', 'Code Node', 'Code Div', 'Flag Inclusion'],
                    (d) => [
                        d.idAction,
                        d.codNode,
                        d.codDiv,
                        d.flgInclusion ? 'Yes' : 'No'
                    ]
                );

            case 'articles':
                return renderTable(
                    detailData.articles,
                    ['ID', 'Code Div', 'Code Node', 'Code Node 1', 'Code Node 2', 'Code Node N'],
                    (a) => [
                        a.idAction,
                        a.codDiv,
                        a.codNode,
                        a.codNode1 || '-',
                        a.codNode2 || '-',
                        a.codNodeN || '-'
                    ]
                );

            case 'measures':
                return renderTable(
                    detailData.measureFields,
                    ['Code Div', 'Code Measure', 'Field Name', 'Formula'],
                    (m) => [
                        m.codDiv,
                        m.codMeasure,
                        m.fieldName,
                        m.formula || '-'
                    ]
                );

            case 'participants':
                return renderTable(
                    detailData.participants,
                    ['ID Action', 'Code Participant', 'Code Hier', 'Code Div', 'Code Node', 'Level', 'Inclusion'],
                    (p) => [
                        p.idAction,
                        p.codParticipant,
                        p.codHier,
                        p.codDiv,
                        p.codNode,
                        p.idLevel,
                        p.flgInclusion ? 'Yes' : 'No'
                    ]
                );

            case 'deliveryPoints':
                return renderTable(
                    detailData.deliveryPoints,
                    ['ID Action', 'Code Delivery Point', 'Code Hier', 'Code Div', 'Code Node', 'Level', 'Inclusion'],
                    (d) => [
                        d.idAction,
                        d.codDeliveryPoint,
                        d.codHier,
                        d.codDiv,
                        d.codNode,
                        d.idLevel,
                        d.flgInclusion ? 'Yes' : 'No'
                    ]
                );

            case 'customers':
                return renderTable(
                    detailData.customers,
                    ['ID Action', 'Code Hier', 'Code Div', 'Code Node', 'Level', 'Start Date', 'Parent Node', 'End Date'],
                    (c) => [
                        c.idAction,
                        c.codHier,
                        c.codDiv,
                        c.codNode,
                        c.idLevel,
                        c.dteStart ? new Date(c.dteStart).toLocaleDateString() : '-',
                        c.codParentNode || '-',
                        c.dteEnd ? new Date(c.dteEnd).toLocaleDateString() : '-'
                    ]
                );

            default:
                return null;
        }
    };

    const renderTable = (data, headers, rowMapper) => {
        if (!data || data.length === 0) {
            return (
                <div className="empty-state">
                    <AlertCircle size={48} color="#9ca3af" />
                    <p>No data available for this section</p>
                </div>
            );
        }

        // Pagination Logic
        const indexOfLastItem = currentPage * itemsPerPage;
        const indexOfFirstItem = indexOfLastItem - itemsPerPage;
        const currentItems = data.slice(indexOfFirstItem, indexOfLastItem);
        const totalPages = Math.ceil(data.length / itemsPerPage);

        const handlePageChange = (pageNumber) => {
            setCurrentPage(pageNumber);
        };

        const handleItemsPerPageChange = (e) => {
            setItemsPerPage(Number(e.target.value));
            setCurrentPage(1);
        };

        return (
            <div style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
                <div className="view-table-wrapper">
                    <table className="view-data-table">
                        <thead>
                            <tr>
                                {headers.map(h => <th key={h}>{h}</th>)}
                            </tr>
                        </thead>
                        <tbody>
                            {currentItems.map((item, idx) => (
                                <tr key={idx}>
                                    {rowMapper(item).map((cell, i) => <td key={i}>{cell}</td>)}
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>

                {/* Pagination UI */}
                <div className="pagination-container">
                    <div className="pagination-info">
                        Showing {currentItems.length > 0 ? indexOfFirstItem + 1 : 0} to {Math.min(indexOfLastItem, data.length)} of {data.length} results
                    </div>
                    <div className="pagination-controls">
                        <div className="per-page-wrapper">
                            <select className="per-page-select" value={itemsPerPage} onChange={handleItemsPerPageChange}>
                                {[5, 10, 20, 50].map(val => (
                                    <option key={val} value={val}>{val} per page</option>
                                ))}
                            </select>
                        </div>
                        <div className="pagination-buttons">
                            <button
                                className="page-btn nav-btn"
                                onClick={() => handlePageChange(currentPage - 1)}
                                disabled={currentPage === 1}
                            >
                                Previous
                            </button>
                            {[...Array(totalPages)].map((_, i) => (
                                <button
                                    key={i + 1}
                                    className={`page-btn ${currentPage === i + 1 ? 'active' : ''}`}
                                    onClick={() => handlePageChange(i + 1)}
                                >
                                    {i + 1}
                                </button>
                            ))}
                            <button
                                className="page-btn nav-btn"
                                onClick={() => handlePageChange(currentPage + 1)}
                                disabled={currentPage === totalPages || totalPages === 0}
                            >
                                Next
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        );
    };

    return (
        <div className="promotions-container">
            <div className="promo-page-header">
                <h1 className="promo-page-title">Promotion Detail View</h1>
                {onClose && (
                    <button className="close-overlay-btn" onClick={onClose} style={{ position: 'static', background: '#f3f4f6' }}>
                        <X size={20} />
                    </button>
                )}
            </div>

            {/* Search Section */}
            <div className="detail-search-section">
                <label className="search-label">Enter Promotion ID <span className="required">*</span></label>
                <div className="search-input-group">
                    <input
                        type="number"
                        className="form-input"
                        placeholder="e.g., 3"
                        value={idAction}
                        onChange={(e) => setIdAction(e.target.value)}
                        onKeyPress={(e) => e.key === 'Enter' && handleSearch()}
                    />
                    <button className="search-btn" onClick={handleSearch} disabled={isLoading}>
                        <Search size={18} />
                        {isLoading ? 'Searching...' : 'Search'}
                    </button>
                </div>
            </div>

            {/* Error State */}
            {error && (
                <div className="error-message">
                    <AlertCircle size={20} />
                    <span>{error}</span>
                </div>
            )}

            {/* Loading State */}
            {isLoading && (
                <div className="loading-state">
                    <div className="loading-spinner"></div>
                    <p>Loading promotion details...</p>
                </div>
            )}

            {/* Promotion Header */}
            {renderPromoHeader()}

            {/* Tabbed Content */}
            {detailData && (
                <div className="detail-tabs-container">
                    <div className="detail-tabs">
                        {tabs.map(tab => {
                            const Icon = tab.icon;
                            return (
                                <button
                                    key={tab.id}
                                    className={`detail-tab ${activeTab === tab.id ? 'active' : ''}`}
                                    onClick={() => setActiveTab(tab.id)}
                                >
                                    <Icon size={18} />
                                    <span>{tab.label}</span>
                                    <span className="tab-count">{tab.count}</span>
                                </button>
                            );
                        })}
                    </div>
                    <div className="detail-tab-content">
                        {renderTabContent()}
                    </div>
                </div>
            )}
        </div>
    );
};

export default PromotionDetailView;
