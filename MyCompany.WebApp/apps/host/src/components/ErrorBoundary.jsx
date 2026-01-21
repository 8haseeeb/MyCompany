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
                        Please ensure the Remote App is running on port 5002 and CORS is enabled.
                    </p>
                </div>
            );
        }

        return this.props.children;
    }
}

export default ErrorBoundary;
