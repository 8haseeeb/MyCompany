import React, { useState, useEffect } from 'react';
import { MessageSquare, ChevronRight, ChevronLeft, Check, Plus, Ticket, Eye, X, MoreVertical, Trash2, Edit2, Search } from 'lucide-react';
import './Promotions.css';
import { promotionService } from '../services/promotionService';
import PromotionDetailView from './PromotionDetailView';

const Promotions = ({ canEdit = false }) => {
    const [showForm, setShowForm] = useState(false);
    const [showViewModal, setShowViewModal] = useState(false);
    const [showDetailView, setShowDetailView] = useState(false);
    const [selectedViewType, setSelectedViewType] = useState('');
    const [isDropdownOpen, setIsDropdownOpen] = useState(false);
    const [isSearchDropdownOpen, setIsSearchDropdownOpen] = useState(false);
    const [searchCriterion, setSearchCriterion] = useState('Select an option');
    const [searchTerm, setSearchTerm] = useState('');
    const [activities, setActivities] = useState([]);
    const [isLoading, setIsLoading] = useState(false);
    const [fetchError, setFetchError] = useState(null);
    const [activeActionMenu, setActiveActionMenu] = useState(null);
    const [customerRelations, setCustomerRelations] = useState([]);
    const [masterParticipants, setMasterParticipants] = useState([]);
    const [masterDps, setMasterDps] = useState([]);
    const [showEditPromoModal, setShowEditPromoModal] = useState(false);
    const [editingPromotion, setEditingPromotion] = useState(null);
    const [editPromoFormData, setEditPromoFormData] = useState({
        name: '',
        dteStartSellIn: '',
        dteEndSellIn: '',
        dteStartSellOut: '',
        dteEndSellOut: '',
        documentKey: '',
        dteToShost: '',
        levParticipants: 0
    });

    // Pagination State for History Table
    const [currentPage, setCurrentPage] = useState(1);
    const [itemsPerPage, setItemsPerPage] = useState(5);

    // Pagination State for Support Form View
    const [viewCurrentPage, setViewCurrentPage] = useState(1);
    const [viewItemsPerPage, setViewItemsPerPage] = useState(5);

    useEffect(() => {
        fetchHistory();
        fetchRelations();
    }, []);

    const fetchRelations = async () => {
        try {
            const { customerService } = await import('../services/customerService');
            const relData = await customerService.getCustomers();
            setCustomerRelations(relData || []);

            // Also fetch master participants and DPs for resolution fallback
            const pData = await promotionService.getParticipants();
            setMasterParticipants(pData || []);

            const dpData = await promotionService.getDeliveryPoints();
            setMasterDps(dpData || []);
        } catch (error) {
            console.error("Error fetching master data:", error);
        }
    };

    const fetchHistory = async () => {
        setIsLoading(true);
        setFetchError(null);
        try {
            const data = await promotionService.getPromotionHistory();
            setActivities(data || []);
            // Reset to first page when new data is fetched
            setCurrentPage(1);
        } catch (error) {
            console.error("Error fetching promotion history:", error);
            setFetchError(error.message || "Failed to fetch activities");
        } finally {
            setIsLoading(false);
        }
    };

    // Pagination Logic for History Table
    const indexOfLastItem = currentPage * itemsPerPage;
    const indexOfFirstItem = indexOfLastItem - itemsPerPage;
    const currentActivities = activities.slice(indexOfFirstItem, indexOfLastItem);
    const totalPages = Math.ceil(activities.length / itemsPerPage);

    const handlePageChange = (pageNumber) => {
        setCurrentPage(pageNumber);
        setActiveActionMenu(null);
    };

    const handleItemsPerPageChange = (e) => {
        setItemsPerPage(Number(e.target.value));
        setCurrentPage(1);
    };

    const steps = [
        'Promo Action',
        'Participants',
        'Delivery Points',
        'Products',
        'Details',
        'Articles',
        'Measure Fields'
    ];

    const [currentStepIndex, setCurrentStepIndex] = useState(0);
    const currentStep = steps[currentStepIndex];

    const [viewData, setViewData] = useState([]);
    const [isViewLoading, setIsViewLoading] = useState(false);
    const [viewError, setViewError] = useState(null);

    useEffect(() => {
        if (showViewModal && selectedViewType) {
            fetchViewData(selectedViewType);
        }
    }, [showViewModal, selectedViewType]);

    const fetchViewData = async (type) => {
        setIsViewLoading(true);
        setViewError(null);
        try {
            let data = [];
            switch (type) {
                case 'Promo Action': data = await promotionService.getPromoActions(); break;
                case 'Participants': data = await promotionService.getParticipants(); break;
                case 'Delivery Points': data = await promotionService.getDeliveryPoints(); break;
                case 'Products': data = await promotionService.getProducts(); break;
                case 'Details': data = await promotionService.getProductDetails(); break;
                case 'Articles': data = await promotionService.getPromoArticles(); break;
                case 'Measure Fields': data = await promotionService.getPromoMeasures(); break;
                default: break;
            }
            setViewData(data || []);
        } catch (error) {
            console.error(`Error fetching ${type}:`, error);
            setViewError(error.message || `Failed to fetch ${type}`);
            setViewData([]);
        } finally {
            setIsViewLoading(false);
        }
    };

    // Prevent background scrolling when modals are open
    useEffect(() => {
        const anyModalOpen = showDetailView || showViewModal || showEditPromoModal;
        if (anyModalOpen) {
            document.body.style.overflow = 'hidden';
        } else {
            document.body.style.overflow = 'unset';
        }
        return () => {
            document.body.style.overflow = 'unset';
        };
    }, [showDetailView, showViewModal, showEditPromoModal]);

    const handleEditPromo = (promo) => {
        setEditingPromotion(promo);
        setEditPromoFormData({
            name: promo.name || promo.Name || '',
            dteStartSellIn: (promo.dteStartSellIn || promo.DteStartSellIn || '').split('T')[0],
            dteEndSellIn: (promo.dteEndSellIn || promo.DteEndSellIn || '').split('T')[0],
            dteStartSellOut: (promo.dteStartSellOut || promo.DteStartSellOut || '').split('T')[0],
            dteEndSellOut: (promo.dteEndSellOut || promo.DteEndSellOut || '').split('T')[0],
            documentKey: promo.documentKey || promo.DocumentKey || '',
            dteToShost: (promo.dteToShost || promo.DteToShost || '').split('T')[0],
            levParticipants: promo.levParticipants ?? promo.LevParticipants ?? 0
        });
        setShowEditPromoModal(true);
        setActiveActionMenu(null);
    };

    const handleUpdatePromo = async () => {
        const idAction = editingPromotion.idAction ?? editingPromotion.IdAction ?? editingPromotion.codAction ?? editingPromotion.CodAction;
        try {
            // Ensure payload fields are correctly typed for .NET binding
            const payload = {
                ...editPromoFormData,
                levParticipants: editPromoFormData.levParticipants === '' ? null : Number(editPromoFormData.levParticipants),
                dteToShost: editPromoFormData.dteToShost === '' ? null : editPromoFormData.dteToShost,
                dteStartSellIn: editPromoFormData.dteStartSellIn === '' ? null : editPromoFormData.dteStartSellIn,
                dteEndSellIn: editPromoFormData.dteEndSellIn === '' ? null : editPromoFormData.dteEndSellIn,
                dteStartSellOut: editPromoFormData.dteStartSellOut === '' ? null : editPromoFormData.dteStartSellOut,
                dteEndSellOut: editPromoFormData.dteEndSellOut === '' ? null : editPromoFormData.dteEndSellOut
            };
            await promotionService.updatePromotion(idAction, payload);
            alert("Promotion updated successfully!");
            setShowEditPromoModal(false);
            fetchHistory();
        } catch (error) {
            console.error("Error updating promotion:", error);
            alert("Failed to update promotion: " + (error.response?.data?.error || error.message));
        }
    };

    const handleDelete = async (idAction) => {
        if (!window.confirm(`Are you sure you want to delete Promotion #${idAction}? This will remove all related data (Participants, Products, etc.) and cannot be undone.`)) {
            return;
        }

        try {
            await promotionService.deletePromotion(idAction);
            alert("Promotion deleted successfully!");
            fetchHistory(); // Refresh the list     
        } catch (error) {
            console.error("Error deleting promotion:", error);
            alert("Failed to delete promotion: " + (error.response?.data?.error || error.message));
        } finally {
            setActiveActionMenu(null);
        }
    };

    const [formData, setFormData] = useState({
        idAction: 0,
        name: '',
        codDiv: '',
        dteStartSellIn: '',
        dteEndSellIn: '',
        dteStartSellOut: '',
        dteEndSellOut: '',
        documentKey: '',
        dteToShost: '',
        levParticipants: 0,
        codParticipant: '',
        participantFlgInclusion: true,
        participantCodHier: '',
        participantCodDiv: '',
        participantCodNode: '',
        participantIdLevel: 0,
        participantDteStart: '',
        codDeliveryPoint: '',
        dpFlgInclusion: true,
        dpCodHier: '',
        dpCodDiv: '',
        dpCodNode: '',
        dpIdLevel: 0,
        dpDteStart: '',
        codProduct: '',
        levProduct: 0,
        codDisplay: '',
        productCodDiv: '',
        qtyEstimated: 0,
        perceDiscount1: 0,
        perceDiscount2: 0,
        numMeasure: 0,
        codMeasure: '',
        detailCodNode: '',
        detailCodDiv: '',
        detailFlgInclusion: true,
        artCodDiv: '',
        artCodNode: '',
        codNode1: '',
        codNode2: '',
        codNodeN: '',
        fieldName: '',
        formula: ''
    });

    const handleInputChange = (e) => {
        const { name, value, type, checked } = e.target;
        const newValue = type === 'checkbox' ? checked : value;

        setFormData(prev => {
            const updated = { ...prev, [name]: newValue };

            // Auto-resolve Hierarchy for Participants
            if (name === 'codParticipant' && value) {
                // 1. Try relation lookup
                const relation = customerRelations.find(r =>
                    r.codNode.toLowerCase() === value.toLowerCase() ||
                    (r.codHier && r.codHier.toLowerCase() === value.toLowerCase())
                );

                // 2. Try master participant fallback
                const masterMatch = !relation ? masterParticipants.find(p => p.codParticipant.toLowerCase() === value.toLowerCase()) : null;

                // 3. Smart Suffix Resolution (e.g., P-2323 matches C-2323 via "2323")
                const suffix = value.includes('-') ? value.split('-').pop() : value;
                const smartMatch = !relation && !masterMatch ? customerRelations.find(r =>
                    r.codNode.toLowerCase().endsWith('-' + suffix.toLowerCase()) ||
                    r.codNode.toLowerCase() === suffix.toLowerCase()
                ) : null;

                if (relation || masterMatch || smartMatch) {
                    const match = relation || masterMatch || smartMatch;
                    updated.participantCodHier = match.codHier;
                    updated.participantCodDiv = match.codDiv;
                    updated.participantCodNode = match.codNode;
                    updated.participantIdLevel = match.idLevel;
                    updated.participantDteStart = match.dteStart;
                } else {
                    // Reset if not found
                    updated.participantCodHier = '';
                }
            }

            // Auto-resolve Hierarchy for Delivery Points
            if (name === 'codDeliveryPoint' && value) {
                // 1. Try relation lookup
                const relation = customerRelations.find(r =>
                    r.codNode.toLowerCase() === value.toLowerCase() ||
                    (r.codHier && r.codHier.toLowerCase() === value.toLowerCase())
                );

                // 2. Try master DP fallback
                const masterMatch = !relation ? masterDps.find(dp => dp.codDeliveryPoint.toLowerCase() === value.toLowerCase()) : null;

                // 3. Smart Suffix Resolution
                const suffix = value.includes('-') ? value.split('-').pop() : value;
                const smartMatch = !relation && !masterMatch ? customerRelations.find(r =>
                    r.codNode.toLowerCase().endsWith('-' + suffix.toLowerCase()) ||
                    r.codNode.toLowerCase() === suffix.toLowerCase()
                ) : null;

                if (relation || masterMatch || smartMatch) {
                    const match = relation || masterMatch || smartMatch;
                    updated.dpCodHier = match.codHier;
                    updated.dpCodDiv = match.codDiv;
                    updated.dpCodNode = match.codNode;
                    updated.dpIdLevel = match.idLevel;
                    updated.dpDteStart = match.dteStart;
                } else {
                    // Reset if not found
                    updated.dpCodHier = '';
                }
            }

            return updated;
        });
    };

    const handleNext = () => {
        if (currentStepIndex < steps.length - 1) {
            setCurrentStepIndex(prev => prev + 1);
        }
    };

    const handleBack = () => {
        if (currentStepIndex > 0) {
            setCurrentStepIndex(prev => prev - 1);
        }
    };

    const handleSubmit = async () => {
        setIsLoading(true);
        const finalPayload = {
            idAction: parseInt(formData.idAction),
            name: formData.name,
            codDiv: formData.codDiv,
            dteStartSellIn: formData.dteStartSellIn,
            dteEndSellIn: formData.dteEndSellIn,
            dteStartSellOut: formData.dteStartSellOut,
            dteEndSellOut: formData.dteEndSellOut,
            documentKey: formData.documentKey,
            dteToShost: formData.dteToShost,
            levParticipants: parseInt(formData.levParticipants),
            participants: [{
                codParticipant: formData.codParticipant,
                flgInclusion: formData.participantFlgInclusion,
                codHier: formData.participantCodHier || null,
                codDiv: formData.participantCodDiv || null,
                codNode: formData.participantCodNode || null,
                idLevel: parseInt(formData.participantIdLevel) || null,
                dteStart: formData.participantDteStart || null
            }],
            deliveryPoints: [{
                codDeliveryPoint: formData.codDeliveryPoint,
                flgInclusion: formData.dpFlgInclusion,
                codHier: formData.dpCodHier || null,
                codDiv: formData.dpCodDiv || null,
                codNode: formData.dpCodNode || null,
                idLevel: parseInt(formData.dpIdLevel) || null,
                dteStart: formData.dpDteStart || null
            }],
            products: [{
                codProduct: formData.codProduct,
                levProduct: parseInt(formData.levProduct),
                codDisplay: formData.codDisplay,
                codDiv: formData.productCodDiv,
                qtyEstimated: parseFloat(formData.qtyEstimated),
                perceDiscount1: parseFloat(formData.perceDiscount1),
                perceDiscount2: parseFloat(formData.perceDiscount2),
                numMeasure: parseInt(formData.numMeasure),
                codMeasure: formData.codMeasure,
                details: [{
                    codNode: formData.detailCodNode,
                    codDiv: formData.detailCodDiv,
                    flgInclusion: formData.detailFlgInclusion,
                    articles: [{
                        codDiv: formData.artCodDiv,
                        codNode: formData.artCodNode,
                        codNode1: formData.codNode1,
                        codNode2: formData.codNode2,
                        codNodeN: formData.codNodeN
                    }]
                }],
                measureFields: [{
                    fieldName: formData.fieldName,
                    formula: formData.formula
                }]
            }]
        };

        try {
            console.log("Submitting Atomic Transaction:", finalPayload);
            await promotionService.createAtomicPromotion(finalPayload);
            alert("Promotion Created Successfully!");
            setShowForm(false);
            fetchHistory(); // Refresh the list
        } catch (error) {
            console.error("Error creating promotion:", error);
            const errorMsg = error.response?.data?.details?.join('\n') || error.message;
            alert("Failed to create promotion:\n" + errorMsg);
        } finally {
            setIsLoading(false);
        }
    };

    const renderTableHeaders = (dataType) => {
        const headerMap = {
            'Promo Action': ['ID', 'Name', 'Code Div', 'Start Sell In', 'End Sell In', 'Document Key', 'Level Participants'],
            'Participants': ['ID', 'Cod Participant', 'Code Hier', 'Code Div', 'Code Node', 'ID Level', 'Flag Inclusion'],
            'Delivery Points': ['ID', 'Cod Delivery Point', 'Code Hier', 'Code Div', 'Code Node', 'ID Level', 'Flag Inclusion'],
            'Products': ['ID', 'Cod Product', 'Cod Display', 'Code Div', 'Qty Estimated', 'Discount 1', 'Discount 2'],
            'Details': ['ID', 'Code Node', 'Code Div', 'Flag Inclusion'],
            'Articles': ['ID', 'Code Div', 'Code Node', 'Code Node 1', 'Code Node 2', 'Code Node N'],
            'Measure Fields': ['Code Div', 'Code Measure', 'Field Name', 'Formula']
        };
        return headerMap[dataType] || [];
    };

    const filteredViewData = viewData.filter(row => {
        if (searchCriterion === 'Select an option' || !searchTerm) return true;

        const term = searchTerm.toLowerCase();
        let val = '';

        switch (selectedViewType) {
            case 'Promo Action':
                switch (searchCriterion) {
                    case 'ID': val = row.idAction || row.IdAction; break;
                    case 'Name': val = row.name || row.Name; break;
                    case 'Code Div': val = row.codDiv || row.CodDiv; break;
                    case 'Document Key': val = row.documentKey || row.DocumentKey; break;
                    default: break;
                }
                break;
            case 'Participants':
                switch (searchCriterion) {
                    case 'Cod Participant': val = row.codParticipant || row.CodParticipant; break;
                    case 'Code Hier': val = row.codHier || row.CodHier; break;
                    case 'Code Div': val = row.codDiv || row.CodDiv; break;
                    case 'Code Node': val = row.codNode || row.CodNode; break;
                    default: break;
                }
                break;
            case 'Delivery Points':
                switch (searchCriterion) {
                    case 'Cod Delivery Point': val = row.codDeliveryPoint || row.CodDeliveryPoint; break;
                    case 'Code Hier': val = row.codHier || row.CodHier; break;
                    case 'Code Div': val = row.codDiv || row.CodDiv; break;
                    case 'Code Node': val = row.codNode || row.CodNode; break;
                    default: break;
                }
                break;
            case 'Products':
                switch (searchCriterion) {
                    case 'Cod Product': val = row.codProduct || row.CodProduct; break;
                    case 'Cod Display': val = row.codDisplay || row.CodDisplay; break;
                    case 'Code Div': val = row.codDiv || row.CodDiv; break;
                    default: break;
                }
                break;
            case 'Details':
                switch (searchCriterion) {
                    case 'Code Node': val = row.codNode || row.CodNode; break;
                    case 'Code Div': val = row.codDiv || row.CodDiv; break;
                    default: break;
                }
                break;
            case 'Articles':
                switch (searchCriterion) {
                    case 'Code Div': val = row.codDiv || row.CodDiv; break;
                    case 'Code Node': val = row.codNode || row.CodNode; break;
                    default: break;
                }
                break;
            case 'Measure Fields':
                switch (searchCriterion) {
                    case 'Code Div': val = row.codDiv || row.CodDiv; break;
                    case 'Code Measure': val = row.codMeasure || row.CodMeasure; break;
                    case 'Field Name': val = row.fieldName || row.FieldName; break;
                    default: break;
                }
                break;
            default:
                break;
        }

        return (val || '').toString().toLowerCase().includes(term);
    });

    // Pagination Logic for Support Form View
    const viewIndexOfLastItem = viewCurrentPage * viewItemsPerPage;
    const viewIndexOfFirstItem = viewIndexOfLastItem - viewItemsPerPage;
    const currentViewItems = filteredViewData.slice(viewIndexOfFirstItem, viewIndexOfLastItem);
    const viewTotalPages = Math.ceil(filteredViewData.length / viewItemsPerPage);

    const handleViewPageChange = (pageNumber) => {
        setViewCurrentPage(pageNumber);
    };

    const handleViewItemsPerPageChange = (e) => {
        setViewItemsPerPage(Number(e.target.value));
        setViewCurrentPage(1);
    };

    // Reset view pagination on search or type change
    useEffect(() => {
        setViewCurrentPage(1);
    }, [searchTerm, searchCriterion, selectedViewType]);

    const renderTableRows = (dataType) => {
        if (isViewLoading) {
            return <tr><td colSpan="10" style={{ textAlign: 'center', padding: '40px' }}>Loading data...</td></tr>;
        }

        if (viewError) {
            return <tr><td colSpan="10" style={{ textAlign: 'center', padding: '40px', color: '#ef4444' }}>Error: {viewError}</td></tr>;
        }

        if (filteredViewData.length === 0) {
            return <tr><td colSpan="10" style={{ textAlign: 'center', padding: '40px' }}>No records found matching your search.</td></tr>;
        }

        return currentViewItems.map((row, idx) => {
            switch (dataType) {
                case 'Promo Action':
                    return (
                        <tr key={idx}>
                            <td>{row.idAction || row.IdAction}</td>
                            <td>{row.name || row.Name}</td>
                            <td>{row.codDiv || row.CodDiv}</td>
                            <td>{row.dteStartSellIn || row.DteStartSellIn ? new Date(row.dteStartSellIn || row.DteStartSellIn).toLocaleDateString() : '-'}</td>
                            <td>{row.dteEndSellIn || row.DteEndSellIn ? new Date(row.dteEndSellIn || row.DteEndSellIn).toLocaleDateString() : '-'}</td>
                            <td>{row.documentKey || row.DocumentKey}</td>
                            <td>{row.levParticipants ?? row.LevParticipants}</td>
                        </tr>
                    );
                case 'Participants':
                    return (
                        <tr key={idx}>
                            <td>{row.idAction || row.IdAction}</td>
                            <td>{row.codParticipant || row.CodParticipant}</td>
                            <td>{row.codHier || row.CodHier}</td>
                            <td>{row.codDiv || row.CodDiv}</td>
                            <td>{row.codNode || row.CodNode}</td>
                            <td>{row.idLevel ?? row.IdLevel}</td>
                            <td>{row.flgInclusion ?? row.FlgInclusion ? 'Yes' : 'No'}</td>
                        </tr>
                    );
                case 'Delivery Points':
                    return (
                        <tr key={idx}>
                            <td>{row.idAction || row.IdAction}</td>
                            <td>{row.codDeliveryPoint || row.CodDeliveryPoint}</td>
                            <td>{row.codHier || row.CodHier}</td>
                            <td>{row.codDiv || row.CodDiv}</td>
                            <td>{row.codNode || row.CodNode}</td>
                            <td>{row.idLevel ?? row.IdLevel}</td>
                            <td>{row.flgInclusion ?? row.FlgInclusion ? 'Yes' : 'No'}</td>
                        </tr>
                    );
                case 'Products':
                    return (
                        <tr key={idx}>
                            <td>{row.idAction || row.IdAction}</td>
                            <td>{row.codProduct || row.CodProduct}</td>
                            <td>{row.codDisplay || row.CodDisplay}</td>
                            <td>{row.codDiv || row.CodDiv}</td>
                            <td>{row.qtyEstimated ?? row.QtyEstimated}</td>
                            <td>{row.perceDiscount1 ?? row.PerceDiscount1}%</td>
                            <td>{row.perceDiscount2 ?? row.PerceDiscount2}%</td>
                        </tr>
                    );
                case 'Details':
                    return (
                        <tr key={idx}>
                            <td>{row.idAction || row.IdAction}</td>
                            <td>{row.codNode || row.CodNode}</td>
                            <td>{row.codDiv || row.CodDiv}</td>
                            <td>{row.flgInclusion ?? row.FlgInclusion ? 'Yes' : 'No'}</td>
                        </tr>
                    );
                case 'Articles':
                    return (
                        <tr key={idx}>
                            <td>{row.idAction || row.IdAction}</td>
                            <td>{row.codDiv || row.CodDiv}</td>
                            <td>{row.codNode || row.CodNode}</td>
                            <td>{row.codNode1 || row.CodNode1}</td>
                            <td>{row.codNode2 || row.CodNode2}</td>
                            <td>{row.codNodeN || row.CodNodeN}</td>
                        </tr>
                    );
                case 'Measure Fields':
                    return (
                        <tr key={idx}>
                            <td>{row.codDiv || row.CodDiv}</td>
                            <td>{row.codMeasure || row.CodMeasure}</td>
                            <td>{row.fieldName || row.FieldName}</td>
                            <td>{row.formula || row.Formula}</td>
                        </tr>
                    );
                default:
                    return null;
            }
        });
    };

    const renderStepContent = () => {
        const step = currentStep;
        switch (step) {
            case 'Promo Action':
                return (
                    <div className="form-section fade-in">
                        <div className="form-row">
                            <div className="form-col">
                                <label className="form-label">Id Action <span className="required">*</span></label>
                                <input type="number" name="idAction" className="form-input" value={formData.idAction} onChange={handleInputChange} placeholder="Enter action ID" />
                            </div>
                            <div className="form-col">
                                <label className="form-label">Name <span className="required">*</span></label>
                                <input type="text" name="name" className="form-input" value={formData.name} onChange={handleInputChange} placeholder="Enter name" />
                            </div>
                        </div>
                        <div className="form-row">
                            <div className="form-col"><label className="form-label">Code Div <span className="required">*</span></label><input type="text" name="codDiv" className="form-input" value={formData.codDiv} onChange={handleInputChange} placeholder="Enter code div" /></div>
                            <div className="form-col"><label className="form-label">Date To Host</label><input type="datetime-local" name="dteToShost" className="form-input" value={formData.dteToShost} onChange={handleInputChange} /></div>
                        </div>
                        <div className="form-row">
                            <div className="form-col"><label className="form-label">Start Sell In</label><input type="datetime-local" name="dteStartSellIn" className="form-input" value={formData.dteStartSellIn} onChange={handleInputChange} /></div>
                            <div className="form-col"><label className="form-label">End Sell In</label><input type="datetime-local" name="dteEndSellIn" className="form-input" value={formData.dteEndSellIn} onChange={handleInputChange} /></div>
                        </div>
                        <div className="form-row">
                            <div className="form-col"><label className="form-label">Start Sell Out</label><input type="datetime-local" name="dteStartSellOut" className="form-input" value={formData.dteStartSellOut} onChange={handleInputChange} /></div>
                            <div className="form-col"><label className="form-label">End Sell Out</label><input type="datetime-local" name="dteEndSellOut" className="form-input" value={formData.dteEndSellOut} onChange={handleInputChange} /></div>
                        </div>
                        <div className="form-row">
                            <div className="form-col"><label className="form-label">Document Key</label><input type="text" name="documentKey" className="form-input" value={formData.documentKey} onChange={handleInputChange} placeholder="Enter document key" /></div>
                            <div className="form-col"><label className="form-label">Level Participants</label><input type="number" name="levParticipants" className="form-input" value={formData.levParticipants} onChange={handleInputChange} placeholder="0" /></div>
                        </div>
                    </div>
                );
            case 'Participants':
                return (
                    <div className="form-section fade-in">
                        <div className="form-row">
                            <div className="form-col">
                                <label className="form-label">Cod Participant <span className="required">*</span></label>
                                <div style={{ fontSize: '11px', color: '#6b7280', marginBottom: '4px' }}>Please use the Customer Node code from your relations.</div>
                                <input
                                    type="text"
                                    name="codParticipant"
                                    className="form-input"
                                    value={formData.codParticipant}
                                    onChange={handleInputChange}
                                    placeholder="Enter Participant Code"
                                />
                                {formData.participantCodHier && (
                                    <div style={{ fontSize: '12px', color: '#059669', marginTop: '6px', fontWeight: '500' }}>
                                        ✓ Resolved: {formData.participantCodHier} | {formData.participantCodNode} | Level {formData.participantIdLevel}
                                    </div>
                                )}
                                {formData.codParticipant && !formData.participantCodHier && (
                                    <div style={{ fontSize: '12px', color: '#d97706', marginTop: '6px', fontWeight: '500' }}>
                                        ⚠ Unresolved: Backend will attempt to resolve this code.
                                    </div>
                                )}
                            </div>
                            <div className="form-col" style={{ display: 'flex', alignItems: 'center', gap: '12px', marginTop: '28px' }}>
                                <input
                                    type="checkbox"
                                    name="participantFlgInclusion"
                                    id="flgInclusion"
                                    checked={formData.participantFlgInclusion}
                                    onChange={handleInputChange}
                                    style={{ width: '20px', height: '20px', accentColor: '#9333ea' }}
                                />
                                <label htmlFor="flgInclusion" style={{ fontSize: '14px', fontWeight: '500', color: '#374151', cursor: 'pointer', margin: 0 }}>Flag Inclusion</label>
                            </div>
                        </div>
                    </div>
                );
            case 'Delivery Points':
                return (
                    <div className="form-section fade-in">
                        <div className="form-row">
                            <div className="form-col">
                                <label className="form-label">Cod Delivery Point <span className="required">*</span></label>
                                <div style={{ fontSize: '11px', color: '#6b7280', marginBottom: '4px' }}>Please use the Customer Node code from your relations.</div>
                                <input
                                    type="text"
                                    name="codDeliveryPoint"
                                    className="form-input"
                                    value={formData.codDeliveryPoint}
                                    onChange={handleInputChange}
                                    placeholder="Enter Delivery Point Code"
                                />
                                {formData.dpCodHier && (
                                    <div style={{ fontSize: '12px', color: '#059669', marginTop: '6px', fontWeight: '500' }}>
                                        ✓ Resolved: {formData.dpCodHier} | {formData.dpCodNode} | Level {formData.dpIdLevel}
                                    </div>
                                )}
                                {formData.codDeliveryPoint && !formData.dpCodHier && (
                                    <div style={{ fontSize: '12px', color: '#d97706', marginTop: '6px', fontWeight: '500' }}>
                                        ⚠ Unresolved: Backend will attempt to resolve this code.
                                    </div>
                                )}
                            </div>
                            <div className="form-col" style={{ display: 'flex', alignItems: 'center', gap: '12px', marginTop: '28px' }}>
                                <input
                                    type="checkbox"
                                    name="dpFlgInclusion"
                                    id="dpFlgInclusion"
                                    checked={formData.dpFlgInclusion}
                                    onChange={handleInputChange}
                                    style={{ width: '20px', height: '20px', accentColor: '#9333ea' }}
                                />
                                <label htmlFor="dpFlgInclusion" style={{ fontSize: '14px', fontWeight: '500', color: '#374151', cursor: 'pointer', margin: 0 }}>Flag Inclusion</label>
                            </div>
                        </div>
                    </div>
                );
            case 'Products':
                return (
                    <div className="form-section fade-in">
                        <div className="form-row">
                            <div className="form-col"><label className="form-label">Cod Product <span className="required">*</span></label><input type="text" name="codProduct" className="form-input" value={formData.codProduct} onChange={handleInputChange} placeholder="Enter product code" /></div>
                            <div className="form-col"><label className="form-label">Cod Display</label><input type="text" name="codDisplay" className="form-input" value={formData.codDisplay} onChange={handleInputChange} placeholder="Enter display code" /></div>
                        </div>
                        <div className="form-row">
                            <div className="form-col"><label className="form-label">Code Div</label><input type="text" name="productCodDiv" className="form-input" value={formData.productCodDiv} onChange={handleInputChange} placeholder="Enter division code" /></div>
                            <div className="form-col"><label className="form-label">Code Measure</label><input type="text" name="codMeasure" className="form-input" value={formData.codMeasure} onChange={handleInputChange} placeholder="Enter measure code" /></div>
                        </div>
                        <div className="form-row">
                            <div className="form-col"><label className="form-label">Quantity Estimated</label><input type="number" name="qtyEstimated" className="form-input" value={formData.qtyEstimated} onChange={handleInputChange} placeholder="0" /></div>
                            <div className="form-col"><label className="form-label">Level Product</label><input type="number" name="levProduct" className="form-input" value={formData.levProduct} onChange={handleInputChange} placeholder="0" /></div>
                        </div>
                        <div className="form-row">
                            <div className="form-col"><label className="form-label">Perce Discount 1</label><input type="number" name="perceDiscount1" className="form-input" value={formData.perceDiscount1} onChange={handleInputChange} placeholder="0" /></div>
                            <div className="form-col"><label className="form-label">Perce Discount 2</label><input type="number" name="perceDiscount2" className="form-input" value={formData.perceDiscount2} onChange={handleInputChange} placeholder="0" /></div>
                        </div>
                    </div>
                );
            case 'Details':
                return (
                    <div className="form-section fade-in">
                        <div className="form-row">
                            <div className="form-col"><label className="form-label">Code Node</label><input type="text" name="detailCodNode" className="form-input" value={formData.detailCodNode} onChange={handleInputChange} placeholder="Enter node code" /></div>
                            <div className="form-col"><label className="form-label">Code Div</label><input type="text" name="detailCodDiv" className="form-input" value={formData.detailCodDiv} onChange={handleInputChange} placeholder="Enter division code" /></div>
                        </div>
                        <div className="form-row">
                            <div className="form-col" style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                                <input type="checkbox" name="detailFlgInclusion" id="detailFlgInclusion" checked={formData.detailFlgInclusion} onChange={handleInputChange} style={{ width: '20px', height: '20px', accentColor: '#9333ea' }} />
                                <label htmlFor="detailFlgInclusion" style={{ fontSize: '14px', fontWeight: '500', color: '#374151', cursor: 'pointer', margin: 0 }}>Flag Inclusion</label>
                            </div>
                        </div>
                    </div>
                );
            case 'Articles':
                return (
                    <div className="form-section fade-in">
                        <div className="form-row">
                            <div className="form-col"><label className="form-label">Code Div</label><input type="text" name="artCodDiv" className="form-input" value={formData.artCodDiv} onChange={handleInputChange} placeholder="Enter division code" /></div>
                            <div className="form-col"><label className="form-label">Code Node</label><input type="text" name="artCodNode" className="form-input" value={formData.artCodNode} onChange={handleInputChange} placeholder="Enter node code" /></div>
                        </div>
                        <div className="form-row">
                            <div className="form-col"><label className="form-label">Code Node 1</label><input type="text" name="codNode1" className="form-input" value={formData.codNode1} onChange={handleInputChange} placeholder="Enter node 1 code" /></div>
                            <div className="form-col"><label className="form-label">Code Node 2</label><input type="text" name="codNode2" className="form-input" value={formData.codNode2} onChange={handleInputChange} placeholder="Enter node 2 code" /></div>
                        </div>
                        <div className="form-row">
                            <div className="form-col"><label className="form-label">Code Node N</label><input type="text" name="codNodeN" className="form-input" value={formData.codNodeN} onChange={handleInputChange} placeholder="Enter node N code" /></div>
                        </div>
                    </div>
                );
            case 'Measure Fields':
                return (
                    <div className="form-section fade-in">
                        <div className="form-row">
                            <div className="form-col"><label className="form-label">Field Name</label><input type="text" name="fieldName" className="form-input" value={formData.fieldName} onChange={handleInputChange} placeholder="Enter field name" /></div>
                        </div>
                        <div className="form-row">
                            <div className="form-col"><label className="form-label">Formula</label><textarea name="formula" className="form-input" style={{ height: '128px', paddingTop: '12px', paddingBottom: '12px', resize: 'vertical' }} value={formData.formula} onChange={handleInputChange} placeholder="Enter formula"></textarea></div>
                        </div>
                    </div>
                );
            default:
                return null;
        }
    };

    return (
        <div className="promotions-container">
            <div className="promo-page-header">
                <h1 className="promo-page-title">Promotions</h1>
                {!showForm && (
                    <div style={{ display: 'flex', gap: '12px' }}>
                        <button className="view-btn" onClick={() => setShowViewModal(true)}>
                            <Eye size={18} />
                            View
                        </button>
                        <button className="view-btn" style={{ marginLeft: '12px' }} onClick={() => setShowDetailView(true)}>
                            <Search size={18} />
                            Detail View
                        </button>
                        {canEdit && (
                            <button className="create-btn" onClick={() => setShowForm(true)}>
                                <Plus size={18} />
                                Create Promotion
                            </button>
                        )}
                    </div>
                )}
            </div>

            {!showForm ? (
                <div className="promo-dashboard fade-in">
                    <h3 className="dashboard-title">Recent Promotion Activities</h3>
                    <div className="dashboard-table-wrapper">
                        <table className="dashboard-table">
                            <thead>
                                <tr>
                                    <th>Promotion ID</th>
                                    <th>Name</th>
                                    <th>Initiator</th>
                                    <th>Start Date</th>
                                    <th>End Date</th>
                                    <th>Status</th>
                                    <th>Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                                {isLoading ? (
                                    <tr><td colSpan="7" style={{ textAlign: 'center', padding: '40px' }}>Loading Activities...</td></tr>
                                ) : currentActivities.length > 0 ? (
                                    currentActivities.map((activity, idx) => {
                                        const startDate = activity.dteStartSellIn || activity.DteStartSellIn;
                                        const endDate = activity.dteEndSellIn || activity.DteEndSellIn;

                                        let status = 'Unknown';
                                        let statusClass = 'pending';

                                        if (startDate && endDate) {
                                            const now = new Date();
                                            const start = new Date(startDate);
                                            const end = new Date(endDate);

                                            if (now < start) {
                                                status = 'Upcoming';
                                                statusClass = 'pending'; // or a new 'upcoming' class
                                            } else if (now >= start && now <= end) {
                                                status = 'Active';
                                                statusClass = 'completed'; // Reusing green style for Active
                                            } else {
                                                status = 'Expired';
                                                statusClass = 'expired'; // Need to add style for this
                                            }
                                        }

                                        return (
                                            <tr key={idx}>
                                                <td className="id-cell">{activity.idAction || activity.IdAction || activity.codAction || activity.CodAction}</td>
                                                <td>{activity.name || activity.Name || activity.actionType || 'Atomic Action'}</td>
                                                <td>{activity.createdBy || activity.CreatedBy || 'System'}</td>
                                                <td>{startDate ? new Date(startDate).toLocaleDateString() : 'N/A'}</td>
                                                <td>{endDate ? new Date(endDate).toLocaleDateString() : 'N/A'}</td>
                                                <td>
                                                    <span className={`status-badge ${statusClass}`}>
                                                        {status}
                                                    </span>
                                                </td>
                                                <td style={{ textAlign: 'right', position: 'relative' }}>
                                                    <button
                                                        className="action-menu-btn"
                                                        onClick={(e) => {
                                                            e.stopPropagation();
                                                            setActiveActionMenu(activeActionMenu === idx ? null : idx);
                                                        }}
                                                    >
                                                        <MoreVertical size={18} />
                                                    </button>

                                                    {activeActionMenu === idx && (
                                                        <div className="action-dropdown-menu fade-in">
                                                            {canEdit ? (
                                                                <>
                                                                    <button
                                                                        className="action-item edit"
                                                                        onClick={() => handleEditPromo(activity)}
                                                                    >
                                                                        <Edit2 size={14} /> Edit
                                                                    </button>
                                                                    <button
                                                                        className="action-item delete"
                                                                        onClick={() => handleDelete(activity.idAction ?? activity.IdAction ?? activity.codAction ?? activity.CodAction)}
                                                                    >
                                                                        <Trash2 size={14} /> Delete
                                                                    </button>
                                                                </>
                                                            ) : (
                                                                <div className="action-item disabled" style={{ fontSize: '12px', color: '#94a3b8', cursor: 'not-allowed', padding: '8px 12px' }}>
                                                                    Admin only
                                                                </div>
                                                            )}
                                                        </div>
                                                    )}
                                                </td>
                                            </tr>
                                        );
                                    })
                                ) : (
                                    <tr><td colSpan="7" style={{ textAlign: 'center', padding: '40px' }}>No recent promotion activities found.</td></tr>
                                )}
                            </tbody>
                        </table>
                    </div>

                    {/* Pagination UI */}
                    <div className="pagination-container">
                        <div className="pagination-info">
                            Showing {currentActivities.length > 0 ? indexOfFirstItem + 1 : 0} to {Math.min(indexOfLastItem, activities.length)} of {activities.length} results
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
            ) : (
                <div className="promo-card">
                    <div className="promo-header">
                        <div style={{ display: 'flex', alignItems: 'center', gap: '16px' }}>
                            <div className="icon-wrapper">
                                <MessageSquare size={22} />
                            </div>
                            <div style={{ display: 'flex', flexDirection: 'column' }}>
                                <h2 className="promo-title">Create Atomic Promotion</h2>
                                <span style={{ fontSize: '13px', color: '#6b7280', marginTop: '4px' }}>Step {currentStepIndex + 1} of {steps.length}: {currentStep}</span>
                            </div>
                        </div>
                        <button className="close-btn" onClick={() => setShowForm(false)}>
                            <X size={20} />
                        </button>
                    </div>

                    <div className="promo-body">
                        <div style={{ width: '100%', backgroundColor: '#f3f4f6', borderRadius: '9999px', height: '8px', marginBottom: '32px', overflow: 'hidden' }}>
                            <div
                                style={{
                                    backgroundColor: '#9333ea',
                                    height: '8px',
                                    borderRadius: '9999px',
                                    transition: 'width 0.5s ease-in-out',
                                    width: `${((currentStepIndex + 1) / steps.length) * 100}%`
                                }}
                            ></div>
                        </div>

                        {renderStepContent()}

                        <div className="submit-btn-wrapper">
                            {currentStepIndex > 0 ? (
                                <button className="back-btn" onClick={handleBack}>
                                    <ChevronLeft size={16} /> Back
                                </button>
                            ) : <div></div>}

                            {currentStepIndex < steps.length - 1 ? (
                                <button className="submit-btn" onClick={handleNext}>
                                    Next <ChevronRight size={16} />
                                </button>
                            ) : (
                                <button className="submit-btn" style={{ backgroundColor: '#10b981' }} onClick={handleSubmit}>
                                    Submit Transaction <Check size={16} />
                                </button>
                            )}
                        </div>
                    </div>
                </div>
            )}

            {/* View Page */}
            {showViewModal && (
                <div className="view-page-overlay">
                    <div className="view-page-container">
                        <div className="view-page-header">
                            <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                                <div className="icon-wrapper">
                                    <MessageSquare size={20} />
                                </div>
                                <h2 className="view-page-title">Support Form</h2>
                            </div>
                            <button className="close-btn" onClick={() => setShowViewModal(false)}>
                                <X size={20} />
                            </button>
                        </div>

                        <div className="view-page-content">
                            <label className="search-label">Request Type <span className="required">*</span></label>

                            {/* Custom Dropdown */}
                            <div className="custom-dropdown-container">
                                <div
                                    className={`custom-dropdown-header ${isDropdownOpen ? 'active' : ''}`}
                                    onClick={() => setIsDropdownOpen(!isDropdownOpen)}
                                >
                                    <span>{selectedViewType || 'Select Request Type'}</span>
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
                                        {steps.map(step => (
                                            <div
                                                key={step}
                                                className={`custom-dropdown-item ${selectedViewType === step ? 'selected' : ''}`}
                                                onClick={() => {
                                                    setSelectedViewType(step);
                                                    setIsDropdownOpen(false);
                                                }}
                                            >
                                                {step}
                                            </div>
                                        ))}
                                    </div>
                                )}
                            </div>

                            {/* Search Section - Hidden when main dropdown is open to prevent overlap */}
                            {selectedViewType && !isDropdownOpen && (
                                <div className="search-by-section fade-in">
                                    <h3 className="search-by-title">Search By</h3>
                                    <div className="search-controls">
                                        <div className="custom-dropdown-container">
                                            <div
                                                className={`custom-dropdown-header ${isSearchDropdownOpen ? 'active' : ''}`}
                                                onClick={() => setIsSearchDropdownOpen(!isSearchDropdownOpen)}
                                            >
                                                <span>{searchCriterion}</span>
                                                <ChevronRight
                                                    size={18}
                                                    style={{
                                                        transform: isSearchDropdownOpen ? 'rotate(90deg)' : 'rotate(0deg)',
                                                        transition: 'transform 0.2s ease',
                                                        color: '#9333ea'
                                                    }}
                                                />
                                            </div>
                                            {isSearchDropdownOpen && (
                                                <div className="custom-dropdown-list">
                                                    <div
                                                        className={`custom-dropdown-item ${searchCriterion === 'Select an option' ? 'selected' : ''}`}
                                                        onClick={() => {
                                                            setSearchCriterion('Select an option');
                                                            setIsSearchDropdownOpen(false);
                                                        }}
                                                    >
                                                        Select an option
                                                    </div>
                                                    {renderTableHeaders(selectedViewType).map(h => (
                                                        <div
                                                            key={h}
                                                            className={`custom-dropdown-item ${searchCriterion === h ? 'selected' : ''}`}
                                                            onClick={() => {
                                                                setSearchCriterion(h);
                                                                setIsSearchDropdownOpen(false);
                                                            }}
                                                        >
                                                            {h}
                                                        </div>
                                                    ))}
                                                </div>
                                            )}
                                        </div>
                                        <input
                                            type="text"
                                            className="search-term-input"
                                            placeholder="Search Term"
                                            value={searchTerm}
                                            onChange={(e) => setSearchTerm(e.target.value)}
                                        />
                                        <button className="search-action-btn">
                                            <Eye size={18} />
                                            Search
                                        </button>
                                    </div>

                                    {/* Data Table */}
                                    <div className="view-table-wrapper">
                                        <table className="view-data-table">
                                            <thead>
                                                <tr>
                                                    {renderTableHeaders(selectedViewType).map((header, idx) => (
                                                        <th key={idx}>{header}</th>
                                                    ))}
                                                </tr>
                                            </thead>
                                            <tbody>
                                                {renderTableRows(selectedViewType)}
                                            </tbody>
                                        </table>
                                    </div>

                                    {/* Pagination UI for View */}
                                    <div className="pagination-container">
                                        <div className="pagination-info">
                                            Showing {currentViewItems.length > 0 ? viewIndexOfFirstItem + 1 : 0} to {Math.min(viewIndexOfLastItem, filteredViewData.length)} of {filteredViewData.length} results
                                        </div>
                                        <div className="pagination-controls">
                                            <div className="per-page-wrapper">
                                                <select className="per-page-select" value={viewItemsPerPage} onChange={handleViewItemsPerPageChange}>
                                                    {[5, 10, 20, 50].map(val => (
                                                        <option key={val} value={val}>{val} per page</option>
                                                    ))}
                                                </select>
                                            </div>
                                            <div className="pagination-buttons">
                                                <button
                                                    className="page-btn nav-btn"
                                                    onClick={() => handleViewPageChange(viewCurrentPage - 1)}
                                                    disabled={viewCurrentPage === 1}
                                                >
                                                    Previous
                                                </button>
                                                {[...Array(viewTotalPages)].map((_, i) => (
                                                    <button
                                                        key={i + 1}
                                                        className={`page-btn ${viewCurrentPage === i + 1 ? 'active' : ''}`}
                                                        onClick={() => handleViewPageChange(i + 1)}
                                                    >
                                                        {i + 1}
                                                    </button>
                                                ))}
                                                <button
                                                    className="page-btn nav-btn"
                                                    onClick={() => handleViewPageChange(viewCurrentPage + 1)}
                                                    disabled={viewCurrentPage === viewTotalPages || viewTotalPages === 0}
                                                >
                                                    Next
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            )}
                        </div>
                    </div>
                </div>
            )}

            {/* Edit Promotion Modal */}
            {showEditPromoModal && (
                <div className="modal-overlay fade-in">
                    <div className="modal-container active">
                        <div className="modal-header">
                            <h2 className="modal-title">Edit Promotion</h2>
                            <button className="modal-close" onClick={() => setShowEditPromoModal(false)}>
                                <X size={20} />
                            </button>
                        </div>
                        <div className="modal-body">
                            <div className="form-section">
                                <div className="form-grid">
                                    <div className="form-group">
                                        <label className="form-label">Name <span className="required">*</span></label>
                                        <input
                                            type="text"
                                            className="form-input"
                                            value={editPromoFormData.name}
                                            onChange={(e) => setEditPromoFormData({ ...editPromoFormData, name: e.target.value })}
                                            placeholder="Enter name"
                                        />
                                    </div>
                                    <div className="form-group">
                                        <label className="form-label">Document Key</label>
                                        <input
                                            type="text"
                                            className="form-input"
                                            value={editPromoFormData.documentKey}
                                            onChange={(e) => setEditPromoFormData({ ...editPromoFormData, documentKey: e.target.value })}
                                            placeholder="Enter document key"
                                        />
                                    </div>
                                    <div className="form-group">
                                        <label className="form-label">Start Sell In</label>
                                        <input
                                            type="date"
                                            className="form-input"
                                            value={editPromoFormData.dteStartSellIn}
                                            onChange={(e) => setEditPromoFormData({ ...editPromoFormData, dteStartSellIn: e.target.value })}
                                        />
                                    </div>
                                    <div className="form-group">
                                        <label className="form-label">End Sell In</label>
                                        <input
                                            type="date"
                                            className="form-input"
                                            value={editPromoFormData.dteEndSellIn}
                                            onChange={(e) => setEditPromoFormData({ ...editPromoFormData, dteEndSellIn: e.target.value })}
                                        />
                                    </div>
                                    <div className="form-group">
                                        <label className="form-label">Start Sell Out</label>
                                        <input
                                            type="date"
                                            className="form-input"
                                            value={editPromoFormData.dteStartSellOut}
                                            onChange={(e) => setEditPromoFormData({ ...editPromoFormData, dteStartSellOut: e.target.value })}
                                        />
                                    </div>
                                    <div className="form-group">
                                        <label className="form-label">End Sell Out</label>
                                        <input
                                            type="date"
                                            className="form-input"
                                            value={editPromoFormData.dteEndSellOut}
                                            onChange={(e) => setEditPromoFormData({ ...editPromoFormData, dteEndSellOut: e.target.value })}
                                        />
                                    </div>
                                    <div className="form-group">
                                        <label className="form-label">Date To Host</label>
                                        <input
                                            type="date"
                                            className="form-input"
                                            value={editPromoFormData.dteToShost}
                                            onChange={(e) => setEditPromoFormData({ ...editPromoFormData, dteToShost: e.target.value })}
                                        />
                                    </div>
                                    <div className="form-group">
                                        <label className="form-label">Level Participants</label>
                                        <input
                                            type="number"
                                            className="form-input"
                                            value={editPromoFormData.levParticipants}
                                            onChange={(e) => setEditPromoFormData({ ...editPromoFormData, levParticipants: e.target.value })}
                                        />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div className="modal-footer">
                            <button className="btn-secondary" onClick={() => setShowEditPromoModal(false)}>Cancel</button>
                            <button className="btn-primary" onClick={handleUpdatePromo}>Save Changes</button>
                        </div>
                    </div>
                </div>
            )}

            {/* Detail View Modal */}
            {showDetailView && (
                <div className="view-page-overlay">
                    <div className="view-page-container" style={{ maxWidth: '1400px', padding: '0 20px 40px' }}>
                        <div className="view-page-content" style={{ padding: 0, background: 'transparent' }}>
                            <PromotionDetailView onClose={() => setShowDetailView(false)} />
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default Promotions;
