import React, { useState, useEffect } from 'react';
import { Package, Plus, Check, X, Eye, ChevronRight, ShoppingBag, Trash2 } from 'lucide-react';
import './Products.css';
import { promotionService } from '../services/promotionService';

const Products = () => {
    const [showForm, setShowForm] = useState(false);

    // Form Data - Removed single product codes, added common fields
    const [formData, setFormData] = useState({
        codProduct: '',
        codDisplay: '',
        codDiv: '',
        qtyEstimated: 0,
        perceDiscount1: 0,
        perceDiscount2: 0,
        numMeasure: 0,
        codMeasure: '',
        promotion: null,
        isNewProduct: false,
        // Detail Fields
        detCodNode: '',
        detCodDiv: '',
        detFlgInclusion: false,
        // Article Fields
        artCodNode1: '',
        artCodNode2: '',
        artCodNodeN: '',
        // Measure Fields
        measFieldName: '',
        measFormula: ''
    });

    const [selectedProducts, setSelectedProducts] = useState([]); // List of selected products for the form

    // Main View State
    const [searchCriterion, setSearchCriterion] = useState('Select an option');
    const [isDropdownOpen, setIsDropdownOpen] = useState(false);
    const [searchTerm, setSearchTerm] = useState('');
    const [products, setProducts] = useState([]); // Main view products list
    const [isLoading, setIsLoading] = useState(false);
    const [fetchError, setFetchError] = useState(null);

    // Promotion Modal State
    const [promotions, setPromotions] = useState([]);
    const [showPromotionModal, setShowPromotionModal] = useState(false);
    const [promotionSearchTerm, setPromotionSearchTerm] = useState('');

    // Product Selection Modal State
    const [showProductModal, setShowProductModal] = useState(false);
    const [productSearchTerm, setProductSearchTerm] = useState('');

    // We reuse 'products' as the source for the modal if available, 
    // or fetch if empty (though logic below assumes 'products' is populated on load)

    useEffect(() => {
        fetchProducts();
    }, []);

    // Fetch promotions when modal opens
    useEffect(() => {
        if (showPromotionModal && promotions.length === 0) {
            fetchPromotions();
        }
    }, [showPromotionModal]);

    const fetchProducts = async () => {
        setIsLoading(true);
        setFetchError(null);
        try {
            const data = await promotionService.getProducts();
            setProducts(data || []);
        } catch (error) {
            console.error("Error fetching products:", error);
            setFetchError(error.message || "Failed to fetch data");
        } finally {
            setIsLoading(false);
        }
    };

    const fetchPromotions = async () => {
        try {
            const data = await promotionService.getPromoActions();
            setPromotions(data || []);
        } catch (error) {
            console.error("Error fetching promotions:", error);
        }
    };

    // Filter Logic for Main View
    const filteredData = products.filter(product => {
        if (searchCriterion === 'Select an option' || !searchTerm) return true;
        const term = searchTerm.toLowerCase();
        switch (searchCriterion) {
            case 'Code Product': return (product.codProduct || product.CodProduct || '').toString().toLowerCase().includes(term);
            case 'Code Display': return (product.codDisplay || product.CodDisplay || '').toString().toLowerCase().includes(term);
            case 'Code Div': return (product.codDiv || product.CodDiv || '').toString().toLowerCase().includes(term);
            case 'Measure': return (product.codMeasure || product.CodMeasure || '').toString().toLowerCase().includes(term);
            default: return true;
        }
    });

    // Filter Logic for Promotion Modal
    const filteredPromotions = promotions.filter(p => {
        if (!promotionSearchTerm) return true;
        const term = promotionSearchTerm.toLowerCase();
        return (p.name || p.Name || '').toLowerCase().includes(term) ||
            (p.idAction || p.IdAction || '').toString().toLowerCase().includes(term);
    });

    // Filter Logic for Product Modal
    const filteredModalProducts = products.filter(p => {
        if (!productSearchTerm) return true;
        const term = productSearchTerm.toLowerCase();
        return (p.codProduct || p.CodProduct || '').toString().toLowerCase().includes(term) ||
            (p.codDisplay || p.CodDisplay || '').toString().toLowerCase().includes(term);
    });

    const handleInputChange = (e) => {
        const { name, value, type, checked } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: type === 'checkbox' ? checked : value
        }));
    };

    const handlePromotionSelect = (promo) => {
        setFormData(prev => ({
            ...prev,
            promotion: promo
        }));
        setShowPromotionModal(false);
    };

    const toggleProductSelection = (product) => {
        const isSelected = selectedProducts.some(p => (p.codProduct || p.CodProduct) === (product.codProduct || product.CodProduct));
        if (isSelected) {
            setSelectedProducts(prev => prev.filter(p => (p.codProduct || p.CodProduct) !== (product.codProduct || product.CodProduct)));
        } else {
            setSelectedProducts(prev => [...prev, product]);
        }
    };

    const removeSelectedProduct = (indexToRemove) => {
        setSelectedProducts(prev => prev.filter((_, idx) => idx !== indexToRemove));
    };

    const handleSubmit = async () => {
        if (!formData.promotion) {
            alert("Please select a promotion.");
            return;
        }

        if (formData.isNewProduct) {
            // NEW PRODUCT LOGIC
            if (!formData.codProduct) {
                alert("Please enter a Product Code.");
                return;
            }

            try {
                // 1. Create the Product
                const promoId = formData.promotion.idAction || formData.promotion.IdAction;

                const newProductPayload = {
                    codProduct: formData.codProduct,
                    codDisplay: formData.codDisplay,
                    codDiv: formData.codDiv,
                    codMeasure: formData.codMeasure,
                    qtyEstimated: formData.qtyEstimated,
                    perceDiscount1: formData.perceDiscount1,
                    perceDiscount2: formData.perceDiscount2,
                    numMeasure: formData.numMeasure,
                    idAction: promoId,
                    // Atomic Details
                    details: [{
                        codNode: formData.detCodNode,
                        codDiv: formData.detCodDiv,
                        flgInclusion: formData.detFlgInclusion,
                        articles: [{
                            codNode1: formData.artCodNode1,
                            codNode2: formData.artCodNode2,
                            codNodeN: formData.artCodNodeN,
                            codNode: formData.detCodNode,
                            codDiv: formData.detCodDiv
                        }]
                    }],
                    measureFields: [{
                        fieldName: formData.measFieldName,
                        formula: formData.measFormula
                    }]
                };

                // 1. Create and Link the Product in one step
                const response = await promotionService.createProduct(newProductPayload);

                // Update Local State for feedback
                const newProduct = {
                    ...newProductPayload,
                    idAction: promoId,
                    promotionName: formData.promotion.name || formData.promotion.Name
                };

                setProducts(prev => [...prev, newProduct]);
                alert(`Successfully Created & Linked New Product: ${newProduct.codProduct}`);
            } catch (error) {
                console.error("Error creating/linking product:", error);
                const errorMsg = error.response?.data ? JSON.stringify(error.response.data) : error.message;
                alert("Failed to save product: " + errorMsg);
            }

        } else {
            // EXISTING PRODUCT LOGIC
            if (selectedProducts.length === 0) {
                alert("Please select at least one product.");
                return;
            }

            try {
                const promoId = formData.promotion.idAction || formData.promotion.IdAction;

                // Prepare Payload: Inject idAction into every selected product
                const productsToLink = selectedProducts.map(p => ({
                    ...p,
                    idAction: parseInt(promoId) // Ensure ID is an integer
                }));

                await promotionService.addProductsToPromotion(productsToLink);
                await fetchProducts();

                alert(`Successfully linked ${selectedProducts.length} product(s) to Promotion #${promoId}!`);
            } catch (error) {
                console.error("Error linking products:", error);
                const errorMsg = error.response?.data ? JSON.stringify(error.response.data) : error.message;
                alert("Failed to link products: " + errorMsg);
            }
        }

        // Reset form
        setShowForm(false);
        setFormData({
            codDiv: '',
            qtyEstimated: 0,
            perceDiscount1: 0,
            perceDiscount2: 0,
            numMeasure: 0,
            codMeasure: '',
            promotion: null,
            isNewProduct: false,
            detCodNode: '',
            detCodDiv: '',
            detFlgInclusion: false,
            artCodNode1: '',
            artCodNode2: '',
            artCodNodeN: '',
            measFieldName: '',
            measFormula: ''
        });
        setSelectedProducts([]);
    };

    return (
        <div className="products-container">
            {!showForm ? (
                <div className="products-table-container fade-in">
                    <div className="search-by-section">
                        <h3 className="search-by-title">Search By</h3>
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
                                            {['Select an option', 'Code Product', 'Code Display', 'Code Div', 'Measure'].map(opt => (
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

                            <button className="create-btn" onClick={() => setShowForm(true)}>
                                <Plus size={18} />
                                Add Product
                            </button>
                        </div>

                        {/* Main View Data Table */}
                        <div className="view-table-wrapper">
                            <table className="view-data-table">
                                <thead>
                                    <tr>
                                        <th>Promotion ID</th>
                                        <th>Code Product</th>
                                        <th>Code Display</th>
                                        <th>Code Div</th>
                                        <th>Qty Estimated</th>
                                        <th>Discount 1 (%)</th>
                                        <th>Discount 2 (%)</th>
                                        <th>Num Measure</th>
                                        <th>Code Measure</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {isLoading ? (
                                        <tr><td colSpan="8" style={{ textAlign: 'center', padding: '40px' }}>Loading Products...</td></tr>
                                    ) : filteredData.length > 0 ? (
                                        filteredData.map((product, idx) => (
                                            <tr key={idx}>
                                                <td>{product.idAction || product.IdAction || product.codAction || product.CodAction || '-'}</td>
                                                <td>{product.codProduct || product.CodProduct}</td>
                                                <td>{product.codDisplay || product.CodDisplay}</td>
                                                <td>{product.codDiv || product.CodDiv}</td>
                                                <td>{product.qtyEstimated ?? product.QtyEstimated}</td>
                                                <td>{product.perceDiscount1 ?? product.PerceDiscount1}</td>
                                                <td>{product.perceDiscount2 ?? product.PerceDiscount2}</td>
                                                <td>{product.numMeasure ?? product.NumMeasure}</td>
                                                <td>{product.codMeasure || product.CodMeasure}</td>
                                            </tr>
                                        ))
                                    ) : fetchError ? (
                                        <tr><td colSpan="8" style={{ textAlign: 'center', padding: '40px', color: '#ef4444' }}>Error: {fetchError} (Check if Backend is running)</td></tr>
                                    ) : (
                                        <tr><td colSpan="8" style={{ textAlign: 'center', padding: '40px' }}>No products found in the database.</td></tr>
                                    )}
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            ) : (
                <div className="form-card fade-in">
                    <div className="form-header">
                        <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                            <div className="icon-wrapper">
                                <ShoppingBag size={22} />
                            </div>
                            <h2 className="form-title">Add Product</h2>
                        </div>
                        <button className="close-btn" onClick={() => setShowForm(false)}>
                            <X size={20} />
                        </button>
                    </div>

                    <div className="form-body">
                        <div className="form-section">
                            <h3 className="section-title">Product Details</h3>
                            <p className="section-subtitle">Select a promotion and the products to add.</p>

                            {/* Promotion Selection */}
                            <div className="form-row">
                                <div className="form-col">
                                    <label className="form-label">Select Promotion <span className="required">*</span></label>
                                    <div
                                        className="form-input"
                                        style={{ cursor: 'pointer', display: 'flex', alignItems: 'center', justifyContent: 'space-between', backgroundColor: '#f9fafb' }}
                                        onClick={() => setShowPromotionModal(true)}
                                    >
                                        <span style={{ color: formData.promotion ? '#111827' : '#9ca3af' }}>
                                            {formData.promotion ? (formData.promotion.name || formData.promotion.Name) : 'Click to select a promotion'}
                                        </span>
                                        <ChevronRight size={16} color="#9ca3af" />
                                    </div>
                                </div>
                            </div>

                            {/* Mode Selection: Existing vs New */}
                            <div className="form-row" style={{ marginBottom: '24px' }}>
                                <div style={{ display: 'flex', gap: '16px', borderBottom: '1px solid #e5e7eb', width: '100%' }}>
                                    <button
                                        onClick={() => { setSelectedProducts([]); setFormData(prev => ({ ...prev, isNewProduct: false })); }}
                                        style={{
                                            padding: '10px 16px',
                                            color: !formData.isNewProduct ? '#9333ea' : '#6b7280',
                                            fontWeight: !formData.isNewProduct ? '600' : '500',
                                            background: 'none', border: 'none', borderBottom: !formData.isNewProduct ? '2px solid #9333ea' : 'transparent', cursor: 'pointer'
                                        }}
                                    >
                                        Select Existing Products
                                    </button>
                                    <button
                                        onClick={() => { setSelectedProducts([]); setFormData(prev => ({ ...prev, isNewProduct: true })); }}
                                        style={{
                                            padding: '10px 16px',
                                            color: formData.isNewProduct ? '#9333ea' : '#6b7280',
                                            fontWeight: formData.isNewProduct ? '600' : '500',
                                            background: 'none', border: 'none', borderBottom: formData.isNewProduct ? '2px solid #9333ea' : 'transparent', cursor: 'pointer'
                                        }}
                                    >
                                        Create New Product
                                    </button>
                                </div>
                            </div>

                            {!formData.isNewProduct ? (
                                /* EXISTING PRODUCTS FLOW */
                                <div className="form-row" style={{ display: 'block' }}>
                                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '8px' }}>
                                        <label className="form-label" style={{ marginBottom: 0 }}>Selected Products <span className="required">*</span></label>
                                        <button
                                            type="button"
                                            style={{ color: '#9333ea', border: 'none', background: 'none', cursor: 'pointer', fontWeight: '600', fontSize: '15px' }}
                                            onClick={() => setShowProductModal(true)}
                                        >
                                            + Select Products
                                        </button>
                                    </div>
                                    <div style={{ border: '1px solid #d1d5db', borderRadius: '8px', padding: '12px', minHeight: '100px', backgroundColor: '#fff' }}>
                                        {selectedProducts.length > 0 ? (
                                            <div style={{ display: 'flex', flexWrap: 'wrap', gap: '8px' }}>
                                                {selectedProducts.map((p, idx) => (
                                                    <div key={idx} style={{
                                                        background: '#f3f4f6',
                                                        padding: '6px 12px',
                                                        borderRadius: '20px',
                                                        fontSize: '13px',
                                                        color: '#374151',
                                                        display: 'flex',
                                                        alignItems: 'center',
                                                        gap: '6px',
                                                        border: '1px solid #e5e7eb'
                                                    }}>
                                                        {p.codProduct || p.CodProduct} - {p.codDisplay || p.CodDisplay}
                                                        <button
                                                            onClick={() => removeSelectedProduct(idx)}
                                                            style={{ border: 'none', background: 'none', cursor: 'pointer', padding: 0, display: 'flex', color: '#6b7280' }}
                                                        >
                                                            <X size={14} />
                                                        </button>
                                                    </div>
                                                ))}
                                            </div>
                                        ) : (
                                            <div style={{ color: '#9ca3af', fontSize: '14px', fontStyle: 'italic', textAlign: 'center', marginTop: '30px' }}>
                                                No products selected. Click "Select Products" to add items.
                                            </div>
                                        )}
                                    </div>
                                </div>
                            ) : (
                                /* NEW PRODUCT FLOW */
                                <>
                                    <div className="form-row">
                                        <div className="form-col">
                                            <label className="form-label">Code Product <span className="required">*</span></label>
                                            <input type="text" name="codProduct" className="form-input" value={formData.codProduct || ''} onChange={handleInputChange} placeholder="Enter product code" />
                                        </div>
                                        <div className="form-col">
                                            <label className="form-label">Code Display</label>
                                            <input type="text" name="codDisplay" className="form-input" value={formData.codDisplay || ''} onChange={handleInputChange} placeholder="Enter display code" />
                                        </div>
                                    </div>
                                    <div className="form-row">
                                        <div className="form-col">
                                            <label className="form-label">Code Div</label>
                                            <input type="text" name="codDiv" className="form-input" value={formData.codDiv || ''} onChange={handleInputChange} placeholder="Enter division code" />
                                        </div>
                                        <div className="form-col">
                                            <label className="form-label">Code Measure</label>
                                            <input type="text" name="codMeasure" className="form-input" value={formData.codMeasure || ''} onChange={handleInputChange} placeholder="Enter measure code" />
                                        </div>
                                    </div>
                                    <div className="form-row">
                                        <div className="form-col">
                                            <label className="form-label">Estimated Quantity</label>
                                            <input type="number" name="qtyEstimated" className="form-input" value={formData.qtyEstimated || 0} onChange={handleInputChange} placeholder="0" />
                                        </div>
                                        <div className="form-col">
                                            <label className="form-label">Num Measure</label>
                                            <input type="number" name="numMeasure" className="form-input" value={formData.numMeasure || 0} onChange={handleInputChange} placeholder="0" />
                                        </div>
                                    </div>
                                </>
                            )}

                            {formData.isNewProduct && (
                                <>
                                    <hr style={{ border: 'none', borderTop: '1px solid #f3f4f6', margin: '24px 0' }} />
                                    <h4 style={{ fontSize: '14px', fontWeight: '600', color: '#374151', marginBottom: '16px' }}>Common Attributes</h4>

                                    <div className="form-row">
                                        <div className="form-col">
                                            <label className="form-label">Discount 1 (%)</label>
                                            <input
                                                type="number"
                                                name="perceDiscount1"
                                                className="form-input"
                                                value={formData.perceDiscount1 || 0}
                                                onChange={handleInputChange}
                                                placeholder="0"
                                            />
                                        </div>
                                        <div className="form-col">
                                            <label className="form-label">Discount 2 (%)</label>
                                            <input
                                                type="number"
                                                name="perceDiscount2"
                                                className="form-input"
                                                value={formData.perceDiscount2 || 0}
                                                onChange={handleInputChange}
                                                placeholder="0"
                                            />
                                        </div>
                                    </div>

                                    {/* NEW SECTIONS: Details, Articles, Measures */}
                                    <hr style={{ border: 'none', borderTop: '1px solid #f3f4f6', margin: '24px 0' }} />

                                    <div className="form-row">
                                        <div className="form-col">
                                            <h4 style={{ fontSize: '14px', fontWeight: '600', color: '#374151', marginBottom: '16px' }}>Detail Properties</h4>
                                            <div style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}>
                                                <div>
                                                    <label className="form-label">Code Node</label>
                                                    <input type="text" name="detCodNode" className="form-input" value={formData.detCodNode} onChange={handleInputChange} placeholder="Enter node code" />
                                                </div>
                                                <div>
                                                    <label className="form-label">Code Div</label>
                                                    <input type="text" name="detCodDiv" className="form-input" value={formData.detCodDiv} onChange={handleInputChange} placeholder="Enter detail division" />
                                                </div>
                                                <div style={{ display: 'flex', alignItems: 'center', gap: '8px', marginTop: '4px' }}>
                                                    <input type="checkbox" name="detFlgInclusion" checked={formData.detFlgInclusion} onChange={handleInputChange} style={{ width: '18px', height: '18px', cursor: 'pointer' }} id="flgInc" />
                                                    <label htmlFor="flgInc" className="form-label" style={{ marginBottom: 0, cursor: 'pointer' }}>Flag Inclusion</label>
                                                </div>
                                            </div>
                                        </div>

                                        <div className="form-col">
                                            <h4 style={{ fontSize: '14px', fontWeight: '600', color: '#374151', marginBottom: '16px' }}>Article References</h4>
                                            <div style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}>
                                                <div>
                                                    <label className="form-label">Code Node 1</label>
                                                    <input type="text" name="artCodNode1" className="form-input" value={formData.artCodNode1} onChange={handleInputChange} placeholder="Node 1" />
                                                </div>
                                                <div>
                                                    <label className="form-label">Code Node 2</label>
                                                    <input type="text" name="artCodNode2" className="form-input" value={formData.artCodNode2} onChange={handleInputChange} placeholder="Node 2" />
                                                </div>
                                                <div>
                                                    <label className="form-label">Code Node N</label>
                                                    <input type="text" name="artCodNodeN" className="form-input" value={formData.artCodNodeN} onChange={handleInputChange} placeholder="Node N" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <hr style={{ border: 'none', borderTop: '1px solid #f3f4f6', margin: '24px 0' }} />
                                    <h4 style={{ fontSize: '14px', fontWeight: '600', color: '#374151', marginBottom: '16px' }}>Measure Configuration</h4>
                                    <div className="form-row">
                                        <div className="form-col">
                                            <label className="form-label">Field Name</label>
                                            <input type="text" name="measFieldName" className="form-input" value={formData.measFieldName} onChange={handleInputChange} placeholder="Enter field name" />
                                        </div>
                                        <div className="form-col">
                                            <label className="form-label">Formula</label>
                                            <input type="text" name="measFormula" className="form-input" value={formData.measFormula} onChange={handleInputChange} placeholder="Enter formula" />
                                        </div>
                                    </div>
                                </>
                            )}

                            <div className="submit-btn-wrapper">
                                <button className="submit-btn bg-green-500 hover:bg-green-600" onClick={handleSubmit}>
                                    Add Product(s) <Check size={18} />
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            )}

            {/* Promotion Selection Modal */}
            {showPromotionModal && (
                <div className="modal-overlay" onClick={() => setShowPromotionModal(false)}>
                    <div className="modal-content" style={{ maxWidth: '700px' }} onClick={(e) => e.stopPropagation()}>
                        <div className="modal-header">
                            <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                                <div className="icon-wrapper">
                                    <Package size={20} />
                                </div>
                                <h2 className="modal-title">Select Promotion</h2>
                            </div>
                            <button className="close-btn" onClick={() => setShowPromotionModal(false)}>
                                <X size={20} />
                            </button>
                        </div>
                        <div className="modal-body">
                            <div className="search-controls" style={{ padding: '0', marginBottom: '20px' }}>
                                <input
                                    type="text"
                                    className="search-term-input"
                                    placeholder="Search by Name or ID"
                                    value={promotionSearchTerm}
                                    onChange={(e) => setPromotionSearchTerm(e.target.value)}
                                />
                            </div>
                            <div className="view-table-wrapper" style={{ maxHeight: '400px', overflowY: 'auto' }}>
                                <table className="view-data-table">
                                    <thead>
                                        <tr>
                                            <th>ID</th>
                                            <th>Name</th>
                                            <th>Division</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {filteredPromotions.length > 0 ? (
                                            filteredPromotions.map((promo, idx) => (
                                                <tr
                                                    key={idx}
                                                    onClick={() => handlePromotionSelect(promo)}
                                                    style={{ cursor: 'pointer' }}
                                                    className="hover:bg-purple-50"
                                                >
                                                    <td>{promo.idAction || promo.IdAction}</td>
                                                    <td>{promo.name || promo.Name}</td>
                                                    <td>{promo.codDiv || promo.CodDiv}</td>
                                                </tr>
                                            ))
                                        ) : (
                                            <tr><td colSpan="3" style={{ textAlign: 'center', padding: '20px' }}>No promotions found.</td></tr>
                                        )}
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            )}

            {/* Product Selection Modal (Multi-Select) */}
            {showProductModal && (
                <div className="modal-overlay" onClick={() => setShowProductModal(false)}>
                    <div className="modal-content" style={{ maxWidth: '800px' }} onClick={(e) => e.stopPropagation()}>
                        <div className="modal-header">
                            <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                                <div className="icon-wrapper" style={{ background: '#E0F2FE', color: '#0EA5E9' }}>
                                    <Package size={20} />
                                </div>
                                <h2 className="modal-title">Select Products to Add</h2>
                            </div>
                            <button className="close-btn" onClick={() => setShowProductModal(false)}>
                                <X size={20} />
                            </button>
                        </div>
                        <div className="modal-body">
                            <div className="search-controls" style={{ padding: '0', marginBottom: '20px', gap: '12px' }}>
                                <input
                                    type="text"
                                    className="search-term-input"
                                    placeholder="Search Products by Code or Name"
                                    value={productSearchTerm}
                                    onChange={(e) => setProductSearchTerm(e.target.value)}
                                />
                                <button className="create-btn" onClick={() => setShowProductModal(false)}>
                                    Confirm Selection ({selectedProducts.length})
                                </button>
                            </div>
                            <div className="view-table-wrapper" style={{ maxHeight: '400px', overflowY: 'auto' }}>
                                <table className="view-data-table">
                                    <thead>
                                        <tr>
                                            <th style={{ width: '50px' }}>Select</th>
                                            <th>Code Product</th>
                                            <th>Code Display</th>
                                            <th>Code Div</th>
                                            <th>Measure</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {filteredModalProducts.length > 0 ? (
                                            filteredModalProducts.map((p, idx) => {
                                                const isSelected = selectedProducts.some(sel => (sel.codProduct || sel.CodProduct) === (p.codProduct || p.CodProduct));
                                                return (
                                                    <tr
                                                        key={idx}
                                                        onClick={() => toggleProductSelection(p)}
                                                        style={{ cursor: 'pointer', backgroundColor: isSelected ? '#f0fdf4' : 'inherit' }}
                                                        className="hover:bg-gray-50"
                                                    >
                                                        <td style={{ textAlign: 'center' }}>
                                                            <div style={{
                                                                width: '18px',
                                                                height: '18px',
                                                                border: isSelected ? 'none' : '2px solid #d1d5db',
                                                                borderRadius: '4px',
                                                                backgroundColor: isSelected ? '#22c55e' : 'white',
                                                                display: 'flex',
                                                                alignItems: 'center',
                                                                justifyContent: 'center',
                                                                color: 'white',
                                                                margin: '0 auto'
                                                            }}>
                                                                {isSelected && <Check size={12} strokeWidth={4} />}
                                                            </div>
                                                        </td>
                                                        <td>{p.codProduct || p.CodProduct}</td>
                                                        <td>{p.codDisplay || p.CodDisplay}</td>
                                                        <td>{p.codDiv || p.CodDiv}</td>
                                                        <td>{p.codMeasure || p.CodMeasure}</td>
                                                    </tr>
                                                );
                                            })
                                        ) : (
                                            <tr><td colSpan="5" style={{ textAlign: 'center', padding: '20px' }}>No products found.</td></tr>
                                        )}
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default Products;
