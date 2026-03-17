import { ApplicationInsights } from '@microsoft/applicationinsights-web';

// Connection string from env: set VITE_APPINSIGHTS_CONNECTION_STRING in .env or build (no hardcoded secrets)
const connectionString = import.meta.env.VITE_APPINSIGHTS_CONNECTION_STRING;

const appInsights = connectionString
    ? new ApplicationInsights({
        config: {
            connectionString
        }
    })
    : null;

if (appInsights) {
    appInsights.loadAppInsights();
    appInsights.trackPageView();
}

export { appInsights };
