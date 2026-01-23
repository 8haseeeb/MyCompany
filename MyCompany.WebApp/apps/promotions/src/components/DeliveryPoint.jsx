import React, { useState, useEffect } from 'react';
import { MapPin, Eye, Search, ChevronRight } from 'lucide-react';
import './CustomerRelation.css'; // Reusing established table styles
import { promotionService } from '../services/promotionService';

const DeliveryPoint = () => {
    const [deliveryPoints, setDeliveryPoints] = useState([]);
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState(null);
    const [searchCriterion, setSearchCriterion] = useState('Select an option');
    const [isDropdownOpen, setIsDropdownOpen] = useState(false);
    const [searchTerm, setSearchTerm] = useState('');

    useEffect(() => {
        fetchDeliveryPoints();
    }, []);

    const fetchDeliveryPoints = async () => {
        setIsLoading(true);
        setError(null);
        try {
            const data = await promotionService.getDeliveryPoints();
            setDeliveryPoints(data || []);
        } catch (err) {
            console.error("Error fetching delivery points:", err);
            setError(err.message || "Failed to fetch delivery points");
        } finally {
            setIsLoading(false);
        }
    };

    const filteredData = deliveryPoints.filter(dp => {
        if (searchCriterion === 'Select an option' || !searchTerm) return true;

        const term = searchTerm.toLowerCase();
        switch (searchCriterion) {
            case 'ID Action': return (dp.idAction || dp.IdAction || '').toString().toLowerCase().includes(term);
            case 'Cod Delivery Point': return (dp.codDeliveryPoint || dp.CodDeliveryPoint || '').toString().toLowerCase().includes(term);
            case 'Code Hier': return (dp.codHier || dp.CodHier || '').toString().toLowerCase().includes(term);
            case 'Code Div': return (dp.codDiv || dp.CodDiv || '').toString().toLowerCase().includes(term);
            case 'Code Node': return (dp.codNode || dp.CodNode || '').toString().toLowerCase().includes(term);
            case 'ID Level': return (dp.idLevel ?? dp.IdLevel ?? '').toString().toLowerCase().includes(term);
            default: return true;
        }
    });

    return (
        <div className="cr-container">
            <div className="cr-table-container fade-in">
                <div className="search-by-section">
                    <h3 className="search-by-title">Delivery Points List</h3>
                    <div className="search-controls">
                        <div style={{ display: 'flex', gap: '12px', flex: 1 }}>
                            <div className="custom-dropdown-container" style={{ width: '200px' }}>
                                <div
                                    className={`custom-dropdown-header ${isDropdownOpen ? 'active' : ''}`}
                                    onClick={() => setIsDropdownOpen(!isDropdownOpen)}
                                >
                                    <span>{searchCriterion}</span>
                                    <ChevronRight
                                        size={18}
                                        style={{
                                            transform: isDropdownOpen ? 'rotate(90deg)' : 'rotate(0deg)',
                                            transition: 'transform 0.2s ease',
                                            color: '#9333ea'
                                        }}
                                    />
                                </div>
                                {isDropdownOpen && (
                                    <div className="custom-dropdown-list">
                                        {['Select an option', 'ID Action', 'Cod Delivery Point', 'Code Hier', 'Code Div', 'Code Node', 'ID Level'].map(opt => (
                                            <div
                                                key={opt}
                                                className={`custom-dropdown-item ${searchCriterion === opt ? 'selected' : ''}`}
                                                onClick={() => {
                                                    setSearchCriterion(opt);
                                                    setIsDropdownOpen(false);
                                                }}
                                            >
                                                {opt}
                                            </div>
                                        ))}
                                    </div>
                                )}
                            </div>
                            <div className="search-input-wrapper" style={{ position: 'relative', flex: 1 }}>
                                <input
                                    type="text"
                                    className="search-term-input"
                                    placeholder="Search Term..."
                                    value={searchTerm}
                                    onChange={(e) => setSearchTerm(e.target.value)}
                                    style={{ paddingLeft: '40px' }}
                                />
                                <Search size={18} style={{ position: 'absolute', left: '12px', top: '50%', transform: 'translateY(-50%)', color: '#94a3b8' }} />
                            </div>
                            <button className="search-action-btn" onClick={fetchDeliveryPoints}>
                                <Eye size={18} /> Refresh
                            </button>
                        </div>
                    </div>

                    <div className="view-table-wrapper">
                        <table className="view-data-table">
                            <thead>
                                <tr>
                                    <th>ID Action</th>
                                    <th>Cod Delivery Point</th>
                                    <th>Code Hier</th>
                                    <th>Code Div</th>
                                    <th>Code Node</th>
                                    <th>ID Level</th>
                                    <th>Flag Inclusion</th>
                                </tr>
                            </thead>
                            <tbody>
                                {isLoading ? (
                                    <tr><td colSpan="7" style={{ textAlign: 'center', padding: '40px' }}>Loading Delivery Points...</td></tr>
                                ) : filteredData.length > 0 ? (
                                    filteredData.map((row, idx) => (
                                        <tr key={idx}>
                                            <td>{row.idAction || row.IdAction}</td>
                                            <td>{row.codDeliveryPoint || row.CodDeliveryPoint}</td>
                                            <td>{row.codHier || row.CodHier}</td>
                                            <td>{row.codDiv || row.CodDiv}</td>
                                            <td>{row.codNode || row.CodNode}</td>
                                            <td>{row.idLevel ?? row.IdLevel}</td>
                                            <td>{row.flgInclusion ?? row.FlgInclusion ? 'Yes' : 'No'}</td>
                                        </tr>
                                    ))
                                ) : (
                                    <tr><td colSpan="7" style={{ textAlign: 'center', padding: '40px' }}>No delivery points found.</td></tr>
                                )}
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default DeliveryPoint;
