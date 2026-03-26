import React from 'react';

class ErrorBoundary extends React.Component {
    constructor(props) {
        super(props);
       this.state = {
                        hasError: false, //if error occour or not 
                        error: null,     //Actual JS error
                        errorInfo: null //Component stack trace
                    };

    }

    static getDerivedStateFromError(error) {
        return { hasError: true };
    }

    componentDidCatch(error, errorInfo) {
        this.setState({ error, errorInfo });
        console.error("MFE Error Caught:", error, errorInfo);
    }

    render() {
        if (this.state.hasError) {
            return (
                <div style={{ padding: '20px', backgroundColor: '#fee2e2', border: '1px solid #ef4444', borderRadius: '8px', color: '#b91c1c' }}>
                    <h2>⚠️ Module Load Failed</h2>
                    <details style={{ whiteSpace: 'pre-wrap', marginTop: '10px' }}>
                        <summary>Show Error Details</summary>
                        {this.state.error && this.state.error.toString()}
                        <br />
                        {this.state.errorInfo && this.state.errorInfo.componentStack}
                    </details>
                    <p style={{ marginTop: '10px' }}>
                        <strong>Required:</strong> the promotions app is a Module Federation <em>remote</em> — it must run <code>vite build</code> so <code>remoteEntry.js</code> exists. Plain <code>vite</code> alone does not serve a remote entry (plugin limitation).
                    </p>
                    <p style={{ marginTop: '8px' }}>
                        From <code>MyCompany.WebApp</code>: <code>npm install</code> then <code>npm run dev</code> (starts host 5001 + promotions build+preview on 5002). Or: (1) <code>cd apps/promotions && npm run dev</code> — wait for the build to finish — (2) <code>cd apps/host && npm run dev</code>.
                    </p>
                    <p style={{ marginTop: '8px' }}>
                        Test: <a href="/promotions/remoteEntry.js" target="_blank" rel="noreferrer">/promotions/remoteEntry.js</a> (same-origin via host → preview) should return JavaScript. Direct <a href="http://127.0.0.1:5002/remoteEntry.js" target="_blank" rel="noreferrer">:5002/remoteEntry.js</a> only if preview is running. Avoid loading the remote from <strong>127.0.0.1</strong> while the app is on <strong>localhost</strong> (or the reverse) — that cross-origin combo often causes <strong>Failed to fetch</strong> on <code>import()</code>. Do not point <code>VITE_PROMOTIONS_*</code> at port <strong>5089</strong> (gateway) or chunks get <strong>401</strong>.
                    </p>
                </div>
            );
        }

        return this.props.children;
    }
}

export default ErrorBoundary;
