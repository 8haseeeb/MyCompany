import React, { useState, useEffect } from 'react';
import { MapPin, Eye, Search } from 'lucide-react';
import './CustomerRelation.css'; // Reusing established table styles
import { promotionService } from '../services/promotionService';

const DeliveryPoint = () => {
    const [deliveryPoints, setDeliveryPoints] = useState([]);
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState(null);
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

    const filteredData = deliveryPoints.filter(dp =>
        (dp.codDeliveryPoint || dp.CodDeliveryPoint || '').toString().toLowerCase().includes(searchTerm.toLowerCase()) ||
        (dp.codHier || dp.CodHier || '').toString().toLowerCase().includes(searchTerm.toLowerCase())
    );

    return (
        <div className="cr-container">
            <div className="cr-table-container fade-in">
                <div className="search-by-section">
                    <h3 className="search-by-title">Delivery Points List</h3>
                    <div className="search-controls">
                        <div style={{ display: 'flex', gap: '12px', flex: 1 }}>
                            <div className="search-input-wrapper" style={{ position: 'relative', flex: 1 }}>
                                <input
                                    type="text"
                                    className="search-term-input"
                                    placeholder="Search by ID or Hierarchy..."
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
